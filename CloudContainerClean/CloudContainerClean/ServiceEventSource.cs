﻿using Microsoft.ServiceFabric.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudContainerClean
{
    [EventSource(Name = "MyCompany-CloudContainerCleanApplication-CloudContainerClean")]
    internal sealed class ServiceEventSource : EventSource
    {
        public static ServiceEventSource Current = new ServiceEventSource();

        [NonEvent]
        public void Message(string message, params object[] args)
        {
            if (this.IsEnabled())
            {
                string finalMessage = string.Format(message, args);
                Message(finalMessage);
            }
        }

        [Event(1, Level = EventLevel.Informational, Message = "{0}")]
        public void Message(string message)
        {
            if (this.IsEnabled())
            {
                WriteEvent(1, message);
            }
        }

        [NonEvent]
        public void ServiceMessage(StatelessService service, string message, params object[] args)
        {
            if (this.IsEnabled())
            {
                string finalMessage = string.Format(message, args);
                ServiceMessage(
                    service.ServiceInitializationParameters.ServiceName.ToString(),
                    service.ServiceInitializationParameters.ServiceTypeName,
                    service.ServiceInitializationParameters.InstanceId,
                    service.ServiceInitializationParameters.PartitionId,
                    service.ServiceInitializationParameters.CodePackageActivationContext.ApplicationName,
                    service.ServiceInitializationParameters.CodePackageActivationContext.ApplicationTypeName,
                    FabricRuntime.GetNodeContext().NodeName,
                    finalMessage);
            }
        }

        [NonEvent]
        public void ServiceMessage(StatefulService service, string message, params object[] args)
        {
            if (this.IsEnabled())
            {
                string finalMessage = string.Format(message, args);
                ServiceMessage(
                    service.ServiceInitializationParameters.ServiceName.ToString(),
                    service.ServiceInitializationParameters.ServiceTypeName,
                    service.ServiceInitializationParameters.ReplicaId,
                    service.ServiceInitializationParameters.PartitionId,
                    service.ServiceInitializationParameters.CodePackageActivationContext.ApplicationName,
                    service.ServiceInitializationParameters.CodePackageActivationContext.ApplicationTypeName,
                    FabricRuntime.GetNodeContext().NodeName,
                    finalMessage);
            }
        }

        [Event(2, Level = EventLevel.Informational, Message = "{7}")]
        private void ServiceMessage(
            string serviceName,
            string serviceTypeName,
            long replicaOrInstanceId,
            Guid partitionId,
            string applicationName,
            string applicationTypeName,
            string nodeName,
            string message)
        {
            if (this.IsEnabled())
            {
                WriteEvent(2, serviceName, serviceTypeName, replicaOrInstanceId, partitionId, applicationName, applicationTypeName, nodeName, message);
            }
        }

        [Event(3, Level = EventLevel.Informational, Message = "Service host process {0} registered service type {1}")]
        public void ServiceTypeRegistered(int hostProcessId, string serviceType)
        {
            WriteEvent(3, hostProcessId, serviceType);
        }

        [NonEvent]
        public void ServiceHostInitializationFailed(Exception e)
        {
            ServiceHostInitializationFailed(e.ToString());
        }

        [Event(4, Level = EventLevel.Error, Message = "Service host initialization failed")]
        private void ServiceHostInitializationFailed(string exception)
        {
            WriteEvent(4, exception);
        }
    }
}
