using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.Smurf.Core.Models.InputStorage.EvenHub;
using Newtonsoft.Json;
using StolenCarStatelessActor.Interfaces;

namespace StolenCarStatelessActor
{
    internal class EventHubLayer
    {
        private const int NumberOfDevice = 1000;

        public IEventHub EventHubProvider { get; set; }

        private string EventHubName { get; set; }

        private string EventHubConnectionString { get; set; }

        private string EventHubNameSpace { get; set; }

        private string AzureConnectionString { get; set; }

        private int NumberOfMessages { get; set; }

        private int NumberOfPartition { get; set; }

        /// <summary>
        /// Logic to send event hub details 
        /// </summary>
        /// <returns>Returns the task </returns>
        public async Task<bool> SendEventsToEventHub(List<StolenCarModel> cameraDetails)
        {
            try
            {
                this.InitializeEventHub();

                var eventHubObj = new EventHub(this.EventHubName, this.EventHubConnectionString);

                string serviceBusConnectionString = eventHubObj.GetServiceBusConnectionString();
                var namespaceManager = NamespaceManager.CreateFromConnectionString(serviceBusConnectionString);

                eventHubObj.CreateEventHub(this.NumberOfPartition, namespaceManager);

                var tasks = new List<Task>();
                var consumerObj = new Consumer(this.EventHubName, serviceBusConnectionString);
                consumerObj.MessageProcessingWithPartition(this.AzureConnectionString, serviceBusConnectionString);

                var eventDataList = new List<EventData>();
                eventDataList = this.GetEventDataList(cameraDetails);

                ////Send EventData list which will populate the Database
                var sendObj = new Sender(this.EventHubName);
                var sentSuccess = await sendObj.SendBatchAsync(eventDataList, serviceBusConnectionString);
            }
            catch(Exception ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Initialize the Eventhub. This will extract eventhub related information from the configuration xml
        /// </summary>
        private void InitializeEventHub()
        {
            var eventHub = Utility.ExtractEventHubDetails();
            var messageObj = Utility.ExtractMessageDetails();

            this.EventHubName = eventHub.Item1;
            this.EventHubConnectionString = eventHub.Item2;
            this.EventHubNameSpace = eventHub.Item3;
            this.AzureConnectionString = eventHub.Item4;

            this.NumberOfMessages = int.Parse(messageObj.Item1);
            this.NumberOfPartition = int.Parse(messageObj.Item2);
        }

        

        /// <summary>
        /// Gets the serialized event data list from list of stolencar model list which can be sent to the eventhub
        /// </summary>
        /// <param name="carDetails"></param>
        /// <returns>Reurns the list of serialized eventdata </returns>
        private List<EventData> GetEventDataList(List<StolenCarModel> carDetails)
        {
            var eventList = new List<EventData>();
            foreach (var detail in carDetails)
            {
                var serializedString = JsonConvert.SerializeObject(carDetails);
                var data = new EventData(Encoding.UTF8.GetBytes(serializedString));
                eventList.Add(data);
            }

            return eventList;
        }

    }
}
