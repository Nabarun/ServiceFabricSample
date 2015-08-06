// //------------------------------------------------------------
// // Copyright (c) Microsoft Corporation.  All rights reserved.
// //------------------------------------------------------------

namespace BadMachineEntity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using WinFabric.TestInfra.Common;
    using WinFabric.TestInfra.SchedulingService;

    public class DbUtility : IDisposable
    {
        private readonly List<Machine> _machineSet =  new List<Machine>();

        public DbUtility(string connectionAddress, string logFileLocation)
        {
            var machineMap = new Dictionary<string, string>();
            this.TaskDb = new TaskDB(connectionAddress, 60);
            this.LogFileLocation = logFileLocation;
        }

        private TaskDB TaskDb { get; }
        private string LogFileLocation { get; }

        /// <summary>
        /// Dispose this object when not in use
        /// </summary>
        void IDisposable.Dispose()
        {
            ((IDisposable) this).Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Get the run Details
        /// </summary>
        /// <param name="timeSpan">Taskes the timespan since when it is required to fetch the test details</param>
        /// <returns>Returns List of machine object</returns>
        public List<Machine> GetRunDetails(int timeSpan)
        {
            var runs = this.TaskDb.GetRunData(DateTime.Now.Date.AddDays(-timeSpan), false).ToList();
            var taskunderAnalysis = string.Empty;
            try
            {
                var allRuns = runs.ToList();

                foreach (var run in allRuns)
                {
                    //Get the runId for future analysis. This will help us to log the exception with exact details 
                    taskunderAnalysis = run.InvocationData.UniqueId;

                    GetTestDetails(run);
                }
            }
            catch (Exception exception)
            {
                LogUtility.WriteLog(LogLevel.Error, $"Exception in GetRunDetails {taskunderAnalysis}", exception, this.LogFileLocation);
            }

            return _machineSet;
        }

        /// <summary>
        /// Query based on each runId and get the corresponding test details
        /// </summary>
        /// <param name="run">Contains the details of the run</param>
        private void GetTestDetails(RunData run)
        {
            // This variable will store the taskId which is throwing exception
            var taskId = string.Empty;
            try
            {
                var tests = this.TaskDb.GetTestRunData(run.InvocationData.UniqueId).
                    Where(e => e.IsCanceled() == false);

                var taskRunDatas = tests as TaskRunData[] ?? tests.ToArray();

                if (taskRunDatas.Any())
                {
                    foreach (var test in taskRunDatas)
                    {
                        taskId = test.TaskId;
                        var testReport =
                            this.TaskDb.GetTaskReports(test.TaskId)
                                .Where(e => e.Status.Contains("Running on"))
                                .OrderByDescending(e => e.CreatedTime);

                        var machineList = testReport.First().Comment.Split(',');

                        if (!test.Success)
                        {
                            GetMachineObject(machineList, test, run, false);
                        }
                        else
                        {
                            GetMachineObject(machineList, test, run, true);
                        }
                    }
                }
            }
            catch (ArgumentNullException nullException)
            {
                LogUtility.WriteLog(LogLevel.Error, "Null Exception in GetTestDetails", nullException, this.LogFileLocation);
            }
            catch (Exception exception)
            {
                LogUtility.WriteLog(LogLevel.Error, $"Exception thrown under GetTestDetails for Task: {taskId}", exception, this.LogFileLocation);                    
            }
        }

        /// <summary>
        /// Construct the Machine Object
        /// </summary>
        /// <param name="machineList">List of Machines</param>
        /// <param name="test">Contains the Test details</param>
        /// <param name="run">Contains the run details </param>
        /// <param name="testStatus">Signifies if the test has passed or failed</param>
        private void GetMachineObject(string[] machineList, TaskRunData test, 
            RunData run, bool testStatus)
        {
            foreach (var machine in machineList)
            {
                var machineObj = new Machine()
                {
                    MachineName = machine,
                    SuiteName = test.TaskName,
                    RunId = run.InvocationData.UniqueId,
                    TaskId = test.TaskId,
                    IsPass = testStatus,
                    IsOfficial = run.InvocationData.IsOfficial,
                    TestStartTime = test.StartTime,
                    TestCompletionTime = test.CompletionTime
                };

                _machineSet.Add(machineObj);
            }
        }
    }
}
