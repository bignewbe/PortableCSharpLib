using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortableCSharpLib.TechnicalAnalysis
{
    public class QuoteBasic : IQuoteBasic
    {
        public int Count { get { return _time.Count; } }
        public long FirstTime { get { return _time.FirstOrDefault(); } }
        public long LastTime { get { return _time.LastOrDefault(); } }

        public string Symbol { get; set; }
        public int Interval { get; set; }                //interval between two data point
        public List<long> _time { get; set; }            //timestamp is expressed in unix _time
        public List<double> _open { get; set; }
        public List<double> _high { get; set; }
        public List<double> _low { get; set; }
        public List<double> _close { get; set; }
        public List<double> _volume { get; set; }

        public QuoteBasic(string symbol, int interval)
        {
            Symbol = symbol;
            Interval = interval;
            _time = new List<long>();
            _open = new List<double>();
            _close = new List<double>();
            _high = new List<double>();
            _low = new List<double>();
            _volume = new List<double>();
        }

        public void Add(long t, double o, double h, double l, double c, double v, bool isTriggerDataAdded = false)
        {
            _open.Add(o);
            _high.Add(h);
            _low.Add(l);
            _close.Add(c);
            _volume.Add(v);
            _time.Add(t);
        }
    }
}
