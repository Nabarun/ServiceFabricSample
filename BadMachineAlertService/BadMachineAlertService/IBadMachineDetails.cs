using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using BadMachineEntity;
using Microsoft.ServiceFabric.Services;

namespace BadMachineAlertService
{
    [ServiceContract]
    public interface IBadMachineDetails :  IService
    {

        [OperationContract]
        Task<IDictionary<string, List<Machine>>> GetMachineDetials();
    }
}
