using Newtonsoft.Json;
using PortableCSharpLib.DataType;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace PortableCSharpLib.TechnicalAnalysis
{
    /// <summary>
    /// basic quote data structure defined as [interval, interval*2). for example [0, 1, 2, 3, 4] => 0, [5,6,7,8,9] => 1
    /// </summary>
    public class QuoteBasicBase : IQuoteBasicBase
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

        public QuoteBasicBase(string symbol, int interval)
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
        public QuoteBasicBase(String Symbol, int Interval, IList<long> Time, IList<double> Open, IList<double> Close, IList<double> High, IList<double> Low, IList<double> Volume)
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

        public QuoteBasicBase(List<OHLC> ohlc)
        {
            if (ohlc == null || ohlc.Count == 0) return;
            var b = ohlc[0];
            this.Symbol = b.Symbol;
            this.Interval = b.Interval;
            this.Time = ohlc.Select(o => o.Time).ToList();
            this.Open = ohlc.Select(o => o.Open).ToList();
            this.Close = ohlc.Select(o => o.Close).ToList();
            this.High = ohlc.Select(o => o.High).ToList();
            this.Low = ohlc.Select(o => o.Low).ToList();
            this.Volume = ohlc.Select(o => o.Volume).ToList();
        }

        /// <summary>
        /// 复制IQuoteBasic的内容
        /// </summary>
        /// <param name="q"></param>
        public QuoteBasicBase(IQuoteBasicBase q)
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

        public event EventHandlers.DataAddedOrUpdatedEventHandler OnDataAddedOrUpdated;
        public event EventHandlers.DataRemovedEventHandler OnDataRemoved;

        #region add data
        /// <summary>
        /// >=1 means added
        /// 0 means last element updated
        /// -1 means nothing changed
        /// </summary>
        /// <param name="t"></param>
        /// <param name="o"></param>
        /// <param name="h"></param>
        /// <param name="l"></param>
        /// <param name="c"></param>
        /// <param name="v"></param>
        /// <param name="isTriggerDataAdded"></param>
        /// <returns></returns>
        public int AddUpdate(long t, double o, double h, double l, double c, double v, bool isTriggerDataAdded = false)
        {
            if (this.Count == 0 || t > this.LastTime)
            {
                Open.Add(o);
                High.Add(h);
                Low.Add(l);
                Close.Add(c);
                Volume.Add(v);
                Time.Add(t);
                if (isTriggerDataAdded) OnDataAddedOrUpdated?.Invoke(this, this, 1);
                return 1;
            }
            else if (t == this.LastTime)
            {
                Open[this.Count - 1] = o;
                High[this.Count - 1] = h;
                Low[this.Count - 1] = l;
                Close[this.Count - 1] = c;
                Volume[this.Count - 1] = v;
                if (isTriggerDataAdded) OnDataAddedOrUpdated?.Invoke(this, this, 0);
                return 0;
            }
            return -1;
        }

        public int AddUpdate(string symbol, int interval, long t, double o, double h, double l, double c, double v, bool isTriggerDataAdded = false)
        {
            if (this.Symbol != symbol || this.Interval < interval || this.Interval%interval != 0) return 0;

            var time = t / this.Interval * this.Interval;
            if (this.Count == 0 || time > this.LastTime)
            {
                Open.Add(o);
                High.Add(h);
                Low.Add(l);
                Close.Add(c);
                Volume.Add(v);
                Time.Add(time);
                if (isTriggerDataAdded) OnDataAddedOrUpdated?.Invoke(this, this, 1);
                return 1;
            }
            else if (time == this.LastTime)
            {
                High[this.Count - 1] = Math.Max(h, High[this.Count - 1]);
                Low[this.Count - 1] = Math.Min(Low[this.Count - 1], l);
                Close[this.Count - 1] = c;
                if (interval == this.Interval)
                    Volume[this.Count - 1] = v;
                else
                    Volume[this.Count - 1] += v;
                if (isTriggerDataAdded) OnDataAddedOrUpdated?.Invoke(this, this, 0);
                return 0;
            }
            return -1;
        }

        //[0,1,2,4,5)
        public int Append(IQuoteBasicBase q, bool isTriggerDataUpdated = false)
        {
            //no data to add
            if (q == null || q.Count <= 0 || q.LastTime < this.LastTime || this.Symbol != q.Symbol || this.Interval < q.Interval || this.Interval % q.Interval != 0) return 0;

            //search backward for quotes to be added. the found time should be >= this.LastTime
            int indexStartSearch = -1;
            for (int i = q.Count - 1; i >= 0; i--)
            {
                if (q.Time[i] < this.LastTime)
                    break;
                indexStartSearch = i;
            }
            if (indexStartSearch == -1) return 0;

            /////////////////////////////////////////////////////////////////////////////////
            var numBeforeAdd = this.Count;
            var isDataChanged = false;
            var numAddedElement = 0;

            int sindex = indexStartSearch; // interval区间的开始索引
            var eindex = -1;
            var endTime = q.Time[sindex] / this.Interval * this.Interval + this.Interval;      //use the first time as the data bar time
            for (int i = indexStartSearch; i <= q.Count - 1; i++)
            {
                if (q.Time[i] >= endTime)
                {
                    eindex = i - 1;        // interval区间的结束索引
                    var num = this.AddItemByQuoteBasic(q, sindex, eindex);
                    if (num >= 0)
                    {
                        numAddedElement += num;
                        isDataChanged = true;
                    }
                    sindex = i;
                    endTime = q.Time[sindex] / this.Interval * this.Interval + this.Interval;
                }
            }

            //add last element
            if (sindex > eindex)
            {
                var num = this.AddItemByQuoteBasic(q, sindex, q.Count - 1);
                if (num >= 0)
                {
                    numAddedElement += num;
                    isDataChanged = true;
                }
            }

            if (isTriggerDataUpdated && isDataChanged)
                OnDataAddedOrUpdated?.Invoke(this, this, numAddedElement);

            return isDataChanged? numAddedElement : -1;  //-1 means nothing changed, 0 means updated, >=1 means added
        }

        public int Append(IQuoteCapture q, bool isTriggerDataUpdated = false)
        {
            if (q == null || q.Count <= 0 || q.LastTime <= this.LastTime || this.Symbol != q.Symbol)
                return 0;

            //search backward for the quotes. the found time should be >= lastTime
            int indexStartSearch = -1;
            for (int i = q.Count - 1; i >= 0; i--)
            {
                if (q.Time[i] < this.LastTime)               //for basic quote we include price at previous interval for calculation
                    break;

                indexStartSearch = i;
            }
            if (indexStartSearch == -1) return 0;

            ////////////////////////////////////////////////////////////////////////////
            var isDataChanged = false;
            var numAddedElement = 0;

            int sindex = indexStartSearch; // interval区间的开始索引
            var eindex = -1;
            var endTime = q.Time[sindex] / this.Interval * this.Interval + this.Interval;      //use the first time as the data bar time
            for (int i = indexStartSearch; i <= q.Count - 1; i++)
            {
                if (q.Time[i] >= endTime)
                {
                    eindex = i - 1;        // interval区间的结束索引
                    var num = this.AddItemByQuoteCapture(q, sindex, eindex);
                    if (num >= 0)
                    {
                        numAddedElement += num;
                        isDataChanged = true;
                    }
                    sindex = i;
                    endTime = q.Time[sindex] / this.Interval * this.Interval + this.Interval;
                }
            }

            //add last element
            if (sindex > eindex)
            {
                var num = this.AddItemByQuoteCapture(q, sindex, q.Count - 1);
                if (num >= 0)
                {
                    numAddedElement += num;
                    isDataChanged = true;
                }
            }

            if (isTriggerDataUpdated && isDataChanged)
                OnDataAddedOrUpdated?.Invoke(this, this, numAddedElement);

            return isDataChanged ? numAddedElement : -1;  //-1 means nothing changed, 0 means updated, >=1 means added
        }

        private int AddItemByQuoteBasic(IQuoteBasicBase q, int sindex, int eindex)
        {
            var len = eindex - sindex + 1;
            var open = q.Open[sindex];
            var close = q.Close[eindex];
            var high = q.High.GetRange(sindex, len).Max();
            var low = q.Low.GetRange(sindex, len).Min();
            var volume = q.Volume.GetRange(sindex, len).Sum();
            var time = q.Time[sindex] / this.Interval * this.Interval;

            //repeat quote to fill gap
            //FillGap(isFillGap, endTime, ref numAddedElement);
            return this.AddUpdate(time, open, high, low, close, volume);  //add the data
        }

        private int AddItemByQuoteCapture(IQuoteCapture q, int sindex, int eindex)
        {
            var len = eindex - sindex + 1;
            var price = q.Price.GetRange(sindex, len);
            var volumnList = q.Volume.GetRange(sindex, len);
            var time = q.Time[sindex] / this.Interval * this.Interval;
            var open = this.Count > 0 ? this.Close.Last() : q.Price[sindex];
            var close = q.Price[eindex];
            var high = price.Max();
            var low = price.Min();
            var volumn = volumnList.Sum();
            return this.AddUpdate(time, open, high, low, close, volumn);  //add the data
        }

        //private void FillGap(bool isFillGap, long endTime, ref int numAddedElement)
        //{
        //    if (this.Count > 0 && isFillGap)
        //    {
        //        while (endTime > this.LastTime + this.Interval)
        //        {
        //            this.Add(this.LastTime + this.Interval,
        //                     this.Open.LastOrDefault(), this.High.LastOrDefault(),
        //                     this.Low.LastOrDefault(), this.Close.LastOrDefault(), 0);
        //            ++numAddedElement;
        //        }
        //    }
        //}
        #endregion


        //return index whose timestamp <= time
        public int FindIndexForGivenTime(long time, bool isReturnJustSmallerElement = false)
        {
            if (this.Count <= 0) return -1;
            if (time < this.FirstTime || time > this.LastTime) return -1;

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
                Time[index + 1] >= (time / this.Interval + 1) * this.Interval)    //if give time is not in the list, BinarySearch returns the index of the element which is just smaller than given time                                           
                ++index;                                                          //in this case, we should increment index
            else
                index = -1;

            return index;
        }


        public QuoteBasicBase Extract(long stime, long etime)
        {
            //var q = new QuoteBasic(this.Symbol, this.Interval);
            if (this.Count == 0 || stime > etime || stime > this.LastTime || etime < this.FirstTime)
                return null;
            int sindex = 0, eindex = this.Count - 1;            //by default we extrace whole data

            if (stime > this.FirstTime)
            {
                sindex = this.FindIndexForGivenTime(stime, true);
                if (this.Time[sindex] != stime)
                    ++sindex;
            }

            if (sindex >= this.Count)
                return null;

            if (etime < this.LastTime)
                eindex = this.FindIndexForGivenTime(etime, true); // <= etime

            if (sindex > eindex)
                return null;

            return this.Extract(sindex, eindex);
        }
        public QuoteBasicBase Extract(int sindex, int eindex)
        {
            if (sindex < 0 || eindex > this.Count - 1 || eindex < sindex)
                throw new ArgumentException(string.Format("Function {0} sindex: {1}, eindex: {2}, Count: {3}",
                    "Extract", sindex, eindex, this.Count));

            int num = eindex - sindex + 1;
            var quote = new QuoteBasicBase(Symbol, Interval);
            quote.Time.AddRange(Time.GetRange(sindex, num));
            quote.Open.AddRange(Open.GetRange(sindex, num));
            quote.High.AddRange(High.GetRange(sindex, num));
            quote.Low.AddRange(Low.GetRange(sindex, num));
            quote.Close.AddRange(Close.GetRange(sindex, num));
            quote.Volume.AddRange(Volume.GetRange(sindex, num));
            return quote;
        }

        ////insert missing quote
        //public bool Insert(IQuoteBasic q)
        //{
        //    if (q == null || q.Count <= 0) return false;
        //    if (this.Symbol != q.Symbol || this.Interval != q.Interval) return false;
        //    if (this.Count <= 0 || q.FirstTime > this.LastTime)
        //    {
        //        this.Time.AddRange(q.Time);
        //        this.Open.AddRange(q.Open);
        //        this.High.AddRange(q.High);
        //        this.Low.AddRange(q.Low);
        //        this.Close.AddRange(q.Close);
        //        this.Volume.AddRange(q.Volume);
        //        return true;
        //    }

        //    if (q.LastTime < this.FirstTime)
        //    {
        //        this.Time.InsertRange(0, q.Time);
        //        this.Open.InsertRange(0, q.Open);
        //        this.High.InsertRange(0, q.High);
        //        this.Low.InsertRange(0, q.Low);
        //        this.Close.InsertRange(0, q.Close);
        //        this.Volume.InsertRange(0, q.Volume);
        //        return true;
        //    }

        //    var sindex = this.FindIndexForGivenTime(q.FirstTime, true);
        //    var eindex = this.FindIndexForGivenTime(q.LastTime, true);
        //    if (sindex == -1 || eindex == -1) return false;

        //    if (this.Time[eindex] != q.LastTime && eindex < this.Count - 1)
        //        eindex += 1;                              //points to elment just larger than last time

        //    //we know Time[sindex] <= q.FirstTime and Time[eindex] >= q.LastTime => we can merge the two sequences
        //    var i = sindex;
        //    var j = 0;
        //    var times = new List<long>();
        //    var open = new List<double>();
        //    var close = new List<double>();
        //    var high = new List<double>();
        //    var low = new List<double>();
        //    var volume = new List<double>();

        //    while (i <= eindex || j <= q.Count - 1)
        //    {
        //        if (j > q.Count - 1)
        //        {   //take all from i
        //            times.AddRange(this.Time.GetRange(i, eindex - i + 1));
        //            open.AddRange(this.Open.GetRange(i, eindex - i + 1));
        //            close.AddRange(this.Close.GetRange(i, eindex - i + 1));
        //            high.AddRange(this.High.GetRange(i, eindex - i + 1));
        //            low.AddRange(this.Low.GetRange(i, eindex - i + 1));
        //            volume.AddRange(this.Volume.GetRange(i, eindex - i + 1));
        //            break;
        //        }
        //        if (i > eindex)
        //        {        //take all from j
        //            times.AddRange(q.Time.GetRange(j, q.Count - j));
        //            open.AddRange(q.Open.GetRange(j, q.Count - j));
        //            close.AddRange(q.Close.GetRange(j, q.Count - j));
        //            high.AddRange(q.High.GetRange(j, q.Count - j));
        //            low.AddRange(q.Low.GetRange(j, q.Count - j));
        //            volume.AddRange(q.Volume.GetRange(j, q.Count - j));
        //            break;
        //        }
        //        while (i <= eindex && j < q.Count && this.Time[i] == q.Time[j])
        //        {
        //            times.Add(this.Time[i]);
        //            open.Add(this.Open[i]);
        //            close.Add(this.Close[i]);
        //            high.Add(this.High[i]);
        //            low.Add(this.Low[i]);
        //            volume.Add(this.Volume[i]);
        //            ++i;
        //            ++j;
        //        }
        //        while (i <= eindex && this.Time[i] < q.Time[j])
        //        {
        //            times.Add(this.Time[i]);
        //            open.Add(this.Open[i]);
        //            close.Add(this.Close[i]);
        //            high.Add(this.High[i]);
        //            low.Add(this.Low[i]);
        //            volume.Add(this.Volume[i]);
        //            ++i;
        //        }
        //        while (j < q.Count && q.Time[j] < this.Time[i])
        //        {
        //            times.Add(q.Time[j]);
        //            open.Add(q.Open[j]);
        //            close.Add(q.Close[j]);
        //            high.Add(q.High[j]);
        //            low.Add(q.Low[j]);
        //            volume.Add(q.Volume[j]);
        //            ++j;
        //        }
        //    }

        //    //remove old data
        //    this.Time.RemoveRange(sindex, eindex - sindex + 1);
        //    this.Open.RemoveRange(sindex, eindex - sindex + 1);
        //    this.Close.RemoveRange(sindex, eindex - sindex + 1);
        //    this.High.RemoveRange(sindex, eindex - sindex + 1);
        //    this.Low.RemoveRange(sindex, eindex - sindex + 1);
        //    this.Volume.RemoveRange(sindex, eindex - sindex + 1);

        //    //insert new data
        //    var insertIndex = Math.Max(0, sindex - 1);
        //    this.Time.InsertRange(insertIndex, times);
        //    this.Open.InsertRange(insertIndex, open);
        //    this.Close.InsertRange(insertIndex, close);
        //    this.High.InsertRange(insertIndex, high);
        //    this.Low.InsertRange(insertIndex, low);
        //    this.Volume.InsertRange(insertIndex, volume);

        //    return true;
        //}

        public void Clear()
        {
            this.Time.Clear();
            this.Open.Clear();
            this.Close.Clear();
            this.High.Clear();
            this.Low.Clear();
            this.Volume.Clear();
            this.OnDataRemoved?.Invoke(this, this);
        }

        //public void Clear(long stime, long etime)
        //{
        //    if (this.Count == 0 || stime > etime || stime > this.LastTime || etime < this.FirstTime)
        //        return;
        //    int sindex = 0, eindex = this.Count - 1;            //by default we extrace whole data

        //    if (stime > this.FirstTime)
        //    {
        //        sindex = this.FindIndexForGivenTime(stime, true);
        //        if (this.Time[sindex] != stime)
        //            ++sindex;
        //    }

        //    if (sindex >= this.Count)
        //        return;

        //    if (etime < this.LastTime)
        //        eindex = this.FindIndexForGivenTime(etime, true); // <= etime

        //    if (sindex > eindex)
        //        return;

        //    Clear(sindex, eindex);
        //}

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
            this.OnDataRemoved?.Invoke(this, this);
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
        public static QuoteBasicBase InitByStream(Stream stream)
        {
            if (stream == null)
                return null;
            var qb = new QuoteBasicBase("", 1);
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
                    this.AddUpdate(long.Parse(segments[0]), ParseDouble(segments[2]), ParseDouble(segments[3]),
                        ParseDouble(segments[4]), ParseDouble(segments[1]), ParseDouble(segments[5]));
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
    }
}
