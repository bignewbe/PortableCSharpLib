using System.Collections.Generic;
using System.IO;

namespace PortableCSharpLib.TechnicalAnalysis
{
    /// <summary>
    /// interface for basic quote info
    /// </summary>
    public interface IQuoteBasicBase
    {
        int Count { get; }
        string Symbol { get; }
        int Interval { get; }
        long FirstTime { get; }
        long LastTime { get; }
        string QuoteID { get; }

        /// <summary>
        /// 已非递减排序
        /// </summary>
        List<long> Time { get; }
        List<double> Open { get; }
        List<double> Close { get; }
        List<double> Low { get; }
        List<double> High { get; }
        List<double> Volume { get; }

        /// <summary>
        /// Clear internal data structure Time, Open, Close, Low, High, Volume
        /// </summary>
        void Clear();

        ///// <summary>
        ///// clear [stime, etime]
        ///// </summary>
        ///// <param name="stime"></param>
        ///// <param name="etime"></param>
        //void Clear(long stime, long etime);

        /// <summary>
        /// clear [sindex, eindex]
        /// Exceptions
        /// 1. IndexOutOfRangeException: index错误时抛出
        /// </summary>
        /// <param name="sindex"></param>
        /// <param name="eindex"></param>
        void Clear(int sindex, int eindex);

        /// <summary>
        /// Append quote to the end. If subInterval not specified, all elements added must be multiple of the existing interval.
        /// Otherwise, the last element can be added when it is multiple of subInterval. For example, if original interval is one hour
        /// and subInterval is 15 minutes, we are allowed to add the last element if it is multiple of 15, i.e., 15, 30 and 45.
        /// 不完整的值（即不是Interval的整数倍）最多只能有一个，且在序列的尾部。在添加完整的值或不完整的值时，都会删除之前不完整的值。
        /// </summary>
        /// <param name="q">内部数据应该以非递减排序</param>
        /// <param name="isFillGap">indicate whether to fill missing data</param>
        /// <returns>成功添加的数量，包含fillGap的数量</returns>
        int Append(IQuoteCapture q, bool isTriggerDataUpdated = false);

        /// <summary>
        /// Append quote to the end. If subInterval not specified, all elements added must be multiple of the existing interval.
        /// Otherwise, the last element can be added when it is multiple of subInterval. For example, if original interval is one hour
        /// and subInterval is 15 minutes, we are allowed to add the last element if it is multiple of 15, i.e., 15, 30 and 45.
        /// 不完整的值（即不是Interval的整数倍）最多只能有一个，且在序列的尾部。在添加完整的值或不完整的值时，都会删除之前不完整的值.
        /// fillGap添加的数据是之前值的副本
        /// </summary>
        /// <param name="q">内部数据应该以非递减排序, 不能包含不完整数据</param>
        /// <param name="subInterval"></param>
        /// <param name="isFillGap">indicate whether to fill missing data</param>
        /// <returns>成功添加的数量，包括fillGap的数量</returns>
        int Append(IQuoteBasicBase q, bool isTriggerDataUpdated = false);

        /// <summary>
        /// Add single data point and raise event if indicated.
        /// 如果为空，则添加
        /// 如果不为空，只有大于this.LastTime时才添加
        /// 如果时间一样，更新最后一个数据
        /// </summary>
        /// <param name="t"></param>
        /// <param name="o"></param>
        /// <param name="h"></param>
        /// <param name="l"></param>
        /// <param name="c"></param>
        /// <param name="v"></param>
        /// <param name="isTriggerDataAdded">是否触发事件</param>
        int AddUpdate(long t, double o, double h, double l, double c, double v, bool isTriggerDataAdded = false);

        /// <summary>
        /// event raised when Add is called with isTriggerDataAdded == true
        /// </summary>
        //event EventHandlers.QuoteBasicDataAddedEventHandler QuoteBasicDataAdded;
        //event EventHandlers.QuoteBasicDataAppendedEventHandler OnQuoteBasicDataAppended;

        /// <summary>
        /// 数据改变时触发的事件。Append触发。
        /// int表示改变的数量，如果int = 0，表示更新了数据，数量没变。
        /// </summary>
        event EventHandlers.DataAddedOrUpdatedEventHandler OnDataAddedOrUpdated;

        /// <summary>
        /// Find index for the given time. If isReturnJustSmallerElement == false, -1 will be returned if time is not found. 
        /// Otherwise, index of just smaller element will be returned. 
        /// </summary>
        /// <param name="time">time to search</param>
        /// <param name="isReturnJustSmallerElement">indicate whether to return the index with just smaller time</param>
        /// <returns>index of the found time，if not found, return -1</returns>
        int FindIndexForGivenTime(long time, bool isReturnJustSmallerElement = false);

        ///// <summary>
        ///// Find index where the given time is located. For candlestick, each index represents the change (o, c, h, l) of the price over 
        ///// the time period within (index * (interval-1), index * interval]. This function return the index for any given time, 
        ///// where the time is located. 
        ///// </summary>
        ///// <param name="time"></param>
        ///// <returns>如果找到返回索引，未找到返回-1</returns>
        //int FindIndexWhereTimeLocated(long time);

        /// <summary>
        /// Extract quote located within [stime, etime].
        /// </summary>
        /// <param name="stime"></param>
        /// <param name="etime"></param>
        /// <returns>如果[stime, etime]之间不包含值，返回空，否则返回IQuoteBasic实例</returns>
        QuoteBasicBase Extract(long stime, long etime);

        /// <summary>
        /// Extract quote within [sindex, eindex]
        /// Exceptions
        /// 1. ArgumentException: 参数错误时抛出
        /// </summary>
        /// <param name="sindex"></param>
        /// <param name="eindex"></param>
        /// <returns>返回IQuoteBasic实例</returns>
        QuoteBasicBase Extract(int sindex, int eindex);

        ///// <summary>
        ///// Insert quote before the beginning(q.LastTime < this.FirstTime)
        ///// or after the end( q.FirstTime > this.LastTime) 
        ///// or in the middle(q.FirstTime < this.FirstTime && q.LastTime < this.LastTime). 
        ///// Symbol和Interval必须相同
        ///// </summary>
        ///// <param name="q"></param>
        ///// <returns>true表示插入成功，false表示插入失败</returns>
        //bool Insert(IQuoteBasic q);

        /// <summary>
        /// Append QuoteBasic to stream
        /// 调用者负责关闭stream 
        /// </summary>
        /// <param name="stream"></param>
        void AppendStream(Stream stream);

        /// <summary>
        /// 从stream中读取数据，以填充QuoteBasic. 不会清除QuoteBasic中原先存在的数据
        /// stream数据的symbol和interval必须与this的相同。
        /// 调用者负责关闭stream 
        /// Exceptions: 
        /// 1. FileFormatNotSupportedException 当文件格式错误时抛出
        /// </summary>
        /// <param name="stream"></param>
        void LoadStream(Stream stream);
    }
}
