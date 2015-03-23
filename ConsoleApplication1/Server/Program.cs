using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Server
{
    class Program
    {       
        static void Main(string[] args)
        {
            Console.WriteLine("Starting RabbitMQ Queue Processor");
            Console.WriteLine();
            Console.WriteLine();

            SimpleReceiveMessage();
        }

        static void SimpleReceiveMessage() 
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

            using (var connection = connectionfactory.CreateConnection())
            {
                using (var model = connection.CreateModel())
                {
                    Shared.Program.ConfigureQueues(model);

                    var consumer = new QueueingBasicConsumer(model);
                    model.BasicConsume("Module1.Sample3", true, consumer);

                    Console.WriteLine(" [*] Waiting for messages." +
                                             "To exit press CTRL+C");
                    while (true)
                    {
                        var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] Received {0}", message);
                    }
                }
            }
        }
    }
}
