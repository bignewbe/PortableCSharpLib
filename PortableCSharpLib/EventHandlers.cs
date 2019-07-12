using PortableCSharpLib.Interace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortableCSharpLib.TechnicalAnalysis
{
    public class EventHandlers
    {
        public delegate void DynamicEventHandler(object sender, dynamic parameter);
        public delegate void DataRemovedEventHandler(object sender, IQuoteBasicBase quote);
        public delegate void DataAddedOrUpdatedEventHandler(object sender, IQuoteBasicBase quote, int numAppended);
        public delegate void QuoteBasicDataAddedEventHandler(object sender, string symbol, int interval, long time, double open, double close, double high, double low, double volume);
        public delegate void QuoteCaptureDataAddedEventHandler(object sender, string symbol, long time, double price, double volume);
    }
}
