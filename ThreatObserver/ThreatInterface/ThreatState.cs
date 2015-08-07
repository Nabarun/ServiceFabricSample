using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace ThreatInterface
{
    [DataContract]
    public class ThreatState
    {
        [DataMember]
        public string ExceptionMessage;

        [DataMember]
        public string ExceptionType;

        [DataMember]
        public string TestRunId;

        [DataMember]
        public string TestId;

    }
}
