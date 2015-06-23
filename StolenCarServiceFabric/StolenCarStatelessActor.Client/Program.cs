using StolenCarStatelessActor.Interfaces;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace StolenCarStatelessActor.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var camera1 = ActorProxy.Create<IStolenCarStatelessActor>(ActorId.NewId(), "fabric:/StolenCarStatelessActorApplication");
            var camera2 = ActorProxy.Create<IStolenCarStatelessActor>(ActorId.NewId(), "fabric:/StolenCarStatelessActorApplication");
            var camera3 = ActorProxy.Create<IStolenCarStatelessActor>(ActorId.NewId(), "fabric:/StolenCarStatelessActorApplication");
            var camera4 = ActorProxy.Create<IStolenCarStatelessActor>(ActorId.NewId(), "fabric:/StolenCarStatelessActorApplication");
            var camera5 = ActorProxy.Create<IStolenCarStatelessActor>(ActorId.NewId(), "fabric:/StolenCarStatelessActorApplication");
            var camera6 = ActorProxy.Create<IStolenCarStatelessActor>(ActorId.NewId(), "fabric:/StolenCarStatelessActorApplication");
            var camera7 = ActorProxy.Create<IStolenCarStatelessActor>(ActorId.NewId(), "fabric:/StolenCarStatelessActorApplication");
            var camera8 = ActorProxy.Create<IStolenCarStatelessActor>(ActorId.NewId(), "fabric:/StolenCarStatelessActorApplication");
            var camera9 = ActorProxy.Create<IStolenCarStatelessActor>(ActorId.NewId(), "fabric:/StolenCarStatelessActorApplication");
            var camera10 = ActorProxy.Create<IStolenCarStatelessActor>(ActorId.NewId(), "fabric:/StolenCarStatelessActorApplication");

            int loop = 0;
            var random = new Random();
            do
            {
                var tollboothList = GetMetricDataList("Camera"+ random.Next(0,100));
                var sendSuccess = camera1.StartSendingCarDetails(tollboothList);
                Console.WriteLine("Sending Camera1 Details {0}: {1}", camera1.GetActorId(), sendSuccess.Result);
                Thread.Sleep(5000);

                tollboothList = GetMetricDataList("Camera" + random.Next(0, 100));
                sendSuccess = camera2.StartSendingCarDetails(tollboothList);
                Console.WriteLine("Sending Camera2 details {0}: {1}", camera2.GetActorId(), sendSuccess.Result);
                Thread.Sleep(5000);

                tollboothList = GetMetricDataList("Camera" + random.Next(0, 100));
                sendSuccess = camera3.StartSendingCarDetails(tollboothList);
                Console.WriteLine("Sending Camera3 details {0}: {1}", camera3.GetActorId(), sendSuccess.Result);
                Thread.Sleep(5000);

                tollboothList = GetMetricDataList("Camera" + random.Next(0, 100));
                sendSuccess = camera4.StartSendingCarDetails(tollboothList);
                Console.WriteLine("Sending Camera4 details {0}: {1}", camera4.GetActorId(), sendSuccess.Result);
                Thread.Sleep(5000);

                tollboothList = GetMetricDataList("Camera" + random.Next(0, 100));
                sendSuccess = camera5.StartSendingCarDetails(tollboothList);
                Console.WriteLine("Sending Camera5 details {0}: {1}", camera5.GetActorId(), sendSuccess.Result);
                Thread.Sleep(5000);

                tollboothList = GetMetricDataList("Camera" + random.Next(0, 100));
                sendSuccess = camera6.StartSendingCarDetails(tollboothList);
                Console.WriteLine("Sending Camera6 details {0}: {1}", camera6.GetActorId(), sendSuccess.Result);
                Thread.Sleep(5000);

                tollboothList = GetMetricDataList("Camera" + random.Next(0, 100));
                sendSuccess = camera7.StartSendingCarDetails(tollboothList);
                Console.WriteLine("Sending Camera7 details {0}: {1}", camera7.GetActorId(), sendSuccess.Result);
                Thread.Sleep(5000);

                tollboothList = GetMetricDataList("Camera" + random.Next(0, 100));
                sendSuccess = camera8.StartSendingCarDetails(tollboothList);
                Console.WriteLine("Sending Camera8 details {0}: {1}", camera8.GetActorId(), sendSuccess.Result);
                Thread.Sleep(5000);

                tollboothList = GetMetricDataList("Camera" + random.Next(0, 100));
                sendSuccess = camera9.StartSendingCarDetails(tollboothList);
                Console.WriteLine("Sending Camera9 details {0}: {1}", camera9.GetActorId(), sendSuccess.Result);
                Thread.Sleep(5000);

                tollboothList = GetMetricDataList("Camera" + random.Next(0, 100));
                sendSuccess = camera10.StartSendingCarDetails(tollboothList);
                Console.WriteLine("Sending Camera10 details {0}: {1}", camera10.GetActorId(), sendSuccess.Result);
                Thread.Sleep(5000);
                loop++;
            } while (loop < 10000);
        }

        /// <summary>
        /// Get List of sample StolenCarModel details which can be sent later to the event hub
        /// </summary>
        /// <returns>Returns the list of stolen car model</returns>
        private static List<StolenCarModel> GetMetricDataList(string camera)
        {
            var random = new Random();
            Random rand = new Random();
            var metricDataList = new List<StolenCarModel>();

            Console.WriteLine();
            Console.WriteLine("#####################{0}####################", camera);

            var date = DateTime.Now;
            var serialId = rand.Next(0, 50);
            var nowTime = date.ToString("g");

            Console.Write("Camera:{0} SerialId:{1} TimeStamp:{2} Vehicle:{3}", camera, serialId, nowTime, "ADC01" + serialId);
            Console.WriteLine();

            var info = new StolenCarModel() { CameraSerial = camera, SerialId = serialId, CaptureTime = nowTime, VehicleNumber = "ADC01" + serialId };

            metricDataList.Add(info);

            Console.WriteLine("##############################################");
            Console.WriteLine();

            return metricDataList;
        }
    }
}
