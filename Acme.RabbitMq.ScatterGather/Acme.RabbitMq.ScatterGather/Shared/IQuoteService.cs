using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared
{
    public interface IQuoteService
    {
        Contracts.QuoteResponse ProcessQuote(Contracts.QuoteRequest request);
    }
}
