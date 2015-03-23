using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shared.Contracts;
using System.Threading;

namespace Server3
{
    public class QuoteService : Shared.IQuoteService
    {
        public QuoteResponse ProcessQuote(QuoteRequest request)
        {
            Console.WriteLine(string.Format("Processing quote for: {0}, product: {1}", request.Name, request.Product));

            var response = new QuoteResponse()
                               {
                                   Company = "Application 3",
                                   Request = request
                               };

            switch (request.Product)
            {
                case 6:
                    Thread.Sleep(new TimeSpan(0, 0, 0, 30));
                    response.Price = decimal.Zero;
                    break;
                default:
                    response.Price = decimal.MaxValue;
                    break;
            }

            return response;
        }
    }
}
