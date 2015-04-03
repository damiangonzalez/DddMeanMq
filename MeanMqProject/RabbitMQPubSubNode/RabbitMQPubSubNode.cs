using System;
using System.ComponentModel;
using System.Text;
using RabbitMQ.Client;

namespace RabbitMQPubSubNode
{
    public class RabbitMqPubSubNode : RabbitMqPubNode
    {
        private BackgroundWorker _bgwRabbitReceiveWorker;
        private readonly Action<string> _callbackActionFromClient;
        private string p;
        private Action<string> _action;
        private readonly DomainEventHandler _domainEventHandler;

        public RabbitMqPubSubNode(string nodeType, Action<string> callbackAction) :
            base(nodeType)
        {
            // assign action to be used by background worker for returning data back to client
            _callbackActionFromClient = callbackAction;

            // create domain event handler
            _domainEventHandler = new DomainEventHandler();

            // kick off background worker for receive to known exchange
            LaunchBackgroundWorkerForReceiveHandling();
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
            var queueName = Channel.QueueDeclare().QueueName;

            Channel.QueueBind(queueName, "logs", "");
            var consumer = new QueueingBasicConsumer(Channel);
            Channel.BasicConsume(queueName, true, consumer);

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
                        string.Format("Message received by {0}: {1}", NodeType, message));

                    _bgwRabbitReceiveWorker.ReportProgress(
                        0,
                        string.Format("Message handling result from {0}: {1}", NodeType, _domainEventHandler.HandleDomainEvent(NodeType, message)));
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
