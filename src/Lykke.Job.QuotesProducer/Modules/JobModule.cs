using Autofac;
using JetBrains.Annotations;
using Lykke.Job.QuotesProducer.Core.Services.Quotes;
using Lykke.Job.QuotesProducer.Services;
using Lykke.Job.QuotesProducer.Services.Quotes;
using Lykke.Job.QuotesProducer.Settings;
using Lykke.Sdk;
using Lykke.Sdk.Health;
using Lykke.SettingsReader;

namespace Lykke.Job.QuotesProducer.Modules
{
    [UsedImplicitly]
    public class JobModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;

        public JobModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();

            builder.RegisterType<OrderBookSubscriber>()
                .As<IOrderBookSubscriber>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_settings.CurrentValue.QuotesProducerJob.Rabbit.OrderbookSubscription));

            builder.RegisterType<QuotesPublisher>()
                .As<IQuotesPublisher>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_settings.CurrentValue.QuotesProducerJob.Rabbit.QuotesPublication));

            builder.RegisterType<QuotesManager>()
                .As<IQuotesManager>();

            builder.RegisterType<QuotesGenerator>()
                .As<IQuotesGenerator>();
        }
    }
}
