using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Job.QuotesProducer.Core.Services.Quotes;
using Lykke.Sdk;

namespace Lykke.Job.QuotesProducer.Services
{
    public class StartupManager : IStartupManager
    {
        private readonly ILog _log;
        private readonly IOrderBookSubscriber _orderBookSubscriber;
        private readonly IQuotesPublisher _quotesPublisher;

        public StartupManager(
            ILogFactory logFactory,
            IOrderBookSubscriber orderBookSubscriber,
            IQuotesPublisher quotesPublisher)
        {
            _log = logFactory.CreateLog(this);
            _orderBookSubscriber = orderBookSubscriber;
            _quotesPublisher = quotesPublisher;
        }

        public async Task StartAsync()
        {
            _log.Info(nameof(StartAsync), "Starting quotes publisher...");

            _quotesPublisher.Start();

            _log.Info(nameof(StartAsync), "Starting order book subscriber...");

            _orderBookSubscriber.Start();

            _log.Info(nameof(StartAsync), "Started up");
        }
    }
}
