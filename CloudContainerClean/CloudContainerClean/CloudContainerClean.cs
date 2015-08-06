using System;
using System.Collections.Generic;
using System.Fabric.Description;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanCloudStorage;
using Microsoft.ServiceFabric;
using Microsoft.ServiceFabric.Services;

namespace CloudContainerClean
{
    public class CloudContainerClean : StatelessService
    {
        private string _azureStorageAccountName;
        private string _azureStorageAccountKey;
        private string _azureStorageUri;
        private static DateTime PurgingTime { get; set; }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            
            int iterations = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                ServiceEventSource.Current.ServiceMessage(this, "Working-{0}", iterations++);
                var configPackage = ServiceInitializationParameters.
                    CodePackageActivationContext.GetConfigurationPackageObject("Config");

                var azureCredentialObject = UpdateApplicationSettings(configPackage.Settings);

                //Clean the Azure Table Storage Entity older than the limit date
                IDeleteStorage deleteTable = new DeleteTable();
                var isSuccess = deleteTable.DeleteByTime(azureCredentialObject, PurgingTime);
                if (isSuccess)
                {
                    ServiceEventSource.Current.ServiceMessage(this, "Successfully deleted Azure Containers ");
                }

                //Clean the Azure Blob container older than the limit date
                IDeleteStorage deleteCloudStorage = new DeleteContainer();
                isSuccess = deleteCloudStorage.DeleteByTime(azureCredentialObject, PurgingTime);
                if (isSuccess)
                {
                    ServiceEventSource.Current.ServiceMessage(this, "Successfully deleted Azure Containers ");
                }

                //We need this service run only once in a day
                await Task.Delay(TimeSpan.FromHours(24), cancellationToken);
            }
        }

        /// <summary>
        /// Gets and Update the Application Setting without hindering the application lifecycle
        /// </summary>
        /// <param name="applicationSettings"></param>
        private AzureAccountCredDetails UpdateApplicationSettings(ConfigurationSettings applicationSettings)
        {
            // For demonstration purposes, this configuration is using the built-in Settings.xml configuration option.
            // We could just as easily put these settings in our custom JSON configuration (RequestSettings.json), or vice-versa.

            try
            {
                var numberOfDays = 0;
                var parameters = applicationSettings.Sections["DeleteContainerCOnfig"].Parameters;

                _azureStorageAccountName = parameters["StorageAccountName"].Value;
                _azureStorageAccountKey = parameters["StorageAccountKey"].Value;
                _azureStorageUri = parameters["StorageUri"].Value;

                PurgingTime = new DateTime();
                var limitInDays = parameters["LimitDays"].Value;

                var isNumber = int.TryParse(limitInDays, out numberOfDays);
                if (isNumber)
                {
                    PurgingTime = DateTime.Today.AddDays(-numberOfDays);

                    //Create the Azure Credential object which will be sent to the delete logic
                    var azureCredentialObject = GetAzureCredentialObject(_azureStorageAccountName,
                        _azureStorageAccountKey, _azureStorageUri);

                    return azureCredentialObject;
                }
            }
            catch (Exception exception)
            {
                ServiceEventSource.Current.ServiceMessage(this, exception.ToString());
            }

            return null;
        }

        private AzureAccountCredDetails GetAzureCredentialObject(string accountName, string accountKey, string azureStorageUri)
        {
            var azureCredentials = new AzureAccountCredDetails(accountName, accountKey, azureStorageUri);
            return azureCredentials;
        }
    }
}
