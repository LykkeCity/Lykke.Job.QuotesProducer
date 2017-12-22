using Lykke.Domain.Prices.Contracts;

namespace Lykke.Job.QuotesProducer.Core.Services.Quotes
{
    public interface IQuotesGenerator
    {
        IQuote Generate(IOrderBook orderBook);
    }
}