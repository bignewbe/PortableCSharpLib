using PortableCSharpLib;
using PortableCSharpLib.TechnicalAnalysis;
using System.Collections.Concurrent;

namespace PortableCSharpLib.Interface
{
    public interface IQuoteCaptureMemStore
    {
        string Exchange { get; }
        int MaxNumTicks { get; }
        ConcurrentDictionary<string, IQuoteCapture> QuoteCaptures { get; }
        void Add(string symbol, long timestamp, double price, double volume);
        IQuoteCapture GetInMemoryQuoteCapture(string symbol);
        event EventHandlers.QuoteCaptureDataAddedOrUpdatedEventHandler OnQuoteCaptureDataAdded;
    }
}