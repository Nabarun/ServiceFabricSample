using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace ThreatInterface
{
    public interface ISubject : IActor
    {
        Task<bool> RegisterObserver(IIssueObserver observer);

        Task<bool> RemoveObserver(IIssueObserver observer);

        Task<bool> SetState(ThreatState state);
    }
}
