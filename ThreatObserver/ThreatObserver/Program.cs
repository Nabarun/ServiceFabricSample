using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using Microsoft.ServiceFabric.Actors;
using ThreatInterface;

namespace ThreatObserver
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                using (FabricRuntime fabricRuntime = FabricRuntime.Create())
                {
                    fabricRuntime.RegisterActor(typeof(ThreatObservable));
                    fabricRuntime.RegisterActor(typeof(AzureInfraIssueObserver));
                    fabricRuntime.RegisterActor(typeof(MachineIssueObserver));
                    fabricRuntime.RegisterActor(typeof(ProductIssueObserver));
                    fabricRuntime.RegisterActor(typeof(TestIssueObserver));

                    var machineIssue = new MachineIssueObserver();
                    var testIssue = new TestIssueObserver();
                    var codeIssue = new ProductIssueObserver();
                    var azureIssue = new AzureInfraIssueObserver();

                    ThreatObservable observable = new ThreatObservable();
                    var registerMachine = observable.RegisterObserver(machineIssue);
                    var registerTest = observable.RegisterObserver(testIssue);
                    var registerCode = observable.RegisterObserver(codeIssue);
                    var registerAzure = observable.RegisterObserver(azureIssue);

                    if (!registerMachine.Result || !registerAzure.Result || !registerTest.Result || !registerCode.Result)
                    {
                        Trace.WriteLine("Registeration failed");
                    }
                    Thread.Sleep(Timeout.Infinite);
                }
            }
            catch (Exception e)
            {
                ActorEventSource.Current.ActorHostInitializationFailed(e);
                throw;
            }
        }

    }
}
