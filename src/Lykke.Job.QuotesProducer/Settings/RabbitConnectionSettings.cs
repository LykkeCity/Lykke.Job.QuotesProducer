using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.QuotesProducer.Settings
{
    [UsedImplicitly]
    public class RabbitConnectionSettings
    {
        [AmqpCheck]
        public string OrderbookSubscription { get; set; }
        [AmqpCheck]
        public string QuotesPublication { get; set; }
    }
}
