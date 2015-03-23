using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shared.Contracts;
using System.Threading;

namespace Server2
{
    public class QuoteService : Shared.IQuoteService
    {
        public QuoteResponse ProcessQuote(QuoteRequest request)
        {
            Console.WriteLine(string.Format("Processing quote for: {0}, product: {1}", request.Name, request.Product));

            var response = new QuoteResponse()
                               {
                                   Company = "Application 2",
                                   Request = request
                               };

            switch (request.Product)
            {
                case 1:
                    Thread.Sleep(new TimeSpan(0, 0, 0, 30));
                    response.Price = decimal.Zero;
                    break;
                case 3:
                    throw new ApplicationException("We do not support product 3");
                    break;
                default:
                    response.Price = decimal.MaxValue;
                    break;
            }

            return response;
        }
    }
}
