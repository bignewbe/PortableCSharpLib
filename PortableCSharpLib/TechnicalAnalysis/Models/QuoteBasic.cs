using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace PortableCSharpLib.TechnicalAnalysis
{
    [DataContract]
    public class QuoteBasic : IQuoteBasic
    {
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

        /// <summary>
        /// Time等List复制了参数的内容，而不是引用相同的对象。
        /// </summary>
        /// <param name="Symbol"></param>
        /// <param name="Interval"></param>
        /// <param name="Time"></param>
        /// <param name="Open"></param>
        /// <param name="Close"></param>
        /// <param name="High"></param>
        /// <param name="Low"></param>
        /// <param name="Volume"></param>
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

        /// <summary>
        /// 复制IQuoteBasic的内容
        /// </summary>
        /// <param name="q"></param>
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
        public event Action<IQuoteBasic, int> DataChanged;

        public void Add(long t, double o, double h, double l, double c, double v, bool isTriggerDataAdded = false)
        {
            if (this.Count == 0 || t > this.LastTime)
            {
                Open.Add(o);
                High.Add(h);
                Low.Add(l);
                Close.Add(c);
                Volume.Add(v);
                Time.Add(t);
                if (isTriggerDataAdded)
                {
                    var tmp = QuoteBasicDataAdded;
                    tmp?.Invoke(this, Symbol, Interval, t, o, c, h, l, v);
                }
            }
        }

        public void UpdateLast(long t, double o, double h, double l, double c, double v)
        {
            if (this.Count == 0 || this.LastTime != t)
                return;
            Open[this.Count - 1] = o;
            High[this.Count - 1] = h;
            Low[this.Count - 1] = l;
            Close[this.Count - 1] = c;
            Volume[this.Count - 1] = v;
            //Time.Add(t);
        }

        public int Append(IQuoteBasic q, int interval = -1, bool isFillGap = false, bool isTriggerDataUpdated = false)
        {
            var numAddedElement = 0;

            //no data to add
            if (q == null || q.Count <= 0 || q.LastTime <= this.LastTime || this.Symbol != q.Symbol) return numAddedElement;

            //get the closest this.Interval in case some incomplete data is added
            var lastTime = this.LastTime / this.Interval * this.Interval;                //note when quote is empty: lastTime == 0 => we always add
            interval = (interval == -1) ? this.Interval : interval;

            //search backward for quotes to be added. the found time should be > this.LastTime
            int indexStartSearch = -1;
            for (int i = q.Count - 1; i >= 0; i--)
            {
                if (q.Time[i] <= this.LastTime)
                    break;

                indexStartSearch = i;
            }
            if (indexStartSearch == -1) return numAddedElement;

            // 判断是否有数据要被添加
            if (!(q.LastTime / interval != q.Time[indexStartSearch] / interval ||  // 不在同一区间
                    q.LastTime % interval == 0 ||   // 排除在边界的情况
                    q.Time[indexStartSearch] % interval == 0)) //  // 排除在边界的情况
                return numAddedElement;

            bool isDataChanged = false; // 是否改变了数据
            #region 最后一个不完整数据
            if (this.LastTime % this.Interval != 0) // 最后一个为不完整数据
            {
                //while (q.Time[indexStartSearch] <= this.LastTime)
                //    indexStartSearch++;
                //if (indexStartSearch >= q.Count)
                //    return numAddedElement;
                var nextTime = lastTime + this.Interval;
                if (q.Time[indexStartSearch] > nextTime) // 进入下一个单元中
                {
                    Time[Time.Count() - 1] = nextTime;
                }
                else
                {
                    int endIndex = indexStartSearch;
                    while (endIndex < q.Count && q.Time[endIndex] <= nextTime) ++endIndex;
                    endIndex--;

                    var open = this.Open.Last();
                    var low = Math.Min(Low.Last(), q.Low.GetRange(indexStartSearch, endIndex - indexStartSearch + 1).Min());
                    var high = Math.Max(High.Last(), q.High.GetRange(indexStartSearch, endIndex - indexStartSearch + 1).Max());
                    var close = q.Close[endIndex];
                    var volume = q.Volume.GetRange(indexStartSearch, endIndex - indexStartSearch + 1).Sum() + this.Volume.Last();
                    long time = q.LastTime < nextTime ? q.LastTime : nextTime;

                    Time.RemoveAt(Time.Count - 1);
                    Open.RemoveAt(Open.Count - 1);
                    High.RemoveAt(High.Count - 1);
                    Low.RemoveAt(Low.Count - 1);
                    Close.RemoveAt(Close.Count - 1);
                    Volume.RemoveAt(Volume.Count - 1);

                    this.Time.Add(time);
                    this.Open.Add(open);
                    this.Close.Add(close);
                    this.High.Add(high);
                    this.Low.Add(low);
                    this.Volume.Add(volume);
                    indexStartSearch = endIndex + 1;
                    isDataChanged = true;
                }
            }
            #endregion

            if (indexStartSearch >= q.Count)
                return numAddedElement;

            int sindex = indexStartSearch; // interval区间的开始索引
            var endTime = (q.Time[sindex] / this.Interval + ((q.Time[sindex] % this.Interval != 0) ? 1 : 0)) * this.Interval;
            for (int i = indexStartSearch; i <= q.Count - 2; i++)
            {
                if (q.Time[i + 1] > endTime)
                {
                    var eindex = i; // interval区间的结束索引
                    this.AddItemByQuoteBasic(q, isFillGap, sindex, eindex, endTime, ref numAddedElement);

                    sindex = i + 1;
                    endTime = (q.Time[sindex] / this.Interval + ((q.Time[sindex] % this.Interval != 0) ? 1 : 0)) * this.Interval;
                }
            }

            if (q.LastTime % this.Interval == 0)
            {
                var eindex = q.Count - 1;
                this.AddItemByQuoteBasic(q, isFillGap, sindex, eindex, endTime, ref numAddedElement);
            }
            else
                FillGap(isFillGap, endTime, ref numAddedElement);

            //add last element even if they are not multiple of this.Interval but multiples of interval
            //note that sindex is always at the beginning element for new group
            if (sindex <= q.Count - 1 && interval != this.Interval && q.LastTime % interval == 0)
            {
                var len = q.Count - sindex;
                var time = q.Time.LastOrDefault();
                var open = q.Open[sindex];
                var close = q.Close.LastOrDefault();
                var high = q.High.GetRange(sindex, len).Max();
                var low = q.Low.GetRange(sindex, len).Min();
                var volume = q.Volume.GetRange(sindex, len).Sum();

                if (this.Count == 0 || time > this.LastTime)
                    ++numAddedElement;
                this.Add(time, open, high, low, close, volume);  //add new element
            }

            if (isTriggerDataUpdated && (isDataChanged || numAddedElement > 0))
            {
                var tmp = DataChanged;
                tmp?.Invoke(this, numAddedElement);
            }

            return numAddedElement;
        }

        private void AddItemByQuoteBasic(IQuoteBasic q, bool isFillGap, int sindex, int eindex, long endTime, ref int numAddedElement)
        {
            var len = eindex - sindex + 1;
            var open = q.Open[sindex];
            var close = q.Close[eindex];
            var high = q.High.GetRange(sindex, len).Max();
            var low = q.Low.GetRange(sindex, len).Min();
            var volume = q.Volume.GetRange(sindex, len).Sum();
            ++numAddedElement;

            //repeat quote to fill gap
            FillGap(isFillGap, endTime, ref numAddedElement);

            this.Add(endTime, open, high, low, close, volume);  //add the data
        }

        public int Append(IQuoteCapture qc, int interval = -1, bool isFillGap = false, bool isTriggerDataUpdated = false)
        {
            var numAddedElement = 0;

            if (qc == null || qc.Count <= 0 || qc.LastTime <= this.LastTime || this.Symbol != qc.Symbol)
                return numAddedElement;

            var lastTime = this.LastTime / this.Interval * this.Interval;
            interval = (interval == -1) ? this.Interval : interval;

            //search backward for the quotes. the found time should be > lastTime
            int indexStartSearch = -1;
            for (int i = qc.Count - 1; i >= 0; i--)
            {
                if (qc.Time[i] <= this.LastTime)               //for basic quote we include price at previous interval for calculation
                    break;

                indexStartSearch = i;
            }
            if (indexStartSearch == -1) return numAddedElement;

            if (!(qc.LastTime / interval != qc.Time[indexStartSearch] / interval ||
                qc.LastTime % interval == 0 ||
                qc.Time[indexStartSearch] % interval == 0))
                return numAddedElement;

            bool isDataChanged = false;
            // now 一定有数据被添加
            #region 被Append的QuoteBasic最后一个不完整数据
            if (this.LastTime % this.Interval != 0) // 最后一个为不完整数据
            {
                while (qc.Time[indexStartSearch] <= this.LastTime)
                    indexStartSearch++;
                if (indexStartSearch >= qc.Count)
                    return numAddedElement;
                var nextTime = lastTime + this.Interval;
                if (qc.Time[indexStartSearch] > nextTime) // 进入下一个单元中
                {
                    Time[Time.Count() - 1] = nextTime;
                }
                else
                {
                    int endIndex = indexStartSearch;
                    while (endIndex < qc.Count && qc.Time[endIndex] <= nextTime) ++endIndex;
                    endIndex--;

                    var open = this.Open.Last();
                    var low = Math.Min(Low.Last(), qc.Price.GetRange(indexStartSearch, endIndex - indexStartSearch + 1).Min());
                    var high = Math.Max(High.Last(), qc.Price.GetRange(indexStartSearch, endIndex - indexStartSearch + 1).Max());
                    var close = qc.Price[endIndex];
                    long time = qc.LastTime < nextTime ? qc.LastTime : nextTime;
                    double volume = this.Volume.Last() + qc.Volume.GetRange(indexStartSearch, endIndex - indexStartSearch + 1).Sum();

                    Time.RemoveAt(Time.Count - 1);
                    Open.RemoveAt(Open.Count - 1);
                    High.RemoveAt(High.Count - 1);
                    Low.RemoveAt(Low.Count - 1);
                    Close.RemoveAt(Close.Count - 1);
                    Volume.RemoveAt(Volume.Count - 1);

                    this.Time.Add(time);
                    this.Open.Add(open);
                    this.Close.Add(close);
                    this.High.Add(high);
                    this.Low.Add(low);
                    this.Volume.Add(volume);
                    indexStartSearch = endIndex + 1;
                    isDataChanged = true;
                }
            }
            #endregion

            if (indexStartSearch >= qc.Count)
            {
                if (isDataChanged)
                    OnDataUpdated(numAddedElement);
                return numAddedElement;
            }

            int sindex = indexStartSearch; // interval区间的开始索引
            var endTime = (qc.Time[sindex] / this.Interval + ((qc.Time[sindex] % this.Interval != 0) ? 1 : 0)) * this.Interval;
            for (int i = indexStartSearch; i <= qc.Count - 2; i++)
            {
                if (qc.Time[i + 1] > endTime)
                {
                    var eindex = i; // interval区间的结束索引
                    this.AddItemByQuoteCapture(qc, isFillGap, sindex, eindex, endTime, ref numAddedElement);

                    sindex = i + 1;
                    endTime = (qc.Time[sindex] / this.Interval + ((qc.Time[sindex] % this.Interval != 0) ? 1 : 0)) * this.Interval;
                }
            }

            if (qc.LastTime % this.Interval == 0)
            {
                var eindex = qc.Count - 1;
                this.AddItemByQuoteCapture(qc, isFillGap, sindex, eindex, endTime, ref numAddedElement);
            }
            else
                FillGap(isFillGap, endTime, ref numAddedElement);
            // add last element even if they are not multiple of this.Interval but multiples of interval
            //note that sindex is always at the element for new group
            if (sindex <= qc.Count - 1 && this.Interval != interval && qc.LastTime % interval == 0)
            {
                var len = qc.Count - sindex;
                var eindex = sindex + len - 1;
                var price = qc.Price.GetRange(sindex, len);
                var time = qc.LastTime;
                var open = this.Count > 0 ? this.Close.Last() : qc.Price[sindex];
                var close = qc.Price[eindex];
                var high = price.Max();
                var low = price.Min();
                var volume = qc.Volume.GetRange(sindex, len).Sum();

                if (this.Count == 0 || time > this.LastTime)
                    ++numAddedElement;
                this.Add(time, open, high, low, close, volume);  //add the data
            }

            if (isTriggerDataUpdated && (isDataChanged || numAddedElement > 0))
            {
                OnDataUpdated(numAddedElement);
            }

            return numAddedElement;
        }

        private void OnDataUpdated(int numAddedElement)
        {
            var tmp = DataChanged;
            tmp?.Invoke(this, numAddedElement);
        }

        private void AddItemByQuoteCapture(IQuoteCapture q, bool isFillGap, int sindex, int eindex, long endTime, ref int numAddedElement)
        {
            var len = eindex - sindex + 1;
            var price = q.Price.GetRange(sindex, len);
            var volumnList = q.Volume.GetRange(sindex, len);
            var time = endTime;
            var open = this.Count > 0 ? this.Close.Last() : q.Price[sindex];
            var close = q.Price[eindex];
            var high = price.Max();
            var low = price.Min();
            var volumn = volumnList.Sum();
            ++numAddedElement;
            //repeat quote to fill gap
            FillGap(isFillGap, endTime, ref numAddedElement);
            this.Add(endTime, open, high, low, close, volumn);  //add the data
        }

        private void FillGap(bool isFillGap, long endTime, ref int numAddedElement)
        {
            if (this.Count > 0 && isFillGap)
            {
                while (endTime > this.LastTime + this.Interval)
                {
                    this.Add(this.LastTime + this.Interval,
                             this.Open.LastOrDefault(), this.High.LastOrDefault(),
                             this.Low.LastOrDefault(), this.Close.LastOrDefault(), 0);
                    ++numAddedElement;
                }
            }
        }

        //return index whose timestamp <= time
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

            if (index == -1 || Time[index] == time)
                return index;

            if (index < this.Count - 1 &&
                Time[index + 1] == (time / this.Interval + 1) * this.Interval)    //if give time is not in the list, BinarySearch returns the index of the element which is just smaller than given time                                           
                ++index;                                                          //in this case, we should increment index
            else
                index = -1;

            return index;
        }


        public IQuoteBasic Extract(long stime, long etime)
        {
            var q = new QuoteBasic(this.Symbol, this.Interval);
            if (this.Count == 0 || stime > etime || stime > this.LastTime || etime < this.FirstTime)
                return q;
            int sindex = 0, eindex = this.Count - 1;            //by default we extrace whole data

            if (stime > this.FirstTime)
            {
                sindex = this.FindIndexForGivenTime(stime, true);
                if (this.Time[sindex] != stime)
                    ++sindex;
            }

            if (sindex >= this.Count)
                return q;

            if (etime < this.LastTime)
                eindex = this.FindIndexForGivenTime(etime, true); // <= etime

            if (sindex > eindex)
                return q;

            return Extract(sindex, eindex);
        }
        public IQuoteBasic Extract(int sindex, int eindex)
        {
            if (sindex < 0 || eindex > this.Count - 1 || eindex < sindex)
                throw new ArgumentException(string.Format("Function {0} sindex: {1}, eindex: {2}, Count: {3}",
                    "Extract", sindex, eindex, this.Count));

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
            if (this.Count <= 0 || q.FirstTime > this.LastTime)
            {
                this.Time.AddRange(q.Time);
                this.Open.AddRange(q.Open);
                this.High.AddRange(q.High);
                this.Low.AddRange(q.Low);
                this.Close.AddRange(q.Close);
                this.Volume.AddRange(q.Volume);
                return true;
            }

            if (q.LastTime < this.FirstTime)
            {
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

            if (this.Time[eindex] != q.LastTime && eindex < this.Count - 1)
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

            while (i <= eindex || j <= q.Count - 1)
            {
                if (j > q.Count - 1)
                {   //take all from i
                    times.AddRange(this.Time.GetRange(i, eindex - i + 1));
                    open.AddRange(this.Open.GetRange(i, eindex - i + 1));
                    close.AddRange(this.Close.GetRange(i, eindex - i + 1));
                    high.AddRange(this.High.GetRange(i, eindex - i + 1));
                    low.AddRange(this.Low.GetRange(i, eindex - i + 1));
                    volume.AddRange(this.Volume.GetRange(i, eindex - i + 1));
                    break;
                }
                if (i > eindex)
                {        //take all from j
                    times.AddRange(q.Time.GetRange(j, q.Count - j));
                    open.AddRange(q.Open.GetRange(j, q.Count - j));
                    close.AddRange(q.Close.GetRange(j, q.Count - j));
                    high.AddRange(q.High.GetRange(j, q.Count - j));
                    low.AddRange(q.Low.GetRange(j, q.Count - j));
                    volume.AddRange(q.Volume.GetRange(j, q.Count - j));
                    break;
                }
                while (i <= eindex && j < q.Count && this.Time[i] == q.Time[j])
                {
                    times.Add(this.Time[i]);
                    open.Add(this.Open[i]);
                    close.Add(this.Close[i]);
                    high.Add(this.High[i]);
                    low.Add(this.Low[i]);
                    volume.Add(this.Volume[i]);
                    ++i;
                    ++j;
                }
                while (i <= eindex && this.Time[i] < q.Time[j])
                {
                    times.Add(this.Time[i]);
                    open.Add(this.Open[i]);
                    close.Add(this.Close[i]);
                    high.Add(this.High[i]);
                    low.Add(this.Low[i]);
                    volume.Add(this.Volume[i]);
                    ++i;
                }
                while (j < q.Count && q.Time[j] < this.Time[i])
                {
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

        public void Clear(long stime, long etime)
        {
            if (this.Count == 0 || stime > etime || stime > this.LastTime || etime < this.FirstTime)
                return;
            int sindex = 0, eindex = this.Count - 1;            //by default we extrace whole data

            if (stime > this.FirstTime)
            {
                sindex = this.FindIndexForGivenTime(stime, true);
                if (this.Time[sindex] != stime)
                    ++sindex;
            }

            if (sindex >= this.Count)
                return;

            if (etime < this.LastTime)
                eindex = this.FindIndexForGivenTime(etime, true); // <= etime

            if (sindex > eindex)
                return;

            Clear(sindex, eindex);
        }

        public void Clear(int sindex, int eindex)
        {
            if (sindex < 0 || eindex > this.Count - 1 || eindex < sindex)
                throw new ArgumentException(string.Format("Function {0} sindex: {1}, eindex: {2}, Count: {3}",
                    "Extract", sindex, eindex, this.Count));

            int num = eindex - sindex + 1;
            this.Time.RemoveRange(sindex, num);
            this.High.RemoveRange(sindex, num);
            this.Low.RemoveRange(sindex, num);
            this.Open.RemoveRange(sindex, num);
            this.Close.RemoveRange(sindex, num);
            this.Volume.RemoveRange(sindex, num);
        }

        public void AppendStream(Stream stream)
        {
            if (this.Count <= 0 || stream == null)
                return;

            var sr = new StreamWriter(stream);

            if (stream.Position == 0)
            {
                sr.WriteLine("Symbol:{0};Interval:{1}", this.Symbol, this.Interval);
                sr.WriteLine("Time;Close;Open;High;Low;Volume;");
            }

            for (int i = 0; i < this.Time.Count; i++)
                sr.WriteLine("{0};{1};{2};{3};{4};{5}", this.Time[i], this.Close[i], this.Open[i],
                    this.High[i], this.Low[i], this.Volume[i]);

            sr.Flush();
        }

        /// <summary>
        /// 从stream中读取数据，并实例化QuoteBasic
        /// </summary>
        /// <param name="stream">stream</param>
        /// <returns></returns>
        public static QuoteBasic InitByStream(Stream stream)
        {
            if (stream == null)
                return null;
            var qb = new QuoteBasic("", 1);
            qb.LoadStream(stream, true);
            return qb;
        }

        public void LoadStream(Stream stream)
        {
            LoadStream(stream, false);
        }

        private void LoadStream(Stream stream, bool isOverwrite)
        {
            if (stream == null)
            {
                return;
            }

            var lines = new List<string>();
            var reader = new StreamReader(stream);
            string str = null;
            while ((str = reader.ReadLine()) != null)
            {
                lines.Add(str);
            }

            if (lines.Count < 2)
                throw new FileFormatNotSupportedException(string.Format("Lines.Count: {0}", lines.Count));

            try
            {
                var i = 0;
                var isHead1Found = false;
                var isHead2Found = false;
                for (i = 0; i < lines.Count; i++)
                {
                    if (string.IsNullOrEmpty(lines[i])) continue;
                    var ln = Regex.Replace(lines[i], @"\s+", "");

                    if (!isHead1Found)
                    {
                        var symbol = ln.Split(';')[0].Split(':')[1];
                        var interval = int.Parse(ln.Split(';')[1].Split(':')[1]);
                        if (isOverwrite)
                        {
                            this.Symbol = symbol;
                            this.Interval = interval;
                        }
                        else if (Symbol != symbol || Interval != interval)
                            throw new FileFormatNotSupportedException(string.Format("Symbol or interval not equals"));
                        isHead1Found = !isHead1Found;
                    }
                    else if (!isHead2Found)
                    {
                        if (ln != "Time;Close;Open;High;Low;Volume;")
                            throw new FileFormatNotSupportedException(string.Format("All Lines: {0}, Error Line: {1}", lines, lines[i]));
                        isHead2Found = !isHead2Found;
                    }

                    if (isHead1Found && isHead2Found)
                        break;
                }

                if (!isHead1Found || !isHead2Found)
                    throw new FileFormatNotSupportedException(string.Format("Head not found"));

                if (isOverwrite)
                    this.Clear();
                for (int j = i + 1; j < lines.Count; j++)
                {
                    var line = lines[j];
                    if (string.IsNullOrEmpty(line)) continue;
                    var ln = Regex.Replace(line, @"\s+", "");
                    var segments = ln.Split(';');
                    //try
                    //{
                    this.Add(long.Parse(segments[0]), ParseDouble(segments[2]), ParseDouble(segments[3]),
                        ParseDouble(segments[4]), ParseDouble(segments[1]), ParseDouble(segments[5]));
                    //}
                    //catch (Exception e)
                    //{

                    //}
                }
                return;
            }
            catch (Exception e)
            {
                this.Clear();
                throw new FileFormatNotSupportedException("file format not supported", e);
            }
        }

        private double ParseDouble(string s)
        {
            try
            {
                return double.Parse(s);
            }
            catch (OverflowException)
            {
                return double.MaxValue;
            }
        }

        /// <summary>
        /// !!!Unsafe!!! don't use it if you don't understand the meaning of it
        /// </summary>
        /// <param name="symbol"></param>
        public void SetSymbol(string symbol)
        {
            this.Symbol = symbol;
        }
    }

    //[DataContract]
    //public class QuoteBasic : IQuoteBasic
    //{
    //    public int Count { get { return Time.Count; } }
    //    public long FirstTime { get { return Time.FirstOrDefault(); } }
    //    public long LastTime { get { return Time.LastOrDefault(); } }
    //    public string QuoteID { get { return string.Format("{0}_{1}", Symbol, Interval); } }

    //    [DataMember]
    //    public string Symbol { get; private set; }
    //    [DataMember]
    //    public int Interval { get; private set; }                //interval between two data point
    //    [DataMember]
    //    public List<long> Time { get; private set; }            //timestamp is expressed in unix _time
    //    [DataMember]
    //    public List<double> Open { get; private set; }
    //    [DataMember]
    //    public List<double> High { get; private set; }
    //    [DataMember]
    //    public List<double> Low { get; private set; }
    //    [DataMember]
    //    public List<double> Close { get; private set; }
    //    [DataMember]
    //    public List<double> Volume { get; private set; }
    //    public QuoteBasic(string symbol, int interval)
    //    {
    //        Symbol = symbol;
    //        Interval = interval;
    //        Time = new List<long>();
    //        Open = new List<double>();
    //        Close = new List<double>();
    //        High = new List<double>();
    //        Low = new List<double>();
    //        Volume = new List<double>();
    //    }

    //    [JsonConstructor]
    //    public QuoteBasic(String Symbol, int Interval, IList<long> Time, IList<double> Open, IList<double> Close, IList<double> High, IList<double> Low, IList<double> Volume)
    //    {
    //        this.Symbol = Symbol;
    //        this.Interval = Interval;
    //        this.Time = new List<long>(Time);
    //        this.Open = new List<double>(Open);
    //        this.Close = new List<double>(Close);
    //        this.High = new List<double>(High);
    //        this.Low = new List<double>(Low);
    //        this.Volume = new List<double>(Volume);
    //    }
    //    public QuoteBasic(IQuoteBasic q)
    //    {
    //        Symbol = q.Symbol;
    //        Interval = q.Interval;
    //        Time = new List<long>(q.Time);
    //        Open = new List<double>(q.Open);
    //        Close = new List<double>(q.Close);
    //        High = new List<double>(q.High);
    //        Low = new List<double>(q.Low);
    //        Volume = new List<double>(q.Volume);
    //    }

    //    public event EventHandlers.QuoteBasicDataAddedEventHandler QuoteBasicDataAdded;

    //    public void Add(long t, double o, double h, double l, double c, double v, bool isTriggerDataAdded = false)
    //    {
    //        if (this.Count == 0 || t > this.LastTime) {
    //            Open.Add(o);
    //            High.Add(h);
    //            Low.Add(l);
    //            Close.Add(c);
    //            Volume.Add(v);
    //            Time.Add(t);
    //            if (isTriggerDataAdded) {
    //                var tmp = QuoteBasicDataAdded;
    //                tmp?.Invoke(this, Symbol, t, o, c, h, l, v);
    //            }
    //        }
    //    }
    //    public int Append(IQuoteBasic q, int interval = -1, bool isFillGap = false)
    //    {
    //        var numAddedElement = 0;

    //        //no data to add
    //        if (q == null || q.Count <= 0 || q.LastTime <= this.LastTime || this.Symbol != q.Symbol) return numAddedElement;

    //        //get the closest this.Interval in case some incomplete data is added
    //        var lastTime = this.LastTime / this.Interval * this.Interval;                //note when quote is empty: lastTime == 0 => we always add
    //        interval = (interval == -1) ? this.Interval : interval;

    //        //search backward for quotes to be added. the found time should be > lastTime
    //        int indexStartSearch = -1;
    //        for (int i = q.Count - 1; i >= 0; i--) {
    //            if (q.Time[i] <= lastTime)
    //                break;

    //            indexStartSearch = i;
    //        }
    //        if (indexStartSearch == -1) return numAddedElement;

    //        if ((q.LastTime / this.Interval != q.Time[indexStartSearch] / this.Interval) || //at least one element at this.Interval to add
    //            (q.LastTime % interval == 0))                                               //at least one element at interval to add (last element only)
    //        {
    //            //remove last element if it is not multiplier of this.Interval. it will be executed only when we are adding sub-interval data
    //            var bRemoveLastElement = (this.Count > 0 &&
    //                this.Interval != interval &&
    //                this.LastTime % this.Interval != 0 &&                                           // 表示最后一个item是不完整的
    //                (q.Time[indexStartSearch] / this.Interval != this.LastTime / this.Interval ||   // 表示会添加一个完整的item，因此移除之前不完整的item
    //                q.LastTime > this.LastTime));                                                   // 表示存在最新的item（不完整的）

    //            if (bRemoveLastElement) {
    //                Time.RemoveAt(Time.Count - 1);
    //                Open.RemoveAt(Open.Count - 1);
    //                High.RemoveAt(High.Count - 1);
    //                Low.RemoveAt(Low.Count - 1);
    //                Close.RemoveAt(Close.Count - 1);
    //                Volume.RemoveAt(Volume.Count - 1);
    //            }

    //            /////////////////////////////////////////////////////////////
    //            //var isNewElementAdded = false;
    //            int sindex = indexStartSearch;
    //            int len = 0;

    //            for (int i = indexStartSearch; i <= q.Count - 1; i++) {
    //                ++len;
    //                //if ((q._time[i] % this.Interval == 0) ||                                                       //exactly at Interval
    //                //    (numAddedElement > 0 && q._time[i] / this.Interval != q._time[i - 1] / this.Interval))     //span over boundary; 1st element we never consider span; this might cause difference for mearged quote.                                                                                                                                               
    //                if (q.Time[i] / this.Interval != this.LastTime / this.Interval) {
    //                    var eindex = sindex + len - 1;
    //                    var time = q.Time[i] / this.Interval * this.Interval;
    //                    var open = q.Open[sindex];
    //                    var close = q.Close[eindex];
    //                    var volume = q.Volume[eindex];
    //                    var high = q.High.GetRange(sindex, len).Max();
    //                    var low = q.Low.GetRange(sindex, len).Min();

    //                    //repeat quote to fill gap
    //                    if (isFillGap && this.Count > 0) {
    //                        var c = this.Close.LastOrDefault();
    //                        var v = this.Volume.LastOrDefault();
    //                        while (time > this.LastTime + this.Interval)
    //                            this.Add(this.LastTime + this.Interval, c, c, c, c, v);
    //                    }

    //                    this.Add(time, open, high, low, close, volume);       //add new element
    //                    ++numAddedElement;

    //                    //current element should be excluded from next group if it is at interval
    //                    if (q.Time[i] % this.Interval == 0) {
    //                        len = 0;
    //                        sindex = i + 1;
    //                    }
    //                    else {
    //                        len = 1;
    //                        sindex = i;
    //                    }
    //                }
    //            }

    //            //add last element even if they are not multiple of this.Interval but multiples of interval
    //            //note that sindex is always at the beginning element for new group
    //            if (sindex <= q.Count - 1 && interval < this.Interval && q.LastTime % interval == 0) {
    //                len = q.Count - sindex;
    //                var time = q.Time.LastOrDefault();
    //                var open = q.Open[sindex];
    //                var close = q.Close.LastOrDefault();
    //                var high = q.High.GetRange(sindex, len).Max();
    //                var low = q.Low.GetRange(sindex, len).Min();

    //                this.Add(time, open, high, low, close, 0);  //add new element
    //                ++numAddedElement;
    //            }
    //        }

    //        return numAddedElement;
    //    }
    //    public int Append(IQuoteCapture q, int interval = -1, bool isFillGap = false)
    //    {
    //        var numAddedElement = 0;

    //        if (q == null || q.Count <= 0 || q.LastTime <= this.LastTime || this.Symbol != q.Symbol)
    //            return numAddedElement;

    //        var lastTime = this.LastTime / this.Interval * this.Interval;
    //        interval = (interval == -1) ? this.Interval : interval;

    //        //search backward for the quotes. the found time should be >= lastTime
    //        int indexStartSearch = -1;
    //        for (int i = q.Count - 1; i >= 0; i--) {
    //            if (q.Time[i] <= lastTime)               //for basic quote we include price at previous interval for calculation
    //                break;

    //            indexStartSearch = i;
    //        }
    //        if (indexStartSearch == -1) return numAddedElement;

    //        if ((q.LastTime / this.Interval != q.Time[indexStartSearch] / this.Interval) || //jump over boundary
    //            (q.LastTime % interval == 0))                                               //at boundary 
    //        {
    //            //remove last element if it is not multiplier of this.Interval. it will be executed only when we are adding sub-interval data
    //            var bRemoveLastElement = (this.Count > 0 &&
    //                this.Interval != interval &&
    //                this.LastTime % this.Interval != 0 &&
    //                 (q.Time[indexStartSearch] / this.Interval != this.LastTime / this.Interval ||
    //                q.LastTime > this.LastTime));

    //            if (bRemoveLastElement) {
    //                Time.RemoveAt(Time.Count - 1);
    //                Open.RemoveAt(Open.Count - 1);
    //                High.RemoveAt(High.Count - 1);
    //                Low.RemoveAt(Low.Count - 1);
    //                Close.RemoveAt(Close.Count - 1);
    //                Volume.RemoveAt(Volume.Count - 1);
    //            }

    //            //////////////////////////////////////////////////////
    //            int sindex = indexStartSearch;
    //            int len = 0;
    //            for (int i = indexStartSearch; i <= q.Count - 1; i++) {
    //                ++len;
    //                //if ((q.Time[i] % this.Interval == 0) ||                                       //new element started 
    //                //    (i > 0 && q.Time[i] / this.Interval != q.Time[i - 1] / this.Interval))    //jump over bounary                  
    //                if (q.Time[i] / this.Interval != this.LastTime / this.Interval) {
    //                    var eindex = sindex + len - 1;
    //                    var price = q.Price.GetRange(sindex, len);
    //                    var time = q.Time[i] / this.Interval * this.Interval;
    //                    var open = q.Price[sindex];
    //                    var close = q.Price[eindex];
    //                    var high = price.Max();
    //                    var low = price.Min();

    //                    //repeat quote to fill gap
    //                    if (isFillGap && this.Count > 0) {
    //                        while (time > this.LastTime + this.Interval) {
    //                            this.Add(this.LastTime + this.Interval,
    //                                     this.Open.LastOrDefault(), this.High.LastOrDefault(),
    //                                     this.Low.LastOrDefault(), this.Close.LastOrDefault(), this.Volume.LastOrDefault());
    //                        }
    //                    }

    //                    this.Add(time, open, high, low, close, 0);  //add the data
    //                    ++numAddedElement;

    //                    //include current element to the next group
    //                    len = 1;
    //                    sindex = i;
    //                }
    //            }

    //            //add last element even if they are not multiple of this.Interval but multiples of interval
    //            //note that sindex is always at the element for new group
    //            if (sindex <= q.Count - 1 && this.Interval != interval && q.LastTime % interval == 0) {
    //                len = q.Count - sindex;
    //                var eindex = sindex + len - 1;
    //                var price = q.Price.GetRange(sindex, len);
    //                var time = q.LastTime;
    //                var open = q.Price[sindex];
    //                var close = q.Price[eindex];
    //                var high = price.Max();
    //                var low = price.Min();
    //                this.Add(time, open, high, low, close, 0);  //add the data
    //                ++numAddedElement;
    //            }
    //        }

    //        return numAddedElement;
    //    }

    //    //return index whose timestamp >= time
    //    public int FindIndexForGivenTime(long time, bool isReturnJustSmallerElement = false)
    //    {
    //        if (this.Count <= 0) return -1;
    //        return General.BinarySearch(Time, 0, Time.Count - 1, time, isReturnJustSmallerElement);
    //    }

    //    public int FindIndexWhereTimeLocated(long time)
    //    {
    //        if (this.Count <= 0)
    //            return -1;

    //        var index = General.BinarySearch(Time, 0, Time.Count - 1, time, true);

    //        if (index == -1)
    //            return index;

    //        if (Time[index] != time)  //if give time is not in the list, BinarySearch returns the index of the element which is just smaller than given time                                           
    //            ++index;              //in this case, we should increment index

    //        if (index >= this.Count)  //out of range
    //            index = -1;

    //        return index;
    //    }
    //    public IQuoteBasic Extract(long stime, long etime)
    //    {
    //        int sindex = 0, eindex = this.Count - 1;            //by default we extrace whole data

    //        if (stime > this.FirstTime)
    //            sindex = this.FindIndexWhereTimeLocated(stime);

    //        if (etime < this.LastTime)
    //            eindex = this.FindIndexWhereTimeLocated(etime);

    //        return Extract(sindex, eindex);
    //    }
    //    public IQuoteBasic Extract(int sindex, int eindex)
    //    {
    //        if (sindex < 0 || eindex == -1 || eindex > this.Count - 1 || eindex < sindex) return null;

    //        int num = eindex - sindex + 1;
    //        var quote = new QuoteBasic(Symbol, Interval);
    //        quote.Time.AddRange(Time.GetRange(sindex, num));
    //        quote.Open.AddRange(Open.GetRange(sindex, num));
    //        quote.High.AddRange(High.GetRange(sindex, num));
    //        quote.Low.AddRange(Low.GetRange(sindex, num));
    //        quote.Close.AddRange(Close.GetRange(sindex, num));
    //        quote.Volume.AddRange(Volume.GetRange(sindex, num));
    //        return quote;
    //    }

    //    //insert missing quote
    //    public bool Insert(IQuoteBasic q)
    //    {
    //        if (q == null || q.Count <= 0) return false;
    //        if (this.Symbol != q.Symbol || this.Interval != q.Interval) return false;
    //        if (this.Count <= 0 || q.FirstTime > this.LastTime) {
    //            this.Time.AddRange(q.Time);
    //            this.Open.AddRange(q.Open);
    //            this.High.AddRange(q.High);
    //            this.Low.AddRange(q.Low);
    //            this.Close.AddRange(q.Close);
    //            this.Volume.AddRange(q.Volume);
    //            return true;
    //        }

    //        if (q.LastTime < this.FirstTime) {
    //            this.Time.InsertRange(0, q.Time);
    //            this.Open.InsertRange(0, q.Open);
    //            this.High.InsertRange(0, q.High);
    //            this.Low.InsertRange(0, q.Low);
    //            this.Close.InsertRange(0, q.Close);
    //            this.Volume.InsertRange(0, q.Volume);
    //            return true;
    //        }

    //        var sindex = this.FindIndexForGivenTime(q.FirstTime, true);
    //        var eindex = this.FindIndexForGivenTime(q.LastTime, true);
    //        if (sindex == -1 || eindex == -1) return false;

    //        if (this.Time[eindex] != q.LastTime)
    //            eindex += 1;                              //points to elment just larger than last time

    //        //we know Time[sindex] <= q.FirstTime and Time[eindex] >= q.LastTime => we can merge the two sequences
    //        var i = sindex;
    //        var j = 0;
    //        var times = new List<long>();
    //        var open = new List<double>();
    //        var close = new List<double>();
    //        var high = new List<double>();
    //        var low = new List<double>();
    //        var volume = new List<double>();

    //        while (i <= eindex || j <= q.Count - 1) {
    //            if (j > q.Count - 1) {   //take all from i
    //                times.AddRange(this.Time.GetRange(i, eindex - i + 1));
    //                open.AddRange(this.Open.GetRange(i, eindex - i + 1));
    //                close.AddRange(this.Close.GetRange(i, eindex - i + 1));
    //                high.AddRange(this.High.GetRange(i, eindex - i + 1));
    //                low.AddRange(this.Low.GetRange(i, eindex - i + 1));
    //                volume.AddRange(this.Volume.GetRange(i, eindex - i + 1));
    //                break;
    //            }
    //            if (i > eindex) {        //take all from j
    //                times.AddRange(q.Time.GetRange(j, q.Count - j));
    //                open.AddRange(q.Open.GetRange(j, q.Count - j));
    //                close.AddRange(q.Close.GetRange(j, q.Count - j));
    //                high.AddRange(q.High.GetRange(j, q.Count - j));
    //                low.AddRange(q.Low.GetRange(j, q.Count - j));
    //                volume.AddRange(q.Volume.GetRange(j, q.Count - j));
    //                break;
    //            }
    //            while (i <= eindex && j < q.Count && this.Time[i] == q.Time[j]) {
    //                times.Add(this.Time[i]);
    //                open.Add(this.Open[i]);
    //                close.Add(this.Close[i]);
    //                high.Add(this.High[i]);
    //                low.Add(this.Low[i]);
    //                volume.Add(this.Volume[i]);
    //                ++i;
    //                ++j;
    //            }
    //            while (i <= eindex && this.Time[i] < q.Time[j]) {
    //                times.Add(this.Time[i]);
    //                open.Add(this.Open[i]);
    //                close.Add(this.Close[i]);
    //                high.Add(this.High[i]);
    //                low.Add(this.Low[i]);
    //                volume.Add(this.Volume[i]);
    //                ++i;
    //            }
    //            while (j < q.Count && q.Time[j] < this.Time[i]) {
    //                times.Add(q.Time[j]);
    //                open.Add(q.Open[j]);
    //                close.Add(q.Close[j]);
    //                high.Add(q.High[j]);
    //                low.Add(q.Low[j]);
    //                volume.Add(q.Volume[j]);
    //                ++j;
    //            }
    //        }

    //        //remove old data
    //        this.Time.RemoveRange(sindex, eindex - sindex + 1);
    //        this.Open.RemoveRange(sindex, eindex - sindex + 1);
    //        this.Close.RemoveRange(sindex, eindex - sindex + 1);
    //        this.High.RemoveRange(sindex, eindex - sindex + 1);
    //        this.Low.RemoveRange(sindex, eindex - sindex + 1);
    //        this.Volume.RemoveRange(sindex, eindex - sindex + 1);

    //        //insert new data
    //        var insertIndex = Math.Max(0, sindex - 1);
    //        this.Time.InsertRange(insertIndex, times);
    //        this.Open.InsertRange(insertIndex, open);
    //        this.Close.InsertRange(insertIndex, close);
    //        this.High.InsertRange(insertIndex, high);
    //        this.Low.InsertRange(insertIndex, low);
    //        this.Volume.InsertRange(insertIndex, volume);

    //        return true;
    //        ////we are sure elements are missing
    //        //if (eindex - sindex + 1 < q.Count) {
    //        //    var times = this.Time.GetRange(sindex, eindex - sindex + 1);
    //        //    var insertIndex = sindex + 1;
    //        //    for (int i = 0; i < q.Count; i++) {
    //        //        if (times.Contains(q.Time[i])) continue;
    //        //        this.Time.Insert(insertIndex, q.Time[i]);
    //        //        this.Open.Insert(insertIndex, q.Open[i]);
    //        //        this.High.Insert(insertIndex, q.High[i]);
    //        //        this.Low.Insert(insertIndex, q.Low[i]);
    //        //        this.Close.Insert(insertIndex, q.Close[i]);
    //        //        this.Volume.Insert(insertIndex, q.Volume[i]);
    //        //        ++insertIndex;
    //        //    }
    //        //}
    //    }

    //    public void Clear()
    //    {
    //        this.Time.Clear();
    //        this.Open.Clear();
    //        this.Close.Clear();
    //        this.High.Clear();
    //        this.Low.Clear();
    //        this.Volume.Clear();
    //    }
    //}

}
