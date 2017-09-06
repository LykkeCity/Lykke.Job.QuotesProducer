using System;
using System.Linq;
using Lykke.Domain.Prices.Contracts;
using Lykke.Domain.Prices.Model;
using Lykke.Job.QuotesProducer.Core.Services.Quotes;

namespace Lykke.Job.QuotesProducer.Services.Quotes
{
    public class QuotesGenerator : IQuotesGenerator
    {
        public IQuote Generate(IOrderBook orderBook)
        {
            // Calculate order min/max
            var extremPrice = orderBook.IsBuy
                ? orderBook.Prices.Select(vp => vp.Price).Aggregate(Math.Max)
                : orderBook.Prices.Select(vp => vp.Price).Aggregate(Math.Min);

            return new Quote
            {
                AssetPair = orderBook.AssetPair,
                IsBuy = orderBook.IsBuy,
                Price = extremPrice,
                Timestamp = orderBook.Timestamp
            };
        }
    }
}