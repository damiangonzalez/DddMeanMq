using System.Threading;
using RabbitMQ.Client;
using System;
using System.ComponentModel;
using System.Text;

namespace RabbitMQPubSubNode
{
    public class RabbitMqPubSubNode
    {
        private readonly string _nodeType;
        private readonly IModel _channel;
        private BackgroundWorker _bgwRabbitReceiveWorker;
        private readonly Action<string> _callbackActionFromClient;
        private string p;
        private Action<string> action;
        private readonly DomainEventHandler _domainEventHandler;

        public RabbitMqPubSubNode(string nodeType, Action<string> callbackAction, bool doesPubOnly = false)
        {
            _nodeType = nodeType;
            // assign action to be used by background worker for returning data back to client
            _callbackActionFromClient = callbackAction;

            // create domain event handler
            _domainEventHandler = new DomainEventHandler();

            // establish connection and model
            var factory = new ConnectionFactory { HostName = "localhost" };
            IConnection connection = factory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.ExchangeDeclare("logs", "fanout");

            // A moment for Rabbit to establish queue etc.
            Thread.Sleep(2000);

            if (doesPubOnly == false)
            {
                // kick off background worker for receive to known exchange
                LaunchBackgroundWorkerForReceiveHandling();
            }
        }

        public void Publish(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish("logs", "", null, body);
        }

        public void LaunchBackgroundWorkerForReceiveHandling()
        {
            _bgwRabbitReceiveWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };
            _bgwRabbitReceiveWorker.DoWork += _bgwLoadClients_DoWork;
            _bgwRabbitReceiveWorker.ProgressChanged += _bgwLoadClients_ProgressChanged;
            _bgwRabbitReceiveWorker.RunWorkerAsync(null);
        }

        private void _bgwLoadClients_DoWork(object sender, DoWorkEventArgs e)
        {
            var queueName = _channel.QueueDeclare().QueueName;

            _channel.QueueBind(queueName, "logs", "");
            var consumer = new QueueingBasicConsumer(_channel);
            _channel.BasicConsume(queueName, true, consumer);

            Console.WriteLine(" [*] Waiting for logs." +
                              "To exit press CTRL+C");
            while (true)
            {
                try
                {
                    var ea = consumer.Queue.Dequeue();

                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);

                    _bgwRabbitReceiveWorker.ReportProgress(
                        0, 
                        string.Format("Message received by {0}: {1}", _nodeType, message));

                    _bgwRabbitReceiveWorker.ReportProgress(
                        0,
                        string.Format("Message handling result from {0}: {1}", _nodeType, _domainEventHandler.HandleDomainEvent(_nodeType, message)));
                }
                catch (Exception ex)
                {
                    _bgwRabbitReceiveWorker.ReportProgress(0, "Exception: " + ex.Message);
                    break;
                }
            }
        }

        void _bgwLoadClients_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {            
            var userState = e.UserState;
            _callbackActionFromClient(userState.ToString());
        }
    }
}
