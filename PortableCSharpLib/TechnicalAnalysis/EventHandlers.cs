using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortableCSharpLib.TechnicalAnalysis
{
    public class EventHandlers
    {
        public delegate void DynamicEventHandler(object sender, dynamic parameter);
        public delegate void BasicQuoteDataAddedEventHandler(object sender, string symbol, long time, double price);
        public delegate void QuoteBasicDataAddedEventHandler(object sender, string symbol, long time, double open, double close, double high, double low, double volume);
        public delegate void QuoteBasicDataAppendedEventHandler(object sender, IQuoteBasic quote, int numAppended);
    }
}
