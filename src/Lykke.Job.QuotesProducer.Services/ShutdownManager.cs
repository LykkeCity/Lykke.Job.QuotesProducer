using System.Threading.Tasks;
using Common.Log;
using Lykke.Job.QuotesProducer.Core.Services;
using Lykke.Job.QuotesProducer.Core.Services.Quotes;

namespace Lykke.Job.QuotesProducer.Services
{
    public class ShutdownManager : IShutdownManager
    {
        private readonly ILog _log;
        private readonly IOrderBookSubscriber _orderBookSubscriber;
        private readonly IQuotesPublisher _quotesPublisher;

        public ShutdownManager(
            ILog log,
            IOrderBookSubscriber orderBookSubscriber, 
            IQuotesPublisher quotesPublisher)
        {
            _log = log;
            _orderBookSubscriber = orderBookSubscriber;
            _quotesPublisher = quotesPublisher;
        }

        public async Task ShutdownAsync()
        {
            await _log.WriteInfoAsync(nameof(ShutdownManager), nameof(ShutdownAsync), "", "Stopping order book subscriber...");

            _orderBookSubscriber.Stop();

            await _log.WriteInfoAsync(nameof(ShutdownManager), nameof(ShutdownAsync), "", "Stopping quotes publisher...");

            _quotesPublisher.Stop();

            await _log.WriteInfoAsync(nameof(ShutdownManager), nameof(ShutdownAsync), "", "Shutted down");
        }
    }
}