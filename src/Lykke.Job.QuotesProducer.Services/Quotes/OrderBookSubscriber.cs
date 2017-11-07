using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AsyncFriendlyStackTrace;
using Common;
using Common.Log;
using Lykke.Domain.Prices.Contracts;
using Lykke.Domain.Prices.Model;
using Lykke.Job.QuotesProducer.Core.Services.Quotes;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.Job.QuotesProducer.Services.Quotes
{
    public class OrderBookSubscriber : IOrderBookSubscriber
    {
        private readonly ILog _log;
        private readonly IQuotesManager _quotesManager;
        private readonly string _rabbitConnectionString;
        private RabbitMqSubscriber<IOrderBook> _subscriber;

        public OrderBookSubscriber(ILog log, IQuotesManager quotesManager, string rabbitConnectionString)
        {
            _log = log;
            _quotesManager = quotesManager;
            _rabbitConnectionString = rabbitConnectionString;
        }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .CreateForSubscriber(_rabbitConnectionString, "orderbook", "quotesproducer")
                .MakeDurable();

            try
            {
                _subscriber = new RabbitMqSubscriber<IOrderBook>(settings,
                    new ResilientErrorHandlingStrategy(_log, settings,
                        retryTimeout: TimeSpan.FromSeconds(10),
                        retryNum: 10,
                        next: new DeadQueueErrorHandlingStrategy(_log, settings)))
                .SetMessageDeserializer(new JsonMessageDeserializer<OrderBook>())
                .SetMessageReadStrategy(new MessageReadQueueStrategy())
                .Subscribe(ProcessOrderBookAsync)
                .CreateDefaultBinding()
                .SetLogger(_log)
                .Start();
        }
            catch (Exception ex)
            {
                _log.WriteErrorAsync(nameof(OrderBookSubscriber), nameof(Start), null, ex).Wait();
                throw;
            }
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
        }

        public void Stop()
        {
            _subscriber?.Stop();
        }

        private async Task ProcessOrderBookAsync(IOrderBook orderBook)
        {
            var validationErrors = ValidateOrderBook(orderBook);
            if (validationErrors.Any())
            {
                var message = string.Join("\r\n", validationErrors);
                var context = new
                {
                    orderBook.AssetPair,
                    orderBook.IsBuy,
                    orderBook.Timestamp
                };
                await _log.WriteWarningAsync(nameof(OrderBookSubscriber), nameof(ProcessOrderBookAsync), context.ToJson(), message);

                return;
            }

            // Is to frequent case, to log it

            if (!orderBook.Prices.Any())
            {
                return;
            }

            await _quotesManager.ProcessOrderBookAsync(orderBook);

        }

        private static ICollection<string> ValidateOrderBook(IOrderBook orderBook)
        {
            var errors = new List<string>();

            if (orderBook == null)
            {
                errors.Add("Argument 'Order' is null.");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(orderBook.AssetPair))
                {
                    errors.Add("'AssetPair' is empty");
                }
                if ((orderBook.Timestamp == DateTime.MinValue || orderBook.Timestamp == DateTime.MaxValue))
                {
                    errors.Add($"Invalid 'Timestamp' range: '{ (object)orderBook.Timestamp}'");
                }
                if (orderBook.Timestamp.Kind != DateTimeKind.Utc)
                {
                    errors.Add($"Invalid 'Timestamp' Kind (UTC is required): '{ (object)orderBook.Timestamp}'");
                }
                if (orderBook.Prices == null)
                {
                    errors.Add("Invalid 'Prices': null");
                }
                else if (orderBook.Prices.All(p => p.Price <= 0) && orderBook.Prices.Any())
                {
                    var prices = orderBook.Prices.Limit(10).Select(p => p.Price.ToString(CultureInfo.InvariantCulture));

                    errors.Add($"All prices is negative or zero. Top 10: [{string.Join(", ", prices)}]");
                }
            }

            return errors;
        }
    }
}
