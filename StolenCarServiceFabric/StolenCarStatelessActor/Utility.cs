using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StolenCarStatelessActor
{
    internal class Utility
    {
        /// <summary>
        /// Extract the Hub Connection Details
        /// </summary>
        /// <returns>Returns tuple of string,string</returns>
        public static Tuple<string, string, string, string> ExtractEventHubDetails()
        {
            var fileDoc = GetEventConfigFile();
            var eventHubName = fileDoc.Descendants().First(e => e.Name == "EventHubName").Value;
            var eventHubConnectionString = fileDoc.Descendants().First(e => e.Name == "EventHubConnectionString").Value;
            var eventHubNameSpace = fileDoc.Descendants().First(e => e.Name == "EventHubNameSpace").Value;
            var azureConnectionString = fileDoc.Descendants().First(e => e.Name == "AzureBlobConnectionString").Value;
            var extractEventHubDetails = new Tuple<string, string, string, string>(eventHubName, eventHubConnectionString, eventHubNameSpace, azureConnectionString);

            return extractEventHubDetails;
        }

        public static Tuple<string> ExtractSqlDatabase()
        {
            var fileDoc = GetEventConfigFile();
            var sqlDatabaseConnectionString = fileDoc.Descendants().First(e => e.Name == "SQLConnectionString").Value;

            return new Tuple<string>(sqlDatabaseConnectionString);
        }

        public static Tuple<string, string> ExtractMessageDetails()
        {
            var fileDoc = GetEventConfigFile();
            var numberOfMessage = fileDoc.Descendants().First(e => e.Name == "NumberOfMessage").Value;
            var numberOfPartitions = fileDoc.Descendants().First(e => e.Name == "NumberOfPartitions").Value;
            var extractMessageDetails = new Tuple<string, string>(numberOfMessage, numberOfPartitions);

            return extractMessageDetails;
        }

        /// <summary>
        /// Logs specified message to event log.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="type">The type of the event log entry.</param>
        public static void Log(string message, EventLogEntryType type = EventLogEntryType.Information)
        {
            try
            {
                EventLog.WriteEntry("TestInfraWeb", message, type);
            }
            catch (Exception)
            {
            }
        }

        private static XDocument GetEventConfigFile()
        {
            const string File = "E:/project/StolenCar/Data/EventHubConfiguration.xml";

            if (!System.IO.File.Exists(File))
            {
                throw new FileNotFoundException("File: EventHubConfiguration.xml doesn't exist", File);
            }

            Utility.Log("Loading " + File);

            var doc = XDocument.Load(File);
            return doc;
        }
    }
}
