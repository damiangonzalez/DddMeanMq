using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared.Contracts
{
    public class QuoteRequest
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public bool IsSmoker { get; set; }
        public int Product { get; set; }
        public int TimeoutSeconds { get; set; }
        public int MinimumResponses { get; set; }
    }
}
