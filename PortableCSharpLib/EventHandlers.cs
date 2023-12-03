using PortableCSharpLib.Interface;
using PortableCSharpLib.TechnicalAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortableCSharpLib
{
    public class EventHandlers
    {
        public delegate void DynamicEventHandler(object sender, dynamic parameter);
        public delegate void ItemWithIdChangedEventHandler<T>(object sender, string id, T item);
        public delegate void ItemChangedEventHandler<T>(object sender, T item);
        //public delegate void ItemListChangedEventHandler<T>(object sender, IList<T> items);

        public delegate void DataRemovedEventHandler(object sender, IQuoteBasicBase quote);
        public delegate void DataAddedOrUpdatedEventHandler(object sender, IQuoteBasicBase quote, int numAppended);
        public delegate void QuoteBasicDataAddedEventHandler(object sender, string symbol, int interval, long time, double open, double close, double high, double low, double volume);
        public delegate void QuoteCaptureDataAddedEventHandler(object sender, string symbol, long time, double price, double volume);

        public delegate void QuoteBasicDataAddedOrUpdatedEventHandler(object sender, string exchange, IQuoteBasicBase quote, int numAppended);
        public delegate void QuoteSavedEventHandler(object sender, string exchange, string filename);
        public delegate void QuoteCaptureDataAddedOrUpdatedEventHandler(object sender, string exchange, IQuoteCapture quote, int numAppended);
    }
}
