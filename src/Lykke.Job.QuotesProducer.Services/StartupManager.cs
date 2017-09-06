using System.Threading.Tasks;
using Common.Log;
using Lykke.Job.QuotesProducer.Core.Services;
using Lykke.Job.QuotesProducer.Core.Services.Quotes;

namespace Lykke.Job.QuotesProducer.Services
{
    public class StartupManager : IStartupManager
    {
        private readonly ILog _log;
        private readonly IOrderBookSubscriber _orderBookSubscriber;
        private readonly IQuotesPublisher _quotesPublisher;

        public StartupManager(
            ILog log,
            IOrderBookSubscriber orderBookSubscriber,
            IQuotesPublisher quotesPublisher)
        {
            _log = log;
            _orderBookSubscriber = orderBookSubscriber;
            _quotesPublisher = quotesPublisher;
        }

        public async Task StartAsync()
        {
            await _log.WriteInfoAsync(nameof(StartupManager), nameof(StartAsync), "", "Starting quotes publisher...");

            _quotesPublisher.Start();

            await _log.WriteInfoAsync(nameof(StartupManager), nameof(StartAsync), "", "Starting order book subscriber...");

            _orderBookSubscriber.Start();

            await _log.WriteInfoAsync(nameof(StartupManager), nameof(StartAsync), "", "Started up");
        }
    }
}