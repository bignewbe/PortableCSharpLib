using PortableCSharpLib.Interface;
using PortableCSharpLib;
using PortableCSharpLib.DataType;
using PortableCSharpLib.TechnicalAnalysis;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PortableCSharpLib.Model
{
    /// <summary>
    /// simple store for quote basic
    /// 1. add 
    /// </summary>
    public class QuoteBasicMemStore : IQuoteBasicMemStore
    {
        string GetQuoteId(string symbol, int interval)
        {
            return string.Format("{0}_{1}", symbol, interval);
        }

        public int MaxNumCandles { get; private set; }
        public List<int> Intervals { get; }
        public ConcurrentDictionary<string, QuoteBasicBase> Quotes { get; private set; } = new ConcurrentDictionary<string, QuoteBasicBase>();

        public string Exchange { get; private set; }
        public event EventHandlers.QuoteBasicDataAddedOrUpdatedEventHandler OnQuoteBasicDataAddedOrUpdated;

        public QuoteBasicMemStore(string exchange, int numBars, List<int> intervals)
        {
            MaxNumCandles = numBars;
            this.Intervals = new List<int>(intervals);
            this.Exchange = exchange;
        }

        private string CreateQuote(string symbol, int interval)
        {
            lock (this)
            {
                var quoteId = this.GetQuoteId(symbol, interval);
                if (!Quotes.ContainsKey(quoteId))
                {
                    var q = new QuoteBasicBase(symbol, interval);
                    Quotes.TryAdd(quoteId, q as QuoteBasicBase);
                    Quotes[quoteId].OnDataAddedOrUpdated += QuoteStore_OnDataAddedOrUpdated;   //trigger add to other intervals
                }
                return quoteId;
            }
        }

        public void ClearQuote(string symbol)
        {
            lock (this)
            {
                foreach (var interval in Intervals)
                {
                    var quoteId = this.GetQuoteId(symbol, interval);
                    if (Quotes.ContainsKey(quoteId))
                        Quotes.TryRemove(quoteId, out _);
                }
            }
        }

        //public void AddCandle(List<OHLC> ohlc, bool isAddToAll, bool isTriggerEvent, bool isAddWithGap = true)
        //{
        //    if (ohlc == null || ohlc.Count == 0) return;
        //    var q = new QuoteBasicBase(ohlc);
        //    this.AddQuoteBasic(q, isAddToAll, isTriggerEvent, isAddWithGap);
        //}

        public void AddCandle(string symbol, int interval, long time, double open, double close, double high, double low, double volume, 
            bool isAddToAll, bool isTriggerEvent, bool isAddWithGap = true)
        {
            lock (this)
            {
                if (Intervals.Contains(interval))
                {
                    if (isAddToAll)
                    {
                        var index = this.Intervals.FindIndex(i => i == interval);
                        var quoteId = this.CreateQuote(symbol, interval);
                        var num = Quotes[quoteId].AddUpdate(time, open, high, low, close, volume, isTriggerEvent, isAddWithGap);      //trigger dataadded event
                        if (num >= 0)  //data has been added or updated
                            this.AddToQuotesHierarch(Quotes[quoteId], index + 1, isTriggerEvent, isAddWithGap);
                    }
                    else
                    {
                        var quoteId = this.CreateQuote(symbol, interval);
                        Quotes[quoteId].AddUpdate(time, open, high, low, close, volume, true, isAddWithGap);
                    }
                }
            }
        }

        //public void AddQuoteCapture(IQuoteCapture qc, bool isTriggerEvent)
        //{
        //    lock (this)
        //    {
        //        var interval = this.Intervals[0];
        //        var quoteId = this.CreateQuote(qc.Symbol, interval);
        //        var num = Quotes[quoteId].Append(qc, false);
        //        if (num >= 0)
        //        {
        //            var d = this.AddToOtherQuotes(Quotes[quoteId], 1, isTriggerEvent);
        //            //QuoteStore_OnDataAddedOrUpdated(this, this.Quotes[quoteId], num);
        //            //foreach (var id in d.Keys) QuoteStore_OnDataAddedOrUpdated(this, this.Quotes[id], d[id]);
        //        }
        //    }
        //}

        public void AddQuoteBasic(IQuoteBasicBase qb, bool isAddToAll, bool isTriggerEvent, bool isAddWithGap = true)
        {
            lock (this)
            {
                if (qb != null && qb.Count > 0 && Intervals.Contains(qb.Interval))
                {
                    if (isAddToAll)
                    {
                        var index = this.Intervals.FindIndex(i => i == qb.Interval);
                        var d = this.AddToQuotesHierarch(qb, index, isTriggerEvent, isAddWithGap);

                        //var interval = this.Intervals[index];
                        //var quoteId = this.CreateQuote(qb.Symbol, interval);
                        //var num = Quotes[quoteId].Append(qb, isTriggerEvent, isAddWithGap);                                                         //trigger OnQuoteBasicDataAddedOrUpdated
                        ////var num = Quotes[quoteId].Append(qb, false);                                                      //do not trigger OnQuoteBasicDataAddedOrUpdated until we add to all quotes
                        //if (num >= 0)
                        //{
                        //    var d = this.AddToOtherQuotes(Quotes[quoteId], index + 1, isTriggerEvent, isAddWithGap);
                        //    //QuoteStore_OnDataAddedOrUpdated(this, this.Quotes[quoteId], num);
                        //    //foreach (var qId in d.Keys) QuoteStore_OnDataAddedOrUpdated(this, this.Quotes[qId], d[qId]);
                        //}
                    }
                    else
                    {
                        var quoteId = this.CreateQuote(qb.Symbol, qb.Interval);
                        var num = Quotes[quoteId].Append(qb, isTriggerEvent, isAddWithGap);
                    }
                }
            }
        }

        private Dictionary<string, int> AddToQuotesHierarch(IQuoteBasicBase prevQuote, int sindex, bool isTriggerEvent, bool isAddWithGap)
        {
            var addedNumber = new Dictionary<string, int>();
            var originQuoteId = prevQuote.QuoteID;

            //append quote to subsequent quote one by one => hieararchically
            for (int i = sindex; i < this.Intervals.Count; i++)
            {
                var interv = this.Intervals[i];
                if (interv < prevQuote.Interval || interv % prevQuote.Interval != 0) continue;

                var quoteId = this.CreateQuote(prevQuote.Symbol, interv);
                var num = Quotes[quoteId].Append(prevQuote, isTriggerEvent, isAddWithGap);                             
                if (num == -1) break;   //no data is updated or added => break;

                addedNumber.Add(quoteId, num);
                prevQuote = Quotes[quoteId];
            }
            return addedNumber;
        }

        private Dictionary<string, int> AddToQuotesOneByOne(IQuoteBasicBase prevQuote, int sindex, bool isTriggerEvent, bool isAddWithGap)
        {
            var addedNumber = new Dictionary<string, int>();
            var q = prevQuote;
            var interval = q.Interval;
            for (int i = sindex; i < this.Intervals.Count; i++)
            {
                var interv = this.Intervals[i];
                if (interv <= interval || interv % interval != 0) continue;

                var quoteId = this.CreateQuote(q.Symbol, interv);
                var num = Quotes[quoteId].Append(q, isTriggerEvent, isAddWithGap);
                if (num >= 0)
                {
                    if (!addedNumber.ContainsKey(quoteId)) addedNumber.Add(quoteId, 0);
                    addedNumber[quoteId] += num;
                }
            }
            return addedNumber;
        }

        public QuoteBasicBase GetQuoteBasic(string symbol, int interval)
        {
            var quoteId = this.GetQuoteId(symbol, interval);
            return Quotes.ContainsKey(quoteId)? Quotes[quoteId] : null;
        }
        
        void QuoteStore_OnDataAddedOrUpdated(object sender, IQuoteBasicBase qb, int numAppended)
        {
            OnQuoteBasicDataAddedOrUpdated?.Invoke(sender, this.Exchange, qb, numAppended);
            if (qb.Count > MaxNumCandles * 2) qb.Clear(0, qb.Count - MaxNumCandles - 1);
        }
    }
}
