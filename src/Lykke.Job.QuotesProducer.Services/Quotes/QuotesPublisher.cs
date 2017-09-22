using System.Threading.Tasks;
using Common.Log;
using Lykke.Domain.Prices.Contracts;
using Lykke.Job.QuotesProducer.Core.Services.Quotes;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.Job.QuotesProducer.Services.Quotes
{
    public class QuotesPublisher : IQuotesPublisher
    {
        private readonly ILog _log;
        private readonly string _rabbitConnectionString;
        private readonly IPublishingQueueRepository<IQuote> _publishingQueueRepository;
        private RabbitMqPublisher<IQuote> _publisher;

        public QuotesPublisher(
            ILog log,
            string rabbitConnectionString,
            IPublishingQueueRepository<IQuote> publishingQueueRepository)
        {
            _log = log;
            _rabbitConnectionString = rabbitConnectionString;
            _publishingQueueRepository = publishingQueueRepository;
        }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .CreateForPublisher(_rabbitConnectionString, "quotefeed")
                .MakeDurable();

            _publisher = new RabbitMqPublisher<IQuote>(settings)
                .SetSerializer(new JsonMessageSerializer<IQuote>())
                .SetPublishStrategy(new DefaultFanoutPublishStrategy(settings))
                .SetQueueRepository(_publishingQueueRepository)
                .SetLogger(_log)
                .Start();
        }

        public void Dispose()
        {
            _publisher?.Dispose();
        }

        public void Stop()
        {
            _publisher?.Stop();
        }

        public Task PublishAsync(IQuote candle)
        {
            return _publisher.ProduceAsync(candle);
        }
    }
}