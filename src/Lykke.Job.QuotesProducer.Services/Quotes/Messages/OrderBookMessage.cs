using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Lykke.Job.QuotesProducer.Services.Quotes.Messages
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public class OrderBookMessage
    {
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string AssetPair { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public bool IsBuy { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public DateTime Timestamp { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public IReadOnlyList<PriceEnvelope> Prices { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
        public class PriceEnvelope
        {
            [UsedImplicitly(ImplicitUseKindFlags.Assign)]
            public double Price { get; set; }
        }
    }
}
