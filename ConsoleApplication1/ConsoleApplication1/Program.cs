using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using Shared;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting RabbitMQ Message Sender");
            Console.WriteLine();
            Console.WriteLine();
            var messageCount = 0;
            var sender = new RabbitSender();
            Console.WriteLine("Press enter key to send a message");
            while (true)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Q)
                    break;
                if (key.Key == ConsoleKey.Enter)
                {
                    var message = string.Format("Message: {0}", messageCount);
                    Console.WriteLine(string.Format("Sending — {0}", message));
                    sender.Send(message);
                    messageCount++;
                }
            }

            Console.ReadLine();
        }

        static void SimpleSendMessage()
        {
            const string HostName = "localhost";
            const string UserName = "guest";
            const string Password = "guest";
            const string QueueName = "Module1.Sample3";
            const string ExchangeName = "";

            var connectionfactory = new RabbitMQ.Client.ConnectionFactory()
            {
                Password = Password,
                UserName = UserName,
                HostName = HostName
            };

            var connection = connectionfactory.CreateConnection();
            var model = connection.CreateModel();
            Shared.Program.ConfigureQueues(model);
            var properties = model.CreateBasicProperties();
            properties.SetPersistent(true);

            // Serialize
            byte[] messageBuffer = Encoding.Default.GetBytes("This is my persistent message");
            // Send message
            model.BasicPublish(ExchangeName, QueueName, properties, messageBuffer);
            Console.WriteLine("Message sent...");
        }
    }
}
