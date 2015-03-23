using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared.Contracts
{
    public class QuoteResponse
    {
        public QuoteRequest Request { get; set; }
        public string Company { get; set; }
        public decimal Price { get; set; }
    }
}
