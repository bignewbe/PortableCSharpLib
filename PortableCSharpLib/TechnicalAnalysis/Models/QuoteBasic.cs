using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortableCSharpLib.TechnicalAnalysis
{
    public class QuoteBasic : IQuoteBasic
    {
        public int Count { get { return Time.Count; } }
        public long FirstTime { get { return Time.FirstOrDefault(); } }
        public long LastTime { get { return Time.LastOrDefault(); } }

        public string Symbol { get; private set; }
        public int Interval { get; private set; }                //interval between two data point
        public List<long> Time { get; private set; }            //timestamp is expressed in unix _time
        public List<double> Open { get; private set; }
        public List<double> High { get; private set; }
        public List<double> Low { get; private set; }
        public List<double> Close { get; private set; }
        public List<double> Volume { get; private set; }

        public QuoteBasic(string symbol, int interval)
        {
            Symbol = symbol;
            Interval = interval;
            Time = new List<long>();
            Open = new List<double>();
            Close = new List<double>();
            High = new List<double>();
            Low = new List<double>();
            Volume = new List<double>();
        }

        public void Add(long t, double o, double h, double l, double c, double v, bool isTriggerDataAdded = false)
        {
            Open.Add(o);
            High.Add(h);
            Low.Add(l);
            Close.Add(c);
            Volume.Add(v);
            Time.Add(t);
        }
    }
}
