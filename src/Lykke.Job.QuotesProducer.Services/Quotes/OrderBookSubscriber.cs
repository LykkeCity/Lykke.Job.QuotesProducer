using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Job.QuotesProducer.Core.Services.Quotes;
using Lykke.Job.QuotesProducer.Services.Quotes.Messages;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.Job.QuotesProducer.Services.Quotes
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public class OrderBookSubscriber : IOrderBookSubscriber
    {
        private readonly ILog _log;
        private readonly ILogFactory _logFactory;
        private readonly IQuotesManager _quotesManager;
        private readonly string _rabbitConnectionString;
        private RabbitMqSubscriber<OrderBookMessage> _subscriber;

        public OrderBookSubscriber(ILogFactory logFactory, IQuotesManager quotesManager, string rabbitConnectionString)
        {
            _log = logFactory.CreateLog(this);
            _logFactory = logFactory;
            _quotesManager = quotesManager;
            _rabbitConnectionString = rabbitConnectionString;
        }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .ForSubscriber(_rabbitConnectionString, "orderbook", "quotesproducer")
                .MakeDurable();

            try
            {
                _subscriber = new RabbitMqSubscriber<OrderBookMessage>(_logFactory, settings,
                    new ResilientErrorHandlingStrategy(_logFactory, settings,
                        retryTimeout: TimeSpan.FromSeconds(10),
                        retryNum: 10,
                        next: new DeadQueueErrorHandlingStrategy(_logFactory, settings)))
                .SetMessageDeserializer(new JsonMessageDeserializer<OrderBookMessage>())
                .SetMessageReadStrategy(new MessageReadQueueStrategy())
                .Subscribe(ProcessOrderBookAsync)
                .CreateDefaultBinding()
                .Start();
        }
            catch (Exception ex)
            {
                _log.Error(nameof(Start), ex);
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

        private async Task ProcessOrderBookAsync(OrderBookMessage orderBookMessage)
        {
            var validationErrors = ValidateOrderBook(orderBookMessage);
            if (validationErrors.Any())
            {
                var message = string.Join("\r\n", validationErrors);
                var context = new
                {
                    orderBookMessage.AssetPair,
                    orderBookMessage.IsBuy,
                    orderBookMessage.Timestamp
                };

                _log.Warning(nameof(ProcessOrderBookAsync), message, context: context.ToJson());

                return;
            }

            // It is too frequent case to log it
            var prices = orderBookMessage.Prices.Where(p => p.Price > 0).ToArray();

            if (!prices.Any())
            {
                return;
            }

            await _quotesManager.ProcessOrderBookAsync(
                orderBookMessage.AssetPair, 
                orderBookMessage.IsBuy, 
                orderBookMessage.Timestamp,
                prices.Select(vp => vp.Price));
        }

        private static ICollection<string> ValidateOrderBook(OrderBookMessage orderBookMessage)
        {
            var errors = new List<string>();

            if (orderBookMessage == null)
            {
                errors.Add("Argument 'Order' is null.");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(orderBookMessage.AssetPair))
                {
                    errors.Add("'AssetPair' is empty");
                }
                if ((orderBookMessage.Timestamp == DateTime.MinValue || orderBookMessage.Timestamp == DateTime.MaxValue))
                {
                    errors.Add($"Invalid 'Timestamp' range: '{ (object)orderBookMessage.Timestamp}'");
                }
                if (orderBookMessage.Timestamp.Kind != DateTimeKind.Utc)
                {
                    errors.Add($"Invalid 'Timestamp' Kind (UTC is required): '{ (object)orderBookMessage.Timestamp}'");
                }
                if (orderBookMessage.Prices == null)
                {
                    errors.Add("Invalid 'Prices': null");
                }
                else if (orderBookMessage.Prices.All(p => p.Price < 0) && orderBookMessage.Prices.Any())
                {
                    var prices = orderBookMessage.Prices.Limit(10).Select(p => p.Price.ToString(CultureInfo.InvariantCulture));

                    errors.Add($"All prices is negative. Top 10: [{string.Join(", ", prices)}]");
                }
            }

            return errors;
        }
    }
}
