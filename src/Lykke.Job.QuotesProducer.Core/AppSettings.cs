namespace Lykke.Job.QuotesProducer.Core
{
    public class AppSettings
    {
        public QuotesProducerSettings QuotesProducerJob { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }

        public class QuotesProducerSettings
        {
            public RabbitSettingsWithDeadLetter OrderbookSubscription { get; set; }
            public RabbitSettings QuotesPublication { get; set; }
            public DbSettings Db { get; set; }
        }

        public class RabbitSettings
        {
            public string ConnectionString { get; set; }
            public string ExchangeName { get; set; }
        }

        public class RabbitSettingsWithDeadLetter : RabbitSettings
        {
            public string DeadLetterExchangeName { get; set; }
        }

        public class DbSettings
        {
            public string LogsConnString { get; set; }
        }

        public class SlackNotificationsSettings
        {
            public AzureQueueSettings AzureQueue { get; set; }
        }

        public class AzureQueueSettings
        {
            public string ConnectionString { get; set; }

            public string QueueName { get; set; }
        }
    }
}