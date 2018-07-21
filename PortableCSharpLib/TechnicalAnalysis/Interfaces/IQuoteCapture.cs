using System.Collections.Generic;

namespace PortableCSharpLib.TechnicalAnalysis
{
    /// <summary>
    /// data stucture for storing sampling data
    /// </summary>
    public interface IQuoteCapture
    {
        int Count { get; }
        long FirstTime { get; }
        long LastTime { get; }
        string Symbol { get; }
        List<long> Time { get; }
        List<double> Price { get; }
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
        /// 提取在[sindex, eindex]中的股票
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
        /// <returns></returns>
        IQuoteCapture Extract(long stime, long etime);

        /// <summary>
        /// Event when new data is added
        /// </summary>
        event EventHandlers.BasicQuoteDataAddedEventHandler DataAdded;
    }
}
