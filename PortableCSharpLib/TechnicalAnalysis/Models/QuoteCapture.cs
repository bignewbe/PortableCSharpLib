using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace PortableCSharpLib.TechnicalAnalysis
{
    /// <summary>
    /// class used for store quotes captured from screen
    /// </summary>
    [DataContract]
    public class QuoteCapture : IQuoteCapture
    {
        public int Count { get { return Time?.Count ?? 0; } }
        public long FirstTime { get { return (Time == null || Time.Count <= 0) ? 0 : Time.FirstOrDefault(); } }
        public long LastTime { get { return (Time == null || Time.Count <= 0) ? 0 : Time.LastOrDefault(); } }

        [DataMember]
        public string Symbol { get; private set; }
        [DataMember]
        public double PipFactor { get; private set; }
        [DataMember]
        public List<double> Price { get; private set; }
        [DataMember]
        public List<long> Time { get; private set; }

        public QuoteCapture()
        {
            Symbol = null;
            PipFactor = 1;
            Time = new List<long>();
            Price = new List<double>();
        }
        public QuoteCapture(string symbol) : this()
        {
            Symbol = symbol;
        }
        public QuoteCapture(IQuoteCapture q)
        {
            Create(q);
        }

        [JsonConstructor]
        public QuoteCapture(string Symbol, double PipFactor, IList<long> Time, IList<double> Price)
        {
            this.Create(Symbol, PipFactor, Time, Price);
        }

        public event EventHandlers.BasicQuoteDataAddedEventHandler DataAdded;

        private void Create(IQuoteCapture q)
        {
            if (q != null)
                this.Create(q.Symbol, q.PipFactor, q.Time, q.Price);
        }

        private void Create(string symbol, double pipFactor, IList<long> time, IList<double> price)
        {
            this.Symbol = symbol;
            this.PipFactor = pipFactor;
            this.Time = new List<long>(time);
            this.Price = new List<double>(price);
        }

        public void Assign(IQuoteCapture q)
        {
            this.Create(q);
        }
        public void Add(long t, double p)
        {
            Time.Add(t);
            Price.Add(p);
            DataAdded?.Invoke(this, Symbol, t, p);
        }
        public virtual void Add(long time, double buyPrice, double sellPrice)
        {
            this.Add(time, (buyPrice + sellPrice) / 2.0);
        }
        public virtual void Append(IQuoteCapture q)
        {
            if (q == null || q.Count <= 0 || this.Symbol != q.Symbol)
                return;

            var sindex = q.Count - 1;
            while (sindex >= 0 && q.Time[sindex] > this.LastTime) --sindex;

            if (sindex == -1) {
                this.Time.AddRange(q.Time);
                this.Price.AddRange(q.Price);
            }
            else if (sindex < q.Count - 1) {
                ++sindex;
                this.Time.AddRange(q.Time.GetRange(sindex, q.Count - sindex));
                this.Price.AddRange(q.Price.GetRange(sindex, q.Count - sindex));
            }
        }
        public virtual IQuoteCapture Extract(long stime, long etime)
        {
            int sIndex = General.BinarySearch(Time, 0, Time.Count - 1, stime, true);

            if (sIndex < 0) {
                sIndex++;
                if (Time.Count == 0 || Time[sIndex] < stime) {
                    return null;
                }
            }
            else if (Time[sIndex] != stime) {
                sIndex++;
                if (sIndex >= Time.Count - 1) {
                    return null;
                }
            }

            int eIndex = General.BinarySearch(Time, 0, Time.Count - 1, etime, true);

            return Extract(sIndex, eIndex);
        }
        public virtual IQuoteCapture Extract(int sindex, int eindex)
        {
            if (sindex < 0 || eindex == -1 || eindex > this.Count - 1 || eindex < sindex)
                return null;
            int num = eindex - sindex + 1;
            var quote = new QuoteCapture(Symbol);
            quote.Time.AddRange(Time.GetRange(sindex, num));
            quote.Price.AddRange(Price.GetRange(sindex, num));
            return quote;
        }
    }
}
