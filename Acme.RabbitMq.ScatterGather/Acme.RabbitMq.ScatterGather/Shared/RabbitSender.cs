using System;
using System.Collections.Generic;
using System.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;

namespace Shared
{
    public class RabbitSender<TRequest, TResponse> : RabbitConnector, IDisposable
    {
        private readonly string _responseQueue;
        private readonly QueueingBasicConsumer _consumer;
        private readonly string _exchangeName;

        /// <summary>
        /// Ctor 
        /// </summary>
        /// <param name="exchangeName">The name of the exchange to use for sending messages</param>
        /// <param name="settings">The connection settings for the rabbit server</param>
        public RabbitSender(RabbitConnectionSettings settings, string exchangeName)
        {
            _exchangeName = exchangeName;
            base._connectionSettings = settings;
            base.ConfigureConnection();

            //Configure for pattern
            _model.ExchangeDeclare(_exchangeName, ExchangeType.Topic, true); //Setup exchange and configure it for topic pattern
            _responseQueue = _model.QueueDeclare().QueueName; //Declare a random queue for the response
            _consumer = new QueueingBasicConsumer(_model);
            _model.BasicConsume(_responseQueue, true, _consumer);
        }
        
        /// <summary>
        /// Sends a message to the queue defined in the configuration
        /// </summary>
        public IEnumerable<TResponse> Send(TRequest message, List<string> routingKeys, TimeSpan timeout, int minExpectedResponses)
        {
            var responses = new List<TResponse>();
            var correlationToken = Guid.NewGuid().ToString();

            //Setup properties
            var properties = _model.CreateBasicProperties();
            properties.ReplyTo = _responseQueue;
            properties.CorrelationId = correlationToken;

            //Serialize
            byte[] messageBuffer = Shared.Serializer.Serialize<TRequest>(message);

            var timeoutAt = DateTime.Now + timeout;
            var routingKey = routingKeys.Aggregate(string.Empty, (current, key) => current + (key.ToLower() + "."));
            if (routingKey.Length > 1)
                routingKey = routingKey.Remove(routingKey.Length - 1, 1);

            _model.BasicPublish(_exchangeName, routingKey, properties, messageBuffer);

            while(DateTime.Now <= timeoutAt)
            {
                object result = null;
                _consumer.Queue.Dequeue(10, out result);
                if (result == null)
                {
                    //No more messages on queue at present so if we have already got the minimum expected responses then
                    //lets just return those
                    if (responses.Count >= minExpectedResponses)
                        return responses;

                    continue;
                }

                var deliveryArgs = (BasicDeliverEventArgs) result;
                if (deliveryArgs.BasicProperties == null ||
                    deliveryArgs.BasicProperties.CorrelationId != correlationToken) continue;

                var response = Shared.Serializer.Deserialize<TResponse>(deliveryArgs.Body);

                responses.Add(response);
            }
            return responses;
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Close();
            }
            if (_model != null && _model.IsOpen)
            {
                _model.Abort();
                
            }
           
            _connectionFactory = null;
        }
    }
}

