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

        List<long> _time { get; }
        List<double> _open { get; }
        List<double> _close { get; }
        List<double> _low { get; }
        List<double> _high { get; }
        List<double> _volume { get; }

        void Add(long t, double o, double h, double l, double c, double v, bool isTriggerDataAdded);
    }
}
