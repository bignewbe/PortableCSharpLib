using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

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
        [DataMember]
        public List<double> Volume { get; private set; }

        private const long MIN_INTERVAL = 1;

        private const double NO_USED_PRICE = -1;

        public QuoteCapture()
        {
            Symbol = null;
            PipFactor = 1;
            Time = new List<long>();
            Price = new List<double>();
            Volume = new List<double>();
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
        public QuoteCapture(string Symbol, double PipFactor, IList<long> Time, IList<double> Price, IList<double> Volume)
        {
            this.Create(Symbol, PipFactor, Time, Price, Volume);
        }

        public event EventHandlers.QuoteCaptureDataAddedEventHandler DataAdded;

        private void Create(IQuoteCapture q)
        {
            if (q != null)
                this.Create(q.Symbol, q.PipFactor, q.Time, q.Price, q.Volume);
        }

        private void Create(string symbol, double pipFactor, IList<long> time, IList<double> price, IList<double> volumn)
        {
            this.Symbol = symbol;
            this.PipFactor = pipFactor;
            this.Time = new List<long>(time);
            this.Price = new List<double>(price);
            this.Volume = new List<double>(volumn);
        }

        public void Assign(IQuoteCapture q)
        {
            this.Create(q);
        }
        public void Add(long t, double p)
        {
            this.Add(t, p, 0);
        }
        public virtual void Add(long time, double buyPrice, double sellPrice, double volumn)
        {
            this.Add(time, (buyPrice + sellPrice) / 2.0);
        }

        public void AddMonotonic(long time, double price)
        {//如果time> LastTime才添加
            AddMonotonic(time, price, (long)0);
        }

        public void AddMonotonic(long time, double price, double volumn)
        {//如果time >= LastTime才添加
            if (this.Time.Count == 0 || this.LastTime <= time)
                Add(time, price, volumn);
        }

        public IQuoteCapture Distinct()
        {
            var result = new QuoteCapture(this.Symbol);
            if (this.Count == 0)
                return result;
            for (var i = 0; i < this.Count; i++)
                result.AddMonotonic(this.Time[i], this.Price[i], this.Volume[i]);

            return result;
        }

        public void Sort(bool isDistinct = true, bool isIncrease = true)
        {
            if (this.Count == 0)
                return;

            //快速排序
            this.QSort(isIncrease);

            if (!isDistinct)
                return;
            //去重
            var i = 1;
            while (i < this.Count)
            {
                if (this.Time[i] == this.Time[i - 1])//重复数据
                {
                    this.Time.RemoveAt(i);
                    this.Price.RemoveAt(i);
                    this.Volume.RemoveAt(i);
                }
                else
                    i++;
            }
        }

        public virtual void Append(IQuoteCapture q)
        {
            if (q == null || q.Count <= 0 || this.Symbol != q.Symbol)
                return;

            var sindex = q.Count - 1;
            while (sindex >= 0 && q.Time[sindex] > this.LastTime) --sindex;

            if (sindex == -1)
            {
                this.Time.AddRange(q.Time);
                this.Price.AddRange(q.Price);
                this.Volume.AddRange(q.Volume);
            }
            else if (sindex < q.Count - 1)
            {
                ++sindex;
                this.Time.AddRange(q.Time.GetRange(sindex, q.Count - sindex));
                this.Price.AddRange(q.Price.GetRange(sindex, q.Count - sindex));
                this.Volume.AddRange(q.Volume.GetRange(sindex, q.Count - sindex));
            }
        }

        public virtual IQuoteCapture Extract(long stime, long etime)
        {

            var q = new QuoteCapture(this.Symbol);
            if (this.Count == 0 || stime > etime || stime > this.LastTime || etime < this.FirstTime)
                return q;

            int sIndex = General.BinarySearch(Time, 0, Time.Count - 1, stime, true);

            if (sIndex < 0)
            {
                sIndex++;
            }
            else if (Time[sIndex] != stime)
            {
                sIndex++;
                if (sIndex > Time.Count - 1)
                {
                    return q;
                }
            }

            int eIndex = General.BinarySearch(Time, 0, Time.Count - 1, etime, true);
            if (eIndex == -1)
                eIndex = Time.Count - 1;

            if (sIndex > eIndex)
            {
                return q;
            }

            return Extract(sIndex, eIndex);
        }
        public virtual IQuoteCapture Extract(int sindex, int eindex)
        {
            if (sindex < 0 || eindex > this.Count - 1 || eindex < sindex)
                throw new ArgumentException(string.Format("Function {0} sindex: {1}, eindex: {2}, Count: {3}",
                    "Extract", sindex, eindex, this.Count));

            int num = eindex - sindex + 1;
            var quote = new QuoteCapture(Symbol);
            quote.Time.AddRange(Time.GetRange(sindex, num));
            quote.Price.AddRange(Price.GetRange(sindex, num));
            quote.Volume.AddRange(Volume.GetRange(sindex, num));
            return quote;
        }

        public void Clear()
        {
            this.Price.Clear();
            this.Time.Clear();
            this.Volume.Clear();
        }

        public void Clear(long stime, long etime)
        {
            if (this.Count == 0 || stime > etime || stime > this.LastTime || etime < this.FirstTime)
                return;

            int sIndex = General.BinarySearch(Time, 0, Time.Count - 1, stime, true);

            if (sIndex < 0)
            {
                sIndex++;
            }
            else if (Time[sIndex] != stime)
            {
                sIndex++;
                if (sIndex > Time.Count - 1)
                {
                    return;
                }
            }

            int eIndex = General.BinarySearch(Time, 0, Time.Count - 1, etime, true);
            if (eIndex == -1)
                eIndex = Time.Count - 1;

            if (sIndex > eIndex)
            {
                return;
            }
            Clear(sIndex, eIndex);
        }

        public void Clear(int sindex, int eindex)
        {
            if (sindex < 0 || eindex > this.Count - 1 || eindex < sindex)
                throw new ArgumentException(string.Format("Function {0} sindex: {1}, eindex: {2}, Count: {3}",
                    "Extract", sindex, eindex, this.Count));

            int num = eindex - sindex + 1;
            this.Time.RemoveRange(sindex, num);
            this.Price.RemoveRange(sindex, num);
            this.Volume.RemoveRange(sindex, num);
        }

        public void AppendStream(Stream stream)
        {
            if (this.Count <= 0 || stream == null)
                return;

            var sr = new StreamWriter(stream);
            if (stream.Position == 0)
            {
                sr.WriteLine("Symbol:{0};PipFactor:{1}", this.Symbol, this.PipFactor);
                sr.WriteLine("Time;Price;Volume;");
            }

            for (int i = 0; i < this.Time.Count; i++)
                sr.WriteLine("{0};{1};{2}", this.Time[i], this.Price[i], this.Volume[i]);

            sr.Flush();
        }

        public void LoadStream(Stream stream)
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
                        var pipFactor = int.Parse(ln.Split(';')[1].Split(':')[1]);
                        if (this.Symbol != symbol || this.PipFactor != pipFactor)
                            throw new FileFormatNotSupportedException(string.Format("Symbol or PipFactor not equals"));
                        isHead1Found = !isHead1Found;
                    }
                    else if (!isHead2Found)
                    {
                        if (ln != "Time;Price;Volume;")
                            throw new FileFormatNotSupportedException(string.Format("All Lines: {0}, Error Line: {1}", lines, lines[i]));
                        isHead2Found = !isHead2Found;
                    }

                    if (isHead1Found && isHead2Found)
                        break;
                }

                if (!isHead1Found || !isHead2Found)
                    throw new FileFormatNotSupportedException(string.Format("Head not found"));

                for (int j = i + 1; j < lines.Count; j++)
                {
                    var line = lines[j];
                    if (string.IsNullOrEmpty(line)) continue;
                    var ln = Regex.Replace(line, @"\s+", "");
                    var segments = ln.Split(';');
                    this.Add(long.Parse(segments[0]), double.Parse(segments[1]), double.Parse(segments[2]));
                }
                return;
            }
            catch (Exception e)
            {
                this.Clear();
                throw new FileFormatNotSupportedException("file format not supported", e);
            }
        }

        static public IQuoteCapture Compress(IQuoteCapture qc)
        {
            if (qc == null)
                throw new ArgumentNullException();

            IQuoteCapture result = new QuoteCapture(qc.Symbol);
            if (qc.Count == 0)
                return result;


            //此时qc有值 先插入一个数据
            result.Add(qc.Time[0], qc.Price[0]);
            bool tail = false;
            for (int i = 1; i < qc.Count; i++)
            {
                var time = qc.Time[i];
                var price = qc.Price[i];

                var lastTime = qc.Time[i - 1];
                var lastPrice = qc.Price[i - 1];

                if (time - lastTime == MIN_INTERVAL && price == lastPrice)
                {//价格重复且时间递增那么该数据可以压缩
                    tail = true;
                }
                else
                {//本次数据不能被压缩那么把该数据当作分段新的起始点
                    //添加上一个分段的末尾如果分段数据只有1个那么不需要添加结尾
                    //[start: price, start+1:price,..., end:price]=>[start : price, end:-1]
                    if (tail)
                        result.Add(lastTime, NO_USED_PRICE);
                    //分段新的起始点
                    result.Add(time, price);
                    tail = false;
                }
            }
            //可能qc末尾是个分段但是此时还没能添加分段尾
            if (tail)
                result.Add(qc.Time.Last(), NO_USED_PRICE);

            return result;
        }

        static public IQuoteCapture Uncompress(IQuoteCapture qc)
        {
            if (qc == null)
                throw new ArgumentNullException();

            IQuoteCapture result = new QuoteCapture(qc.Symbol);
            if (qc.Count == 0)
                return result;

            //qc中至少有1个数据
            bool isSegment = false;
            result.Add(qc.FirstTime, qc.Price[0]);

            //true则认为此时在处理分段 每处理完一次分段时置为false
            isSegment = true;
            for (var i = 1; i < qc.Count; ++i)
            {//
             //通常NO_USED_PRICE不会被使用 

                if (qc.Price[i] == NO_USED_PRICE && isSegment)
                {//这是重复数据 要展开
                    var endTime = qc.Time[i];
                    var endPrice = qc.Price[i];
                    var startTime = qc.Time[i - 1];
                    var startPrice = qc.Price[i - 1];

                    var j = startTime + MIN_INTERVAL;
                    //用startPrice生成start到end之间的数据
                    while (j < endTime)
                    {
                        result.Add(j, startPrice);
                        j += MIN_INTERVAL;
                    }
                    //生成end数据(endtime 大则代表是正确的分段 小则意味不正确的分段 插入原来的数据
                    result.Add(endTime, endTime > startTime ? startPrice : endPrice);
                    //分段解压缩结束 
                    isSegment = false;
                }
                else//非重复数据或者数据起始点 直接插入
                {
                    result.Add(qc.Time[i], qc.Price[i]);
                    isSegment = true;
                }

            }
            return result;
        }

        //QSort快速排序
        private void QSort(bool isIncrease)
        {
            this.QSort(isIncrease, 0, this.Count - 1);
        }

        private void QSort(bool isIncrease, int start, int end)
        {//
            if (start < 0 || end >= this.Time.Count || start > end)
                throw new IndexOutOfRangeException(string.Format("Index:{0}_{1}", start, end));
            if (start == end)
                return;
            //选择基准点
            long basePoint = Time[start];
            var i = start;
            var j = end;
            while (i < j)
            {
                if (isIncrease ? Time[j] > basePoint : Time[j] < basePoint)
                    j--;
                else
                {
                    if (isIncrease ? Time[i] <= basePoint : Time[i] >= basePoint)
                        i++;
                    else
                        Swap(i, j);
                }
            }
            //此时i == j
            //交换start 和 i
            Swap(start, i);
            if (start < i - 1)
                this.QSort(isIncrease, start, i - 1);
            if (i + 1 < end)
                this.QSort(isIncrease, i + 1, end);
        }

        private void Swap(int index, int otherIndex)
        {
            if (index < 0 || this.Count <= index || otherIndex < 0 || this.Count <= otherIndex)
                throw new IndexOutOfRangeException(string.Format("swap:{0}_{1}", index, otherIndex));

            if (index == otherIndex)
                return;
            //交换时间
            var cacheTime = Time[index];
            Time[index] = Time[otherIndex];
            Time[otherIndex] = cacheTime;
            //交换价格
            var cachePrice = Price[index];
            Price[index] = Price[otherIndex];
            Price[otherIndex] = cachePrice;
            //交换交易量
            var cacheVolumn = Volume[index];
            Volume[index] = Volume[otherIndex];
            Volume[otherIndex] = cacheVolumn;
        }

        public void Add(long t, double p, double v)
        {
            Time.Add(t);
            Price.Add(p);
            Volume.Add(v);
            DataAdded?.Invoke(this, Symbol, t, p, v);
        }
    }

    ///// <summary>
    ///// class used for store quotes captured from screen
    ///// </summary>
    //[DataContract]
    //public class QuoteCapture : IQuoteCapture
    //{
    //    public int Count { get { return Time?.Count ?? 0; } }
    //    public long FirstTime { get { return (Time == null || Time.Count <= 0) ? 0 : Time.FirstOrDefault(); } }
    //    public long LastTime { get { return (Time == null || Time.Count <= 0) ? 0 : Time.LastOrDefault(); } }

    //    [DataMember]
    //    public string Symbol { get; private set; }
    //    [DataMember]
    //    public double PipFactor { get; private set; }
    //    [DataMember]
    //    public List<double> Price { get; private set; }
    //    [DataMember]
    //    public List<long> Time { get; private set; }

    //    public QuoteCapture()
    //    {
    //        Symbol = null;
    //        PipFactor = 1;
    //        Time = new List<long>();
    //        Price = new List<double>();
    //    }
    //    public QuoteCapture(string symbol) : this()
    //    {
    //        Symbol = symbol;
    //    }
    //    public QuoteCapture(IQuoteCapture q)
    //    {
    //        Create(q);
    //    }

    //    [JsonConstructor]
    //    public QuoteCapture(string Symbol, double PipFactor, IList<long> Time, IList<double> Price)
    //    {
    //        this.Create(Symbol, PipFactor, Time, Price);
    //    }

    //    public event EventHandlers.BasicQuoteDataAddedEventHandler DataAdded;

    //    private void Create(IQuoteCapture q)
    //    {
    //        if (q != null)
    //            this.Create(q.Symbol, q.PipFactor, q.Time, q.Price);
    //    }

    //    private void Create(string symbol, double pipFactor, IList<long> time, IList<double> price)
    //    {
    //        this.Symbol = symbol;
    //        this.PipFactor = pipFactor;
    //        this.Time = new List<long>(time);
    //        this.Price = new List<double>(price);
    //    }
    //    public void Assign(IQuoteCapture q)
    //    {
    //        this.Create(q);
    //    }
    //    public void Add(long t, double p)
    //    {
    //        Time.Add(t);
    //        Price.Add(p);
    //        DataAdded?.Invoke(this, Symbol, t, p);
    //    }
    //    public virtual void Add(long time, double buyPrice, double sellPrice)
    //    {
    //        this.Add(time, (buyPrice + sellPrice) / 2.0);
    //    }
    //    public virtual void Append(IQuoteCapture q)
    //    {
    //        if (q == null || q.Count <= 0 || this.Symbol != q.Symbol)
    //            return;

    //        var sindex = q.Count - 1;
    //        while (sindex >= 0 && q.Time[sindex] > this.LastTime) --sindex;

    //        if (sindex == -1) {
    //            this.Time.AddRange(q.Time);
    //            this.Price.AddRange(q.Price);
    //        }
    //        else if (sindex < q.Count - 1) {
    //            ++sindex;
    //            this.Time.AddRange(q.Time.GetRange(sindex, q.Count - sindex));
    //            this.Price.AddRange(q.Price.GetRange(sindex, q.Count - sindex));
    //        }
    //    }
    //    public virtual IQuoteCapture Extract(long stime, long etime)
    //    {
    //        int sIndex = General.BinarySearch(Time, 0, Time.Count - 1, stime, true);

    //        if (sIndex < 0) {
    //            sIndex++;
    //            if (Time.Count == 0 || Time[sIndex] < stime) {
    //                return null;
    //            }
    //        }
    //        else if (Time[sIndex] != stime) {
    //            sIndex++;
    //            if (sIndex >= Time.Count - 1) {
    //                return null;
    //            }
    //        }

    //        int eIndex = General.BinarySearch(Time, 0, Time.Count - 1, etime, true);

    //        return Extract(sIndex, eIndex);
    //    }
    //    public virtual IQuoteCapture Extract(int sindex, int eindex)
    //    {
    //        if (sindex < 0 || eindex == -1 || eindex > this.Count - 1 || eindex < sindex)
    //            return null;
    //        int num = eindex - sindex + 1;
    //        var quote = new QuoteCapture(Symbol);
    //        quote.Time.AddRange(Time.GetRange(sindex, num));
    //        quote.Price.AddRange(Price.GetRange(sindex, num));
    //        return quote;
    //    }
    //}
}
