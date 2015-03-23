using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace Shared
{
    public class Program
    {
        private const string HostName = "localhost";
        private const string UserName = "guest";
        private const string Password = "guest";
        private const string QueueName = "Module2.Sample1";
        private const string ExchangeName = "";

        static void Main(string[] args)
        {

        }

        public static void ConfigureQueues(IModel model)
        {
            // Note configuring queues is idempotent (it will skip if queue exists)
            // Configuring queues
            model.QueueDeclare("Module1.Sample3", true, false, false, null);
            // model.QueueDeclare("MyQueue2", true, false, false, null);
            // Console.WriteLine("Queue Created");
            // model.ExchangeDeclare("MyExchange2", ExchangeType.Topic);
            // Console.WriteLine("Exchange Created");
            // model.QueueBind("MyQueue2", "MyExchange2", "cars");
            // Console.WriteLine("Exchange and Queue bound");
        }
    }
}
