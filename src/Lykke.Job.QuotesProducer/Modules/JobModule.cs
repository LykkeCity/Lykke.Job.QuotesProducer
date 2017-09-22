using Autofac;
using AzureStorage.Blob;
using Common.Log;
using Lykke.Domain.Prices.Contracts;
using Lykke.Domain.Prices.Model;
using Lykke.Job.QuotesProducer.Core;
using Lykke.Job.QuotesProducer.Core.Services;
using Lykke.Job.QuotesProducer.Core.Services.Quotes;
using Lykke.Job.QuotesProducer.Services;
using Lykke.Job.QuotesProducer.Services.Quotes;
using Lykke.RabbitMq.Azure;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.SettingsReader;

namespace Lykke.Job.QuotesProducer.Modules
{
    public class JobModule : Module
    {
        private readonly AppSettings.QuotesProducerSettings _settings;
        private readonly IReloadingManager<AppSettings.DbSettings> _dbSettings;
        private readonly ILog _log;

        public JobModule(AppSettings.QuotesProducerSettings settings, IReloadingManager<AppSettings.DbSettings> dbSettings, ILog log)
        {
            _settings = settings;
            _dbSettings = dbSettings;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
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
                .WithParameter(TypedParameter.From(_settings.Rabbit.OrderbookSubscription));

            builder.RegisterType<QuotesPublisher>()
                .As<IQuotesPublisher>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_settings.Rabbit.QuotesPublication))
                .WithParameter(TypedParameter.From<IPublishingQueueRepository<IQuote>>(
                    new BlobPublishingQueueRepository<Quote, IQuote>(
                        AzureBlobStorage.Create(_dbSettings.ConnectionString(x => x.PublisherMessageSnapshotsConnString)))));

            builder.RegisterType<QuotesManager>()
                .As<IQuotesManager>();

            builder.RegisterType<QuotesGenerator>()
                .As<IQuotesGenerator>();
        }
    }
}