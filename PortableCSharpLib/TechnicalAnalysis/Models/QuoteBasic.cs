using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
        public string QuoteID { get { return string.Format("{0}_{1}", Symbol, Interval); } }

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

        public event EventHandlers.QuoteBasicDataAddedEventHandler QuoteBasicDataAdded;

        public void Add(long t, double o, double h, double l, double c, double v, bool isTriggerDataAdded = false)
        {
            if (this.Count == 0 || t > this.LastTime) {
                Open.Add(o);
                High.Add(h);
                Low.Add(l);
                Close.Add(c);
                Volume.Add(v);
                Time.Add(t);
                if (isTriggerDataAdded) {
                    var tmp = QuoteBasicDataAdded;
                    tmp?.Invoke(this, Symbol, t, o, c, h, l, v);
                }
            }
        }
        public int Append(IQuoteBasic q, int interval = -1)
        {
            var numAddedElement = 0;

            //no data to add
            if (q == null || q.Count <= 0 || q.LastTime <= this.LastTime || this.Symbol != q.Symbol) return numAddedElement;

            //get the closest this.Interval in case some incomplete data is added
            var lastTime = this.LastTime / this.Interval * this.Interval;                //note when quote is empty: lastTime == 0 => we always add
            interval = (interval == -1) ? this.Interval : interval;

            //search backward for quotes to be added. the found time should be > lastTime
            int indexStartSearch = -1;
            for (int i = q.Count - 1; i >= 0; i--) {
                if (q.Time[i] <= lastTime)
                    break;

                indexStartSearch = i;
            }
            if (indexStartSearch == -1) return numAddedElement;

            if ((q.LastTime / this.Interval != q.Time[indexStartSearch] / this.Interval) || //at least one element at this.Interval to add
                (q.LastTime % interval == 0))                                               //at least one element at interval to add (last element only)
            {
                //remove last element if it is not multiplier of this.Interval. it will be executed only when we are adding sub-interval data
                var bRemoveLastElement = (this.Count > 0 && this.Interval != interval &&
                    this.LastTime % this.Interval != 0 && q.Time[indexStartSearch] <= this.LastTime && q.LastTime > this.LastTime);

                if (bRemoveLastElement) {
                    Time.RemoveAt(Time.Count - 1);
                    Open.RemoveAt(Open.Count - 1);
                    High.RemoveAt(High.Count - 1);
                    Low.RemoveAt(Low.Count - 1);
                    Close.RemoveAt(Close.Count - 1);
                    Volume.RemoveAt(Volume.Count - 1);
                }

                /////////////////////////////////////////////////////////////
                //var isNewElementAdded = false;
                int sindex = indexStartSearch;
                int len = 0;

                for (int i = indexStartSearch; i <= q.Count - 1; i++) {
                    ++len;
                    //if ((q._time[i] % this.Interval == 0) ||                                                       //exactly at Interval
                    //    (numAddedElement > 0 && q._time[i] / this.Interval != q._time[i - 1] / this.Interval))     //span over boundary; 1st element we never consider span; this might cause difference for mearged quote.                                                                                                                                               
                    if (q.Time[i] / this.Interval != this.LastTime / this.Interval) {
                        var eindex = sindex + len - 1;
                        var time = q.Time[i] / this.Interval * this.Interval;
                        var open = q.Open[sindex];
                        var close = q.Close[eindex];
                        var volume = q.Volume[eindex];
                        var high = q.High.GetRange(sindex, len).Max();
                        var low = q.Low.GetRange(sindex, len).Min();

                        ////repeat quote to fill gap
                        //if (this.Count > 0) {
                        //    var c = this._close.LastOrDefault();
                        //    var v = this._volume.LastOrDefault();
                        //    while (time > this.LastTime + this.Interval)
                        //        this.Add(this.LastTime + this.Interval, c, c, c, c, v);
                        //}

                        this.Add(time, open, high, low, close, volume);       //add new element
                        ++numAddedElement;

                        //current element should be excluded from next group if it is at interval
                        if (q.Time[i] % this.Interval == 0) {
                            len = 0;
                            sindex = i + 1;
                        }
                        else {
                            len = 1;
                            sindex = i;
                        }
                    }
                }

                //add last element even if they are not multiple of this.Interval but multiples of interval
                //note that sindex is always at the beginning element for new group
                if (sindex <= q.Count - 1 && interval < this.Interval && q.LastTime % interval == 0) {
                    len = q.Count - sindex;
                    var time = q.Time.LastOrDefault();
                    var open = q.Open[sindex];
                    var close = q.Close.LastOrDefault();
                    var high = q.High.GetRange(sindex, len).Max();
                    var low = q.Low.GetRange(sindex, len).Min();

                    this.Add(time, open, high, low, close, 0);  //add new element
                    ++numAddedElement;
                }
            }

            return numAddedElement;
        }
        public int Append(IQuoteCapture q, int interval = -1)
        {
            var numAddedElement = 0;

            if (q == null || q.Count <= 0 || q.LastTime <= this.LastTime || this.Symbol != q.Symbol)
                return numAddedElement;

            var lastTime = this.LastTime / this.Interval * this.Interval;
            interval = (interval == -1) ? this.Interval : interval;

            //search backward for the quotes. the found time should be >= lastTime
            int indexStartSearch = -1;
            for (int i = q.Count - 1; i >= 0; i--) {
                if (q.Time[i] < lastTime)               //for basic quote we include price at previous interval for calculation
                    break;

                indexStartSearch = i;
            }
            if (indexStartSearch == -1) return numAddedElement;

            if ((q.LastTime / this.Interval != q.Time[indexStartSearch] / this.Interval) || //jump over boundary
                (q.LastTime % interval == 0))                                               //at boundary 
            {
                //remove last element if it is not multiplier of this.Interval. it will be executed only when we are adding sub-interval data
                var bRemoveLastElement = (this.Count > 0 && this.Interval != interval && this.LastTime % this.Interval != 0 && q.Time[indexStartSearch] <= this.LastTime && q.LastTime > this.LastTime);
                if (bRemoveLastElement) {
                    Time.RemoveAt(Time.Count - 1);
                    Open.RemoveAt(Open.Count - 1);
                    High.RemoveAt(High.Count - 1);
                    Low.RemoveAt(Low.Count - 1);
                    Close.RemoveAt(Close.Count - 1);
                    Volume.RemoveAt(Volume.Count - 1);
                }

                //////////////////////////////////////////////////////
                int sindex = indexStartSearch;
                int len = 1;
                for (int i = indexStartSearch + 1; i <= q.Count - 1; i++)                         //we have to start from indexStartSearch + 1 since sindex may at this.Interval
                {
                    ++len;
                    if ((q.Time[i] % this.Interval == 0) ||                                       //new element started 
                        (i > 0 && q.Time[i] / this.Interval != q.Time[i - 1] / this.Interval))    //jump over bounary                  
                    {
                        var eindex = sindex + len - 1;
                        var price = q.Price.GetRange(sindex, len);
                        var time = q.Time[i] / this.Interval * this.Interval;
                        var open = q.Price[sindex];
                        var close = q.Price[eindex];
                        var high = price.Max();
                        var low = price.Min();

                        //repeat quote to fill gap
                        if (this.Count > 0) {
                            while (time > this.LastTime + this.Interval) {
                                this.Add(this.LastTime + this.Interval,
                                         this.Open.LastOrDefault(), this.High.LastOrDefault(),
                                         this.Low.LastOrDefault(), this.Close.LastOrDefault(), this.Volume.LastOrDefault());
                            }
                        }

                        this.Add(time, open, high, low, close, 0);  //add the data
                        ++numAddedElement;

                        //include current element to the next group
                        len = 1;
                        sindex = i;
                    }
                }

                //add last element even if they are not multiple of this.Interval but multiples of interval
                //note that sindex is always at the element for new group
                if (sindex <= q.Count - 1 && this.Interval != interval && q.LastTime % interval == 0) {
                    len = q.Count - sindex;
                    var eindex = sindex + len - 1;
                    var price = q.Price.GetRange(sindex, len);
                    var time = q.LastTime;
                    var open = q.Price[sindex];
                    var close = q.Price[eindex];
                    var high = price.Max();
                    var low = price.Min();
                    this.Add(time, open, high, low, close, 0);  //add the data
                    ++numAddedElement;
                }
            }

            return numAddedElement;
        }
        
        //return index whose timestamp >= time
        public int FindIndexForGivenTime(long time, bool isReturnJustSmallerElement = false)
        {
            if (this.Count <= 0) return -1;
            return General.BinarySearch(Time, 0, Time.Count - 1, time, isReturnJustSmallerElement);
        }

        public int FindIndexWhereTimeLocated(long time)
        {
            if (this.Count <= 0)
                return -1;

            var index = General.BinarySearch(Time, 0, Time.Count - 1, time, true);

            if (index == -1)
                return index;

            if (Time[index] != time)  //if give time is not in the list, BinarySearch returns the index of the element which is just smaller than given time                                           
                ++index;              //in this case, we should increment index

            if (index >= this.Count)  //out of range
                index = -1;

            return index;
        }
        public IQuoteBasic Extract(long stime, long etime)
        {
            int sindex = 0, eindex = this.Count - 1;            //by default we extrace whole data

            if (stime > this.FirstTime)
                sindex = this.FindIndexWhereTimeLocated(stime);

            if (etime < this.LastTime)
                eindex = this.FindIndexWhereTimeLocated(etime);

            return Extract(sindex, eindex);
        }
        public IQuoteBasic Extract(int sindex, int eindex)
        {
            if (sindex < 0 || eindex == -1 || eindex > this.Count - 1 || eindex < sindex) return null;

            int num = eindex - sindex + 1;
            var quote = new QuoteBasic(Symbol, Interval);
            quote.Time.AddRange(Time.GetRange(sindex, num));
            quote.Open.AddRange(Open.GetRange(sindex, num));
            quote.High.AddRange(High.GetRange(sindex, num));
            quote.Low.AddRange(Low.GetRange(sindex, num));
            quote.Close.AddRange(Close.GetRange(sindex, num));
            quote.Volume.AddRange(Volume.GetRange(sindex, num));
            return quote;
        }

        //insert missing quote
        public bool Insert(IQuoteBasic q)
        {
            if (q == null || q.Count <= 0) return false;
            if (this.Symbol != q.Symbol || this.Interval != q.Interval) return false;
            if (this.Count <= 0 || q.FirstTime > this.LastTime) {
                this.Time.AddRange(q.Time);
                this.Open.AddRange(q.Open);
                this.High.AddRange(q.High);
                this.Low.AddRange(q.Low);
                this.Close.AddRange(q.Close);
                this.Volume.AddRange(q.Volume);
                return true;
            }

            if (q.LastTime < this.FirstTime) {
                this.Time.InsertRange(0, q.Time);
                this.Open.InsertRange(0, q.Open);
                this.High.InsertRange(0, q.High);
                this.Low.InsertRange(0, q.Low);
                this.Close.InsertRange(0, q.Close);
                this.Volume.InsertRange(0, q.Volume);
                return true;
            }

            var sindex = this.FindIndexForGivenTime(q.FirstTime, true);
            var eindex = this.FindIndexForGivenTime(q.LastTime, true);
            if (sindex == -1 || eindex == -1) return false;

            if (this.Time[eindex] != q.LastTime)          
                eindex += 1;                              //points to elment just larger than last time

            //we know Time[sindex] <= q.FirstTime and Time[eindex] >= q.LastTime => we can merge the two sequences
            var i = sindex;
            var j = 0;
            var times = new List<long>();
            var open = new List<double>();
            var close = new List<double>();
            var high = new List<double>();
            var low = new List<double>();
            var volume = new List<double>();

            while (i <= eindex || j <= q.Count - 1) {
                if (j > q.Count - 1) {   //take all from i
                    times.AddRange(this.Time.GetRange(i, eindex - i + 1));
                    open.AddRange(this.Open.GetRange(i, eindex - i + 1));
                    close.AddRange(this.Close.GetRange(i, eindex - i + 1));
                    high.AddRange(this.High.GetRange(i, eindex - i + 1));
                    low.AddRange(this.Low.GetRange(i, eindex - i + 1));
                    volume.AddRange(this.Volume.GetRange(i, eindex - i + 1));
                    break;
                }
                if (i > eindex) {        //take all from j
                    times.AddRange(q.Time.GetRange(j, q.Count - j));
                    open.AddRange(q.Open.GetRange(j, q.Count - j));
                    close.AddRange(q.Close.GetRange(j, q.Count - j));
                    high.AddRange(q.High.GetRange(j, q.Count - j));
                    low.AddRange(q.Low.GetRange(j, q.Count - j));
                    volume.AddRange(q.Volume.GetRange(j, q.Count - j));
                    break;
                }
                while (i <= eindex && j < q.Count && this.Time[i] == q.Time[j]) {
                    times.Add(this.Time[i]);
                    open.Add(this.Open[i]);
                    close.Add(this.Close[i]);
                    high.Add(this.High[i]);
                    low.Add(this.Low[i]);
                    volume.Add(this.Volume[i]);
                    ++i;
                    ++j;
                }
                while (i <= eindex && this.Time[i] < q.Time[j]) {
                    times.Add(this.Time[i]);
                    open.Add(this.Open[i]);
                    close.Add(this.Close[i]);
                    high.Add(this.High[i]);
                    low.Add(this.Low[i]);
                    volume.Add(this.Volume[i]);
                    ++i;
                }
                while (j < q.Count && q.Time[j] < this.Time[i]) {
                    times.Add(q.Time[j]);
                    open.Add(q.Open[j]);
                    close.Add(q.Close[j]);
                    high.Add(q.High[j]);
                    low.Add(q.Low[j]);
                    volume.Add(q.Volume[j]);
                    ++j;
                }
            }

            //remove old data
            this.Time.RemoveRange(sindex, eindex - sindex + 1);
            this.Open.RemoveRange(sindex, eindex - sindex + 1);
            this.Close.RemoveRange(sindex, eindex - sindex + 1);
            this.High.RemoveRange(sindex, eindex - sindex + 1);
            this.Low.RemoveRange(sindex, eindex - sindex + 1);
            this.Volume.RemoveRange(sindex, eindex - sindex + 1);

            //insert new data
            var insertIndex = Math.Max(0, sindex - 1);
            this.Time.InsertRange(insertIndex, times);
            this.Open.InsertRange(insertIndex, open);
            this.Close.InsertRange(insertIndex, close);
            this.High.InsertRange(insertIndex, high);
            this.Low.InsertRange(insertIndex, low);
            this.Volume.InsertRange(insertIndex, volume);

            return true;
            ////we are sure elements are missing
            //if (eindex - sindex + 1 < q.Count) {
            //    var times = this.Time.GetRange(sindex, eindex - sindex + 1);
            //    var insertIndex = sindex + 1;
            //    for (int i = 0; i < q.Count; i++) {
            //        if (times.Contains(q.Time[i])) continue;
            //        this.Time.Insert(insertIndex, q.Time[i]);
            //        this.Open.Insert(insertIndex, q.Open[i]);
            //        this.High.Insert(insertIndex, q.High[i]);
            //        this.Low.Insert(insertIndex, q.Low[i]);
            //        this.Close.Insert(insertIndex, q.Close[i]);
            //        this.Volume.Insert(insertIndex, q.Volume[i]);
            //        ++insertIndex;
            //    }
            //}
        }

        public void Clear()
        {
            this.Time.Clear();
            this.Open.Clear();
            this.Close.Clear();
            this.High.Clear();
            this.Low.Clear();
            this.Volume.Clear();
        }
    }
}
