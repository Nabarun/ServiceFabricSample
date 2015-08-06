using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCloudStorage
{
    public interface IDeleteStorage
    {
        bool DeleteByName(AzureAccountCredDetails credentials, string containerName);
        bool DeleteByTime(AzureAccountCredDetails credentials, DateTime limit);
    }
}
