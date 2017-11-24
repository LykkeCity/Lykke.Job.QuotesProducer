using System.Threading.Tasks;
using Autofac;
using Common;
using Lykke.Job.QuotesProducer.Contract;

namespace Lykke.Job.QuotesProducer.Core.Services.Quotes
{
    public interface IQuotesPublisher : IStartable, IStopable
    {
        Task PublishAsync(QuoteMessage candle);
    }
}
