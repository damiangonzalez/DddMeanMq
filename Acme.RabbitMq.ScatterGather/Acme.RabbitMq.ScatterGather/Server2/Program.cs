﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Quote Service - Application 2");

            var rabbitConnectionSettings = new Shared.RabbitConnectionSettings
            {
                HostName = "localhost",
                Password = "guest",
                Username = "guest"
            };
            const string quoteExchangeName = "HealthInsuranceQuoteExchange";

            var subscriptions = new List<string> { "#.1.#", "#.2.#", "#.4.#", "#.5.#", "#.6.#" };

            var consumers = new List<Shared.RabbitReceiver>();
            for (var index = 0; index <= 10; index++)
            {
                var rabbitServer = new Shared.RabbitReceiver(rabbitConnectionSettings, index.ToString(), quoteExchangeName,
                    "QuoteApplication2ProcessingQueue", subscriptions, new QuoteService());

                Console.WriteLine("Starting instance: " + index);
                consumers.Add(rabbitServer);
            }

            Parallel.ForEach(consumers, x => x.Start());

            Console.ReadLine();
        }
    }
}
