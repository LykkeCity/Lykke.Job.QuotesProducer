using System.Threading.Tasks;

namespace Lykke.Job.QuotesProducer.Core.Services
{
    public interface IShutdownManager
    {
        Task ShutdownAsync();
    }
}