using JetBrains.Annotations;
using Lykke.Sdk.Settings;

namespace Lykke.Job.QuotesProducer.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public QuotesProducerSettings QuotesProducerJob { get; set; }
    }
}
