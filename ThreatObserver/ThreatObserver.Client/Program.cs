
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreatInterface;

namespace ThreatObserver.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
            var proxy = ActorProxy.Create<ISubject>(new ActorId("ThreatObservable"),new Uri("fabric:/ThreatObserverApplication/SubjectActorService"));
            var lstThreats = GetListOfThreats();

            var result1 = proxy.SetState(lstThreats[0]).Result;
            var result2 = proxy.SetState(lstThreats[1]).Result;
            var result3 = proxy.SetState(lstThreats[2]);
            var result4 = proxy.SetState(lstThreats[3]);

            //Assert all the results to true and verify
        }

        private static List<ThreatState> GetListOfThreats()
        {
            var listThreats = new List<ThreatState>();
            listThreats.Add(new ThreatState() {ExceptionMessage = "Azure Storage Exception", ExceptionType = "Azure", TestRunId = "1234",TestId = "2345"});
            listThreats.Add(new ThreatState() { ExceptionMessage = "Bad Machine Exception", ExceptionType = "Machine", TestRunId = "1234", TestId = "2345" });
            listThreats.Add(new ThreatState() { ExceptionMessage = "Test Exception", ExceptionType = "Test", TestRunId = "1234", TestId = "2345" });
            listThreats.Add(new ThreatState() { ExceptionMessage = "Code Exception", ExceptionType = "Code", TestRunId = "1234", TestId = "2345" });

            return listThreats;
        } 
    }
}
