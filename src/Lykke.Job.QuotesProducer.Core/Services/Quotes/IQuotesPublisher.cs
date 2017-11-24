using System.Threading.Tasks;
using Autofac;
using Common;
using Lykke.Domain.Prices.Contracts;

namespace Lykke.Job.QuotesProducer.Core.Services.Quotes
{
    public interface IQuotesPublisher : IStartable, IStopable
    {
        Task PublishAsync(IQuote candle);
    }
}