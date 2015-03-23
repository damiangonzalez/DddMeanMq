using System;
using System.Collections.Generic;
using System.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System.Text;

namespace Shared
{
    public class RabbitSender 
    {
        private const string HostName = "localhost";
        private const string UserName = "guest";
        private const string Password = "guest";
        private const string QueueName = "Module1.Sample3";
        private const string ExchangeName = "";
        private const bool IsDurable = true;
        private IModel model;

        public RabbitSender()
        {
            DisplaySettings();
            SetupRabbitMq();
        }

        private void SetupRabbitMq()
        {
            var connectionfactory = new RabbitMQ.Client.ConnectionFactory()
            {
                Password = Password,
                UserName = UserName,
                HostName = HostName
            };

            var connection = connectionfactory.CreateConnection();
            model = connection.CreateModel();
            Shared.Program.ConfigureQueues(model);
        }

        private void DisplaySettings()
        {
            // throw new NotImplementedException();
        }

        public void Send(string message)
        {
            var properties = model.CreateBasicProperties();
            properties.SetPersistent(true);
            // Serialize
            byte[] messageBuffer = Encoding.Default.GetBytes(message);
            // Send message
            model.BasicPublish(ExchangeName, QueueName, properties, messageBuffer);
            Console.WriteLine("Message sent...");
        }
    }
}

