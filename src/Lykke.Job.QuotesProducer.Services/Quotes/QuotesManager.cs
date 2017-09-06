using System.Threading.Tasks;
using Lykke.Domain.Prices.Contracts;
using Lykke.Job.QuotesProducer.Core.Services.Quotes;

namespace Lykke.Job.QuotesProducer.Services.Quotes
{
    public class QuotesManager : IQuotesManager
    {
        private readonly IQuotesPublisher _publisher;
        private readonly IQuotesGenerator _generator;

        public QuotesManager(IQuotesPublisher publisher, IQuotesGenerator generator)
        {
            _publisher = publisher;
            _generator = generator;
        }

        public Task ProcessOrderBookAsync(IOrderBook orderBook)
        {
            var quote = _generator.Generate(orderBook);

            return _publisher.PublishAsync(quote);
        }
    }
}