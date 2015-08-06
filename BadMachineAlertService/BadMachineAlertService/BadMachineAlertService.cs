using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Description;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BadMachineEntity;
using EmailClient;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services;
using Microsoft.ServiceFabric.Services.Wcf;

namespace BadMachineAlertService
{
    public class BadMachineAlertService : StatefulService, IBadMachineDetails
    {
        private string _analysisSqlConnectionString;
        private string _emailFromAlias;
        private string _emailSubject;
        private string _emailToAlias;
        private string _logFileLocation;
        private string _sendGridPassword;
        private string _sendGridUserName;
        private string _sqlConnectionString;
        private int _fetchPastTestRunsInDays;

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                var dictionary = await StateManager.GetOrAddAsync<IReliableDictionary<string, List<Machine>>>
                    ("MachineDictionary");

                //Clear the dictionary
                if (dictionary.Any())
                {
                    await dictionary.ClearAsync();
                }
                var endpoint =
                    ServiceInitializationParameters.CodePackageActivationContext.GetEndpoint("ServiceEndPoint").Port;

                var configPackage = ServiceInitializationParameters.
                    CodePackageActivationContext.GetConfigurationPackageObject("Config");

                UpdateApplicationSettings(configPackage.Settings);

                ServiceInitializationParameters.CodePackageActivationContext.ConfigurationPackageModifiedEvent +=
                    CodePackageActivationContext_ConfigurationPackageModifiedEvent;

                var dbUtility = new DbUtility(_sqlConnectionString, _logFileLocation);

                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    ServiceEventSource.Current.ServiceMessage(
                        this, $"Get Test data from past {_fetchPastTestRunsInDays} days");

                    var machineList = dbUtility.GetRunDetails(_fetchPastTestRunsInDays);

                    //Clean the dictionary before every iteration
                    await dictionary.ClearAsync();

                    foreach (var machine in machineList)
                    {
                        using (var tx = StateManager.CreateTransaction())
                        {
                            await
                                dictionary.AddOrUpdateAsync(tx, machine.MachineName, new List<Machine> {machine},
                                    (k, v) =>
                                    {
                                        var list = dictionary.TryGetValueAsync(tx, k).Result;
                                        if (
                                            !list.Value.Any(
                                                e =>
                                                    e.MachineName == machine.MachineName && e.RunId == machine.RunId &&
                                                    e.TaskId == machine.TaskId))
                                        {
                                            list.Value.Add(machine);
                                        }

                                        return list.Value;
                                    });

                            await tx.CommitAsync();
                        }
                    }

                    ServiceEventSource.Current.ServiceMessage(
                        this,
                        $"Total Custom Objects: {await dictionary.GetCountAsync()}.");


                    //Send Email from SendGrid
                    SendEmail(dictionary);

                    ServiceEventSource.Current.ServiceMessage(
                        this, $"Email succesfully Sent to {_emailToAlias}");

                    await Task.Delay(TimeSpan.FromHours(12), cancellationToken);
                }
            }
            catch (AccessViolationException avException)
            {
                ServiceEventSource.Current.ServiceMessage(this,
                    $"Access Violation Exception in BadMachineAlertService: {avException.Message}");
                throw;

            }
            catch (Exception exception)
            {
                ServiceEventSource.Current.ServiceMessage(this,
                    $"Exception in BadMachineAlertService: {exception.Message}");
                throw;
            }
        }

       /// <summary>
       /// Gets and Update the Application Setting without hindering the application lifecycle
       /// </summary>
       /// <param name="applicationSettings"></param>
        private void UpdateApplicationSettings(ConfigurationSettings applicationSettings)
        {
            // For demonstration purposes, this configuration is using the built-in Settings.xml configuration option.
            // We could just as easily put these settings in our custom JSON configuration (RequestSettings.json), or vice-versa.

            try
            {
                var parameters = applicationSettings.Sections["BadMachineAlertConfig"].Parameters;

                _sqlConnectionString = parameters["SQLConnectionString"].Value;
                _analysisSqlConnectionString = parameters["AnalysisSqlConnectionString"].Value;
                _sendGridUserName = parameters["SendGridUserName"].Value;
                _sendGridPassword = parameters["SendGridPassword"].Value;
                _emailFromAlias = parameters["EmailSender"].Value;
                _emailToAlias = parameters["EmailReceiver"].Value;
                _emailSubject = parameters["EmailSubject"].Value;
                _fetchPastTestRunsInDays = Convert.ToInt32(parameters["FetchPastTestRunsInDays"].Value);
                _logFileLocation = parameters["LogFileLocation"].Value;
            }
            catch (Exception exception)
            {
                ServiceEventSource.Current.ServiceMessage(this, exception.ToString());
                LogUtility.WriteLog(LogLevel.Error, "Exception thrown in UpdateApplicationSetting module", exception, _logFileLocation);
            }
        }

        /// <summary>
        ///     This event handler is called whenever a new config package is available.
        ///     We can do whatever we want with the event.
        ///     In this sample, we reconfigure the service without stopping anything.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CodePackageActivationContext_ConfigurationPackageModifiedEvent(object sender,
            PackageModifiedEventArgs<ConfigurationPackage> e)
        {
            // The Settings property contains the built-in Settings.xml config.
            UpdateApplicationSettings(e.NewPackage.Settings);
        }

        /// <summary>
        /// SendEmail will use SendGrid APIs to Send email
        /// </summary>
        /// <param name="dictionary"></param>
        private void SendEmail(IReliableDictionary<string, List<Machine>> dictionary)
        {
            var email = new EmailUtility();
            var sb = new StringBuilder();

            try
            {
                sb.Append("<!DOCTYPE html><html>");
                sb.Append(
                    "<head><meta charset=\"UTF-8\" /><meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\" /><title>Test Report</title></head>");
                sb.Append(
                    "<table border='1'><tr><td>MachineName</td><td>Fail Occurence</td><td>Pass Occurence</td><td>Task</td></tr>");
                
                var orderedDictionary =
                    dictionary.OrderBy(e => e.Value.Count(g => g.IsPass)*10 + e.Value.Count(g => !g.IsPass)*-10);

                //Just send top 10 bad machines
                foreach (var keyValuePair in orderedDictionary.Take(10))
                {
                    var runAndTask = new StringBuilder();
                    runAndTask.Append("<table>" +
                                        "<tr>" +
                                        "<td>Start Time</td>" +
                                        "<td>Completion</td>" +
                                        "<td>Is Pass</td>" +
                                        "<td>IsOfficial</td>" +
                                        "<td>Link</td>" +
                                        "</tr>");
                    foreach (var task in keyValuePair.Value)
                    {
                        var trStyle = !task.IsPass ? "<tr bgcolor='pink'>" : "<tr bgcolor='lightgreen'>";
                        runAndTask.Append(trStyle +
                                      $"<td>{task.TestStartTime}</td>" +
                                      $"<td>{task.TestCompletionTime}</td>" +
                                      $"<td>{task.IsPass}</td>" +
                                      $"<td>{task.IsOfficial}</td>" +
                                      "<td><a href = http://testinfra1/Test/UploadedLogs?testId=" + task.TaskId + ">" +
                                      task.TaskId + "</a></td>" +
                                      "</tr>");
                    }

                    runAndTask.Append("</table>");

                    sb.Append(
                        $"<tr><td>Machine Name: {keyValuePair.Key}</td><td>Fail : {keyValuePair.Value.Count(e => !e.IsPass)}</td><td> Pass: {keyValuePair.Value.Count(e => e.IsPass)}</td><td>{runAndTask}</td></tr>" +
                        Environment.NewLine);
                }
                sb.Append("</table></html>");

                var emailObj = new Email(_emailToAlias, _emailFromAlias, _emailSubject, sb.ToString());
                var sendGridObj = new SendGridDetails(_sendGridUserName, _sendGridPassword);

                email.SendMailUsingSendGrid(emailObj, sendGridObj);
            }
            catch (Exception exception)
            {
                ServiceEventSource.Current.ServiceMessage(this,
                    $"Exception in Sending Email: {exception.Message}");
                throw;
            }
        }

        /// <summary>
        /// Client Interaction with the service will return the contents in Reliable Dictionary
        /// </summary>
        /// <returns></returns>
        public async Task<IDictionary<string, List<Machine>>> GetMachineDetials()
        {
            IDictionary<string, List<Machine>> clientDictionary = new Dictionary<string, List<Machine>>();
            var dictionary = await StateManager.GetOrAddAsync<IReliableDictionary<string, List<Machine>>>
                    ("MachineDictionary");
            foreach (var keyValuePair in dictionary)
            {
                clientDictionary.Add(keyValuePair.Key, keyValuePair.Value);
            }

            return clientDictionary;
        }
    }
}