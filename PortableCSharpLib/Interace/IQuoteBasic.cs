using PortableCSharpLib.Interace;

namespace PortableCSharpLib.TechnicalAnalysis
{
    /// <summary>
    /// interface for basic quote info
    /// </summary>
    public interface IQuoteBasic:IQuoteBasicBase
    {
        /// <summary>
        /// clear [stime, etime]
        /// </summary>
        /// <param name="stime"></param>
        /// <param name="etime"></param>
        void Clear(long stime, long etime);

        /// <summary>
        /// Insert quote before the beginning(q.LastTime < this.FirstTime)
        /// or after the end( q.FirstTime > this.LastTime) 
        /// or in the middle(q.FirstTime < this.FirstTime && q.LastTime < this.LastTime). 
        /// Symbol和Interval必须相同
        /// </summary>
        /// <param name="q"></param>
        /// <returns>true表示插入成功，false表示插入失败</returns>
        bool Insert(IQuoteBasicBase q);

        /// <summary>
        /// Append quote to the end. If subInterval not specified, all elements added must be multiple of the existing interval.
        /// Otherwise, the last element can be added when it is multiple of subInterval. For example, if original interval is one hour
        /// and subInterval is 15 minutes, we are allowed to add the last element if it is multiple of 15, i.e., 15, 30 and 45.
        /// 不完整的值（即不是Interval的整数倍）最多只能有一个，且在序列的尾部。在添加完整的值或不完整的值时，都会删除之前不完整的值。
        /// </summary>
        /// <param name="q">内部数据应该以非递减排序</param>
        /// <param name="subInterval">sub interval</param>
        /// <param name="isFillGap">indicate whether to fill missing data</param>
        /// <returns>成功添加的数量，包含fillGap的数量</returns>
        int Append(IQuoteCapture q, int subInterval = -1, bool isFillGap = false, bool isTriggerDataUpdated = false);

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
        int Append(IQuoteBasicBase q, int subInterval = -1, bool isFillGap = false, bool isTriggerDataUpdated = false);

        /// <summary>
        /// Add single data point and raise event if indicated.
        /// 如果为空，则添加
        /// 如果不为空，只有大于this.LastTime时才添加
        /// </summary>
        /// <param name="t"></param>
        /// <param name="o"></param>
        /// <param name="h"></param>
        /// <param name="l"></param>
        /// <param name="c"></param>
        /// <param name="v"></param>
        /// <param name="isTriggerDataAdded">是否触发事件</param>
        void Add(long t, double o, double h, double l, double c, double v, bool isTriggerDataAdded = false);

        /// <summary>
        /// update the last data point and raise event if indicated
        /// </summary>
        /// <param name="t"></param>
        /// <param name="o"></param>
        /// <param name="h"></param>
        /// <param name="l"></param>
        /// <param name="c"></param>
        /// <param name="v"></param>
        void UpdateLast(long t, double o, double h, double l, double c, double v);

        /// <summary>
        /// event raised when Add is called with isTriggerDataAdded == true
        /// </summary>
        //event EventHandlers.QuoteBasicDataAddedEventHandler QuoteBasicDataAdded;
        //event EventHandlers.QuoteBasicDataAppendedEventHandler OnQuoteBasicDataAppended;

        ///// <summary>
        ///// 数据改变时触发的事件。Append触发。
        ///// int表示改变的数量，如果int = 0，表示更新了数据，数量没变。
        ///// </summary>
        ////event Action<IQuoteBasic, int> DataAddedOrUpdated;
        //event EventHandlers.DataAddedOrUpdatedEventHandler OnDataAddedOrUpdated;

        /// <summary>
        /// Find index where the given time is located. For candlestick, each index represents the change (o, c, h, l) of the price over 
        /// the time period within (index * (interval-1), index * interval]. This function return the index for any given time, 
        /// where the time is located. 
        /// </summary>
        /// <param name="time"></param>
        /// <returns>如果找到返回索引，未找到返回-1</returns>
        int FindIndexWhereTimeLocated(long time);
    }

    ///// <summary>
    ///// interface for basic quote info
    ///// </summary>
    //public interface IQuoteBasic
    //{
    //    int Count { get; }
    //    string Symbol { get; }
    //    int Interval { get; }
    //    long FirstTime { get; }
    //    long LastTime { get; }
    //    string QuoteID { get; }

    //    /// <summary>
    //    /// 已非递减排序
    //    /// </summary>
    //    List<long> Time { get; }
    //    List<double> Open { get; }
    //    List<double> Close { get; }
    //    List<double> Low { get; }
    //    List<double> High { get; }
    //    List<double> Volume { get; }

    //    /// <summary>
    //    /// Clear internal data structure Time, Open, Close, Low, High, Volume
    //    /// </summary>
    //    void Clear();
    //    /// <summary>
    //    /// Append quote to the end. If subInterval not specified, all elements added must be multiple of the existing interval.
    //    /// Otherwise, the last element can be added when it is multiple of subInterval. For example, if original interval is one hour
    //    /// and subInterval is 15 minutes, we are allowed to add the last element if it is multiple of 15, i.e., 15, 30 and 45.
    //    /// 不完整的值（即不是Interval的整数倍）最多只能有一个，且在序列的尾部。在添加完整的值或不完整的值时，都会删除之前不完整的值。
    //    /// </summary>
    //    /// <param name="q">quote to append</param>
    //    /// <param name="subInterval">sub interval</param>
    //    /// <param name="isFillGap">indicate whether to fill missing data</param>
    //    /// <returns>成功添加的数量</returns>
    //    int Append(IQuoteCapture q, int subInterval = -1, bool isFillGap = false);
    //    /// <summary>
    //    /// Append quote to the end. If subInterval not specified, all elements added must be multiple of the existing interval.
    //    /// Otherwise, the last element can be added when it is multiple of subInterval. For example, if original interval is one hour
    //    /// and subInterval is 15 minutes, we are allowed to add the last element if it is multiple of 15, i.e., 15, 30 and 45.
    //    /// 不完整的值（即不是Interval的整数倍）最多只能有一个，且在序列的尾部。在添加完整的值或不完整的值时，都会删除之前不完整的值.
    //    /// </summary>
    //    /// <param name="q"></param>
    //    /// <param name="subInterval"></param>
    //    /// <param name="isFillGap">indicate whether to fill missing data</param>
    //    /// <returns>成功添加的数量</returns>
    //    int Append(IQuoteBasic q, int subInterval = -1, bool isFillGap = false);
    //    /// <summary>
    //    /// Add single data point and raise event if indicated.
    //    /// </summary>
    //    /// <param name="t"></param>
    //    /// <param name="o"></param>
    //    /// <param name="h"></param>
    //    /// <param name="l"></param>
    //    /// <param name="c"></param>
    //    /// <param name="v"></param>
    //    /// <param name="isTriggerDataAdded">是否触发事件</param>
    //    void Add(long t, double o, double h, double l, double c, double v, bool isTriggerDataAdded = false);

    //    /// <summary>
    //    /// event raised when Add is called with isTriggerDataAdded == true
    //    /// </summary>
    //    event EventHandlers.QuoteBasicDataAddedEventHandler QuoteBasicDataAdded;

    //    /// <summary>
    //    /// Find index for the given time. If isReturnJustSmallerElement == false, -1 will be returned if time is not found. 
    //    /// Otherwise, index of just smaller element will be returned. 
    //    /// </summary>
    //    /// <param name="time">time to search</param>
    //    /// <param name="isReturnJustSmallerElement">indicate whether to return the index with just smaller time</param>
    //    /// <returns>index of the found time</returns>
    //    int FindIndexForGivenTime(long time, bool isReturnJustSmallerElement = false);
    //    /// <summary>
    //    /// Find index where the given time is located. For candlestick, each index represents the change (o, c, h, l) of the price over 
    //    /// the time period within (index * (interval-1), index * interval]. This function return the index for any given time, 
    //    /// where the time is located. 
    //    /// </summary>
    //    /// <param name="time"></param>
    //    /// <returns></returns>
    //    int FindIndexWhereTimeLocated(long time);
    //    /// <summary>
    //    /// Extract quote located within [stime, etime]
    //    /// </summary>
    //    /// <param name="stime"></param>
    //    /// <param name="etime"></param>
    //    /// <returns></returns>
    //    IQuoteBasic Extract(long stime, long etime);
    //    /// <summary>
    //    /// Extract quote within [sindex, eindex]
    //    /// </summary>
    //    /// <param name="sindex"></param>
    //    /// <param name="eindex"></param>
    //    /// <returns></returns>
    //    IQuoteBasic Extract(int sindex, int eindex);
    //    /// <summary>
    //    /// Insert quote before the beginning, after the end or in the middle. 
    //    /// </summary>
    //    /// <param name="q"></param>
    //    /// <returns></returns>
    //    bool Insert(IQuoteBasic q);
    //}
}
