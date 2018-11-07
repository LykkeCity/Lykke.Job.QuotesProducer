using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Job.QuotesProducer.Contract;
using Lykke.Job.QuotesProducer.Core.Services.Quotes;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.Job.QuotesProducer.Services.Quotes
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public class QuotesPublisher : IQuotesPublisher
    {
        private readonly ILogFactory _logFactory;
        private readonly string _rabbitConnectionString;
        private RabbitMqPublisher<QuoteMessage> _publisher;

        public QuotesPublisher(
            ILogFactory logFactory,
            string rabbitConnectionString)
        {
            _logFactory = logFactory;
            _rabbitConnectionString = rabbitConnectionString;
        }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .ForPublisher(_rabbitConnectionString, "quotefeed")
                .MakeDurable();

            _publisher = new RabbitMqPublisher<QuoteMessage>(_logFactory, settings)
                .SetSerializer(new JsonMessageSerializer<QuoteMessage>())
                .SetPublishStrategy(new DefaultFanoutPublishStrategy(settings))
                .PublishSynchronously()
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

        public Task PublishAsync(QuoteMessage candle)
        {
            return _publisher.ProduceAsync(candle);
        }
    }
}
