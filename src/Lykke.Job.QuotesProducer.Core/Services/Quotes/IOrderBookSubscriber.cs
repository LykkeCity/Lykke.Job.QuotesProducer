using Autofac;
using Common;

namespace Lykke.Job.QuotesProducer.Core.Services.Quotes
{
    public interface IOrderBookSubscriber : IStartable, IStopable
    {
    }
}