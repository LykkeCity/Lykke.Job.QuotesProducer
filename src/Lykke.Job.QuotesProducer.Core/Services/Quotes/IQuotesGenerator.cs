using System;
using System.Collections.Generic;
using Lykke.Job.QuotesProducer.Contract;

namespace Lykke.Job.QuotesProducer.Core.Services.Quotes
{
    public interface IQuotesGenerator
    {
        QuoteMessage Generate(string assetPair, bool isBuy, DateTime timestamp, IEnumerable<double> prices);
    }
}
