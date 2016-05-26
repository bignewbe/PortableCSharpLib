using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortableCSharpLib.TechnicalAnalysis
{
    /// <summary>
    /// interface for basic quote info
    /// </summary>
    public interface IQuoteBasic
    {
        int Count { get; }
        string Symbol { get; }
        int Interval { get; }
        long FirstTime { get; }
        long LastTime { get; }
        string QuoteID { get; }

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
        /// <summary>
        /// Append quote to the end. If subInterval not specified, all elements added must be multiple of the existing interval.
        /// Otherwise, the last element can be added when it is multiple of subInterval. For example, if original interval is one hour
        /// and subInterval is 15 minutes, we are allowed to add the last element if it is multiple of 15, i.e., 15, 30 and 45.
        /// </summary>
        /// <param name="q">quote to append</param>
        /// <param name="subInterval">sub interval</param>
        /// <returns></returns>
        int Append(IQuoteCapture q, int subInterval = -1);
        /// <summary>
        /// Append quote to the end. If subInterval not specified, all elements added must be multiple of the existing interval.
        /// Otherwise, the last element can be added when it is multiple of subInterval. For example, if original interval is one hour
        /// and subInterval is 15 minutes, we are allowed to add the last element if it is multiple of 15, i.e., 15, 30 and 45.
        /// </summary>
        /// <param name="q"></param>
        /// <param name="subInterval"></param>
        /// <returns></returns>
        int Append(IQuoteBasic q, int subInterval = -1);
        /// <summary>
        /// Add single data point and raise event if indicated.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="o"></param>
        /// <param name="h"></param>
        /// <param name="l"></param>
        /// <param name="c"></param>
        /// <param name="v"></param>
        /// <param name="isTriggerDataAdded"></param>
        void Add(long t, double o, double h, double l, double c, double v, bool isTriggerDataAdded=false);

        /// <summary>
        /// event raised when Add is called with isTriggerDataAdded == true
        /// </summary>
        event EventHandlers.QuoteBasicDataAddedEventHandler QuoteBasicDataAdded;

        /// <summary>
        /// Find index for the given time. If isReturnJustSmallerElement == false, -1 will be returned if time is not found. 
        /// Otherwise, index of just smaller element will be returned. 
        /// </summary>
        /// <param name="time">time to search</param>
        /// <param name="isReturnJustSmallerElement">indicate whether to return the index with just smaller time</param>
        /// <returns>index of the found time</returns>
        int FindIndexForGivenTime(long time, bool isReturnJustSmallerElement=false);
        /// <summary>
        /// Find index where the given time is located. For candlestick, each index represents the change (o, c, h, l) of the price over 
        /// the time period within [index * (interval-1) + 1, index * interval]. This function return the index for any given time, 
        /// where the time is located. 
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        int FindIndexWhereTimeLocated(long time);
        /// <summary>
        /// Extract quote located within [stime, etime]
        /// </summary>
        /// <param name="stime"></param>
        /// <param name="etime"></param>
        /// <returns></returns>
        IQuoteBasic Extract(long stime, long etime);
        /// <summary>
        /// Extract quote within [sindex, eindex]
        /// </summary>
        /// <param name="sindex"></param>
        /// <param name="eindex"></param>
        /// <returns></returns>
        IQuoteBasic Extract(int sindex, int eindex);
        /// <summary>
        /// Insert quote before the beginning, after the end or in the middle. 
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        bool Insert(IQuoteBasic q);
    }
}
