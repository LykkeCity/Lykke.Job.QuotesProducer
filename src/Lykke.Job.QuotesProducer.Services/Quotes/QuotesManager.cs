using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Job.QuotesProducer.Core.Services.Quotes;

namespace Lykke.Job.QuotesProducer.Services.Quotes
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public class QuotesManager : IQuotesManager
    {
        private readonly IQuotesPublisher _publisher;
        private readonly IQuotesGenerator _generator;

        public QuotesManager(IQuotesPublisher publisher, IQuotesGenerator generator)
        {
            _publisher = publisher;
            _generator = generator;
        }

        public Task ProcessOrderBookAsync(string assetPair, bool isBuy, DateTime timestamp, IEnumerable<double> prices)
        {
            var quote = _generator.Generate(assetPair, isBuy, timestamp, prices);

            return _publisher.PublishAsync(quote);
        }
    }
}
