using System.Collections.Generic;
using System.IO;

namespace PortableCSharpLib.TechnicalAnalysis
{
    public interface IQuoteCapture
    {
        int Count { get; }
        long FirstTime { get; }
        long LastTime { get; }
        string Symbol { get; }
        List<long> Time { get; }
        List<double> Price { get; }
        List<double> Volume { get; }
        double PipFactor { get; }
        /// <summary>
        /// 使用参数提供的内容填充结构
        /// </summary>
        /// <param name="q"></param>
        void Assign(IQuoteCapture q);
        /// <summary>
        /// 添加数据，但不触发DataAdded Event。
        /// </summary>
        /// <param name="q"></param>
        void Append(IQuoteCapture q);
        /// <summary>
        /// 添加数据并触发DataAdded Event。
        /// </summary>
        /// <param name="time"></param>
        /// <param name="price"></param>
        void Add(long time, double price);
        /// <summary>
        /// 添加数据并触发DataAdded Event。
        /// </summary>
        /// <param name="time"></param>
        /// <param name="price"></param>
        /// <param name="volumn"></param>
        void Add(long time, double price, double volumn);
        /// <summary>
        /// 添加数据并检查时间如果时间小于等于LastTime直接丢弃 成功则触发DataAdded Event。
        /// </summary>
        /// <param name="time"></param>
        /// <param name="price"></param>
        void AddMonotonic(long time, double price);
        /// <summary>
        /// 添加数据并检查时间如果时间小于等于LastTime直接丢弃 成功则触发DataAdded Event。
        /// </summary>
        /// <param name="time"></param>
        /// <param name="price"></param>
        void AddMonotonic(long time, double price, double volumn);
        /// <summary>
        /// 对内部数据进行排序
        /// </summary>
        /// <param name="isDistinct">true去除重复数据</param>
        /// <param name="isIncrease">true递增 false递减</param>
        void Sort(bool isDistinct = true, bool isIncrease = true);
        /// <summary>
        /// 提取在[sindex, eindex]中的股票
        ///   /// Exceptions
        /// 1. ArgumentException: 参数错误时抛出
        /// </summary>
        /// <param name="sindex">start index</param>
        /// <param name="eindex">end index</param>
        /// <returns></returns>
        IQuoteCapture Extract(int sindex, int eindex);
        /// <summary>
        /// 提取包含在[stime, etime]中的quote。
        /// </summary>
        /// <param name="stime">start time</param>
        /// <param name="etime">end time</param>
        /// <returns>如果[stime, etime]之间不包含值，返回空，否则返回IQuoteBasic实例</returns>
        IQuoteCapture Extract(long stime, long etime);

        /// <summary>
        /// Event when new data is added
        /// </summary>
        event EventHandlers.QuoteCaptureDataAddedEventHandler DataAdded;

        /// <summary>
        ///  Clear Time, Price
        /// </summary>
        void Clear();

        /// <summary>
        /// clear [stime, etime]
        /// </summary>
        /// <param name="stime"></param>
        /// <param name="etime"></param>
        void Clear(long stime, long etime);

        /// <summary>
        /// clear [sindex, eindex]
        ///   /// Exceptions
        /// 1. ArgumentException: 参数错误时抛出
        /// </summary>
        /// <param name="sindex"></param>
        /// <param name="eindex"></param>
        void Clear(int sindex, int eindex);



        /// <summary>
        /// Append QuoteCapture to stream
        /// 调用者负责关闭stream 
        /// </summary>
        /// <param name="stream"></param>
        void AppendStream(Stream stream);

        /// <summary>
        /// 从stream中读取数据，以填充QuoteCapture
        /// 调用者负责关闭stream 
        /// Exceptions: 
        /// 1. FileFormatNotSupportedException 当文件格式错误时抛出
        /// </summary>
        /// <param name="stream"></param>
        void LoadStream(Stream stream);
    }

    ///// <summary>
    ///// data stucture for storing sampling data
    ///// </summary>
    //public interface IQuoteCapture
    //{
    //    int Count { get; }
    //    long FirstTime { get; }
    //    long LastTime { get; }
    //    string Symbol { get; }
    //    List<long> Time { get; }
    //    List<double> Price { get; }
    //    double PipFactor { get; }
    //    /// <summary>
    //    /// 使用参数提供的内容填充结构
    //    /// </summary>
    //    /// <param name="q"></param>
    //    void Assign(IQuoteCapture q);
    //    /// <summary>
    //    /// 添加数据，但不触发DataAdded Event。
    //    /// </summary>
    //    /// <param name="q"></param>
    //    void Append(IQuoteCapture q);
    //    /// <summary>
    //    /// 添加数据并触发DataAdded Event。
    //    /// </summary>
    //    /// <param name="time"></param>
    //    /// <param name="price"></param>
    //    void Add(long time, double price);
    //    /// <summary>
    //    /// 提取在[sindex, eindex]中的股票
    //    /// </summary>
    //    /// <param name="sindex">start index</param>
    //    /// <param name="eindex">end index</param>
    //    /// <returns></returns>
    //    IQuoteCapture Extract(int sindex, int eindex);
    //    /// <summary>
    //    /// 提取包含在[stime, etime]中的quote。
    //    /// </summary>
    //    /// <param name="stime">start time</param>
    //    /// <param name="etime">end time</param>
    //    /// <returns></returns>
    //    IQuoteCapture Extract(long stime, long etime);

    //    /// <summary>
    //    /// Event when new data is added
    //    /// </summary>
    //    event EventHandlers.BasicQuoteDataAddedEventHandler DataAdded;
    //}
}
