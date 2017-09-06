using Autofac;
using Common.Log;
using Lykke.Job.QuotesProducer.Core;
using Lykke.Job.QuotesProducer.Core.Services;
using Lykke.Job.QuotesProducer.Core.Services.Quotes;
using Lykke.Job.QuotesProducer.Services;
using Lykke.Job.QuotesProducer.Services.Quotes;

namespace Lykke.Job.QuotesProducer.Modules
{
    public class JobModule : Module
    {
        private readonly AppSettings.QuotesProducerSettings _settings;
        private readonly ILog _log;

        public JobModule(AppSettings.QuotesProducerSettings settings, ILog log)
        {
            _settings = settings;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_settings)
                .SingleInstance();

            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

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
                .WithParameter(TypedParameter.From(_settings.OrderbookSubscription));

            builder.RegisterType<QuotesPublisher>()
                .As<IQuotesPublisher>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_settings.QuotesPublication));

            builder.RegisterType<QuotesManager>()
                .As<IQuotesManager>();

            builder.RegisterType<QuotesGenerator>()
                .As<IQuotesGenerator>();
        }
    }
}