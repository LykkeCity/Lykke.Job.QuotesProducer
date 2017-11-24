using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.QuotesProducer.Core.Services.Quotes
{
    public interface IQuotesManager
    {
        Task ProcessOrderBookAsync(string assetPair, bool isBuy, DateTime timestamp, IEnumerable<double> prices);
    }
}
