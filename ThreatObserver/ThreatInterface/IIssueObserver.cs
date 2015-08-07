using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace ThreatInterface
{
    public interface IIssueObserver: IActor
    {
        Task<bool> Update(ThreatState t);
    }
}
