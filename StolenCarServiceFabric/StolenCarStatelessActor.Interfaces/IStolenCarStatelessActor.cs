using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace StolenCarStatelessActor.Interfaces
{
    public interface IStolenCarStatelessActor : IActor
    {
        Task<string> DoWorkAsync();

        Task<bool> StartSendingCarDetails(List<StolenCarModel> stolenCarList);
    }

    public class StolenCarModel
    {
        public int SerialId { get; set; }

        public string VehicleNumber { get; set; }

        public string CameraSerial { get; set; }

        public string CaptureTime { get; set; }
    }

    public class CameraDetails
    {
        public string CamID { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string Area { get; set; }
    }
}
