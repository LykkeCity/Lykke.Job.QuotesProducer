using System.Threading.Tasks;
using Lykke.Domain.Prices.Contracts;

namespace Lykke.Job.QuotesProducer.Core.Services.Quotes
{
    public interface IQuotesManager
    {
        Task ProcessOrderBookAsync(IOrderBook orderBook);
    }
}