using System.Text;
using System.Threading;
using RabbitMQ.Client;

namespace RabbitMQPubSubNode
{
    public class RabbitMqPubNode
    {
        protected readonly string NodeType;
        protected readonly IModel Channel;

        public RabbitMqPubNode(string nodeType)
        {
            NodeType = nodeType;

            // establish connection and model
            var factory = new ConnectionFactory
            {
                HostName = "10.71.76.200",
                UserName = "remoteuser",
                Password = "password",
                VirtualHost = "/",
                Protocol = Protocols.DefaultProtocol,
                Port = AmqpTcpEndpoint.UseDefaultPort
            }; // localhost
            
            IConnection connection = factory.CreateConnection();
            Channel = connection.CreateModel();
            Channel.ExchangeDeclare("logs", "fanout");

            // A moment for Rabbit to establish queue etc.
            Thread.Sleep(2000);
        }

        public void Publish(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            Channel.BasicPublish("logs", "", null, body);
        }
    }
}
