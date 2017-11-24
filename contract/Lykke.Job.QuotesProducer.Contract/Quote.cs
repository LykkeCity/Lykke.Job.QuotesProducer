using System;
using JetBrains.Annotations;

namespace Lykke.Job.QuotesProducer.Contract
{
    [PublicAPI]
    public class QuoteMessage
    {
        public string AssetPair { get; set; }
        public bool IsBuy { get; set; }
        public double Price { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
