using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortableCSharpLib.TechnicalAnalysis
{
    /// <summary>
    /// interface for basic quote info
    /// </summary>
    public interface IQuoteBasic
    {
        int Count { get; }
        string Symbol { get; }
        int Interval { get; }
        long FirstTime { get; }
        long LastTime { get; }

        List<long> Time { get; }
        List<double> Open { get; }
        List<double> Close { get; }
        List<double> Low { get; }
        List<double> High { get; }
        List<double> Volume { get; }

        void Add(long t, double o, double h, double l, double c, double v, bool isTriggerDataAdded);
    }
}
