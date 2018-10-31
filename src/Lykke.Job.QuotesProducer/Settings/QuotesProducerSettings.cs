using JetBrains.Annotations;

namespace Lykke.Job.QuotesProducer.Settings
{
    [UsedImplicitly]
    public class QuotesProducerSettings
    {
        public RabbitConnectionSettings Rabbit { get; set; }
        public DbSettings Db { get; set; }
    }
}
