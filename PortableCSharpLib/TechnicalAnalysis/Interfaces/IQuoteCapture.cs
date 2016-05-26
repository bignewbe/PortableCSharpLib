using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortableCSharpLib.TechnicalAnalysis
{
    public interface IQuoteCapture
    {
        int Count { get; }
        long FirstTime { get; }
        long LastTime { get; }
        string Symbol { get; }
        List<long> Time { get; }
        List<double> Price { get; }
        double PipFactor { get; }
        void Assign(IQuoteCapture q);
        void Append(IQuoteCapture q);
        void Add(long time, double price);
        IQuoteCapture Extract(int sindex, int eindex);
        IQuoteCapture Extract(long stime, long etime);

        /// <summary>
        /// Event when new data is added
        /// </summary>
        event EventHandlers.BasicQuoteDataAddedEventHandler DataAdded;
    }
}
