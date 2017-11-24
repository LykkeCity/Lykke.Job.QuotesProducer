using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Lykke.Job.QuotesProducer.Contract;
using Lykke.Job.QuotesProducer.Core.Services.Quotes;

namespace Lykke.Job.QuotesProducer.Services.Quotes
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public class QuotesGenerator : IQuotesGenerator
    {
        public QuoteMessage Generate(string assetPair, bool isBuy, DateTime timestamp, IEnumerable<double> prices)
        {
            // Calculates best price
            var bestPrice = isBuy
                ? prices.Where(p => p > 0).Aggregate(Math.Max)
                : prices.Where(p => p > 0).Aggregate(Math.Min);

            return new QuoteMessage
            {
                AssetPair = assetPair,
                IsBuy = isBuy,
                Price = bestPrice,
                Timestamp = timestamp
            };
        }
    }
}
