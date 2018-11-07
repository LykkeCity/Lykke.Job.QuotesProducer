using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Job.QuotesProducer.Core.Services.Quotes;
using Lykke.Sdk;

namespace Lykke.Job.QuotesProducer.Services
{
    public class ShutdownManager : IShutdownManager
    {
        private readonly ILog _log;
        private readonly IOrderBookSubscriber _orderBookSubscriber;
        private readonly IQuotesPublisher _quotesPublisher;

        public ShutdownManager(
            ILogFactory logFactory,
            IOrderBookSubscriber orderBookSubscriber, 
            IQuotesPublisher quotesPublisher)
        {
            _log = logFactory.CreateLog(this);
            _orderBookSubscriber = orderBookSubscriber;
            _quotesPublisher = quotesPublisher;
        }

        public async Task StopAsync()
        {
            _log.Info(nameof(StopAsync), "Stopping order book subscriber...");

            _orderBookSubscriber.Stop();

            _log.Info(nameof(StopAsync), "Stopping quotes publisher...");

            _quotesPublisher.Stop();

            _log.Info(nameof(StopAsync), "Shutted down");
        }
    }
}
