using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StolenCarStatelessActor.Interfaces;
using Microsoft.ServiceFabric;
using Microsoft.ServiceFabric.Actors;

namespace StolenCarStatelessActor
{
    public class StolenCarStatelessActor : Actor, IStolenCarStatelessActor
    {
        public async Task<string> DoWorkAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Doing Work");

            return await Task.FromResult("Work result");
        }

        public async Task<bool> StartSendingCarDetails(List<StolenCarModel> cameraDetails)
        {
            ActorEventSource.Current.ActorMessage(this, "Start Sending Data");
            EventHubLayer hub = new EventHubLayer();
            var sendDetails = await hub.SendEventsToEventHub(cameraDetails);
            return sendDetails;
        }
    }
}
