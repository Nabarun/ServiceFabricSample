using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using ThreatInterface;

namespace ThreatObserver
{
    [ActorService(Name="MachineIssue")]
    class MachineIssueObserver : Actor<ThreatState>, IIssueObserver
    {
        public async Task<bool> Update(ThreatState value)
        {
            try
            {
                if (value.ExceptionType.Equals("Test"))
                {
                    this.State = value;
                    Console.WriteLine(this.State.ExceptionType + " : " + this.State.ExceptionMessage + " " +
                                      this.State.TestRunId);
                    ActorEventSource.Current.ActorMessage(this,
                        "Exception Message: {0}" +
                        "Exception Type: {1}" +
                        "Test Run Id: {2}" +
                        "Test Id: {3}",
                        value.ExceptionMessage,
                        value.ExceptionType,
                        value.TestRunId,
                        value.TestId);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Exception seen in Machine Issue Observer : {0}", ex.InnerException.Message);
                return false;
            }
            return true;
        }
    }
}
