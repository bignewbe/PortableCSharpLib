using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace PortableCSharpLib.TechnicalAnalysis
{
    [DataContract]
    public class QuoteBasic : IQuoteBasic
    {
        static QuoteBasic() { PortableCSharpLib.General.CheckDateTime(); }

        public int Count { get { return Time.Count; } }
        public long FirstTime { get { return Time.FirstOrDefault(); } }
        public long LastTime { get { return Time.LastOrDefault(); } }

        [DataMember]
        public string Symbol { get; private set; }
        [DataMember]
        public int Interval { get; private set; }                //interval between two data point
        [DataMember]
        public List<long> Time { get; private set; }            //timestamp is expressed in unix _time
        [DataMember]
        public List<double> Open { get; private set; }
        [DataMember]
        public List<double> High { get; private set; }
        [DataMember]
        public List<double> Low { get; private set; }
        [DataMember]
        public List<double> Close { get; private set; }
        [DataMember]
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

        [JsonConstructor]
        public QuoteBasic(String Symbol, int Interval, IList<long> Time, IList<double> Open, IList<double> Close, IList<double> High, IList<double> Low, IList<double> Volume)
        {
            this.Symbol = Symbol;
            this.Interval = Interval;
            this.Time = new List<long>(Time);
            this.Open = new List<double>(Open);
            this.Close = new List<double>(Close);
            this.High = new List<double>(High);
            this.Low = new List<double>(Low);
            this.Volume = new List<double>(Volume);
        }

        public QuoteBasic(IQuoteBasic q)
        {
            Symbol = q.Symbol;
            Interval = q.Interval;
            Time = new List<long>(q.Time);
            Open = new List<double>(q.Open);
            Close = new List<double>(q.Close);
            High = new List<double>(q.High);
            Low = new List<double>(q.Low);
            Volume = new List<double>(q.Volume);
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
