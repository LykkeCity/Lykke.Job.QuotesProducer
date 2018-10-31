using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.QuotesProducer.Settings
{
    [UsedImplicitly]
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }
    }
}
