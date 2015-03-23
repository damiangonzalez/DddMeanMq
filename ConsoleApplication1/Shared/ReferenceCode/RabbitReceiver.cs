using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Shared
{
    public class RabbitReceiver : RabbitConnector, IDisposable
    {
        public bool Enabled { set; get; }
        public string InstanceName { set; get; }
        public string ProcessingQueue { set; get; }
        public string Exchange { set; get; }
        public List<string> Subscriptions { set; get; }
        // public IQuoteService QuoteService { get; set; }

        /// <summary>
        /// Ctor with a key to lookup the configuration
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="instanceName"></param>
        /// <param name="exchangeName"></param>
        /// <param name="processingQueueName"></param>
        /// <param name="subscriptions"></param>
        public RabbitReceiver(RabbitConnectionSettings settings, string instanceName, 
            string exchangeName, string processingQueueName, 
            List<string> subscriptions)
        {
            base._connectionSettings = settings;
            InstanceName = instanceName;
            ProcessingQueue = processingQueueName;
            Exchange = exchangeName;
            Subscriptions = subscriptions;
            //QuoteService = quoteService;
            base.ConfigureConnection();

            Enabled = true;

            //Setup Pattern
            _model.BasicQos(0, 1, false);
            _model.ExchangeDeclare(exchangeName, ExchangeType.Topic, true);
            _model.QueueDeclare(processingQueueName, false, false, false, null);

            if (subscriptions == null) return;
            foreach(var subscription in subscriptions)
            {
                _model.QueueBind(processingQueueName, exchangeName, subscription.ToLower());
            }
        }
        /// <summary>
        /// Starts receiving a message from a queue
        /// </summary>
        public void Start()
        {
            var consumer = new ConsumeDelegate(Poll);
            consumer.Invoke();
        }

        private delegate void ConsumeDelegate();

        private void Poll()
        {
            var consumer = new QueueingBasicConsumer(_model);
            _model.BasicConsume(ProcessingQueue, false, consumer);

            while (Enabled)
            {
                //Get next message
                var deliveryArgs = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                //Deserialize message
               // var request = Serializer.Deserialize<Contracts.QuoteRequest>(deliveryArgs.Body);

                //Process Message
                //var response = QuoteService.ProcessQuote(request);

                //Send Response
                var replyProperties = _model.CreateBasicProperties();
                replyProperties.CorrelationId = deliveryArgs.BasicProperties.CorrelationId;
                byte[] messageBuffer = null; //Serializer.Serialize<Contracts.QuoteResponse>(response);

                _model.BasicPublish("", deliveryArgs.BasicProperties.ReplyTo, replyProperties, messageBuffer);


                //Acknowledge message is processed
                _model.BasicAck(deliveryArgs.DeliveryTag, false);
            }
        }
        

        public void Dispose()
        {
            if(_model != null)
                _model.Dispose();
            if(_connection != null)
                _connection.Dispose();

            _connectionFactory = null;
        }
    }
}
