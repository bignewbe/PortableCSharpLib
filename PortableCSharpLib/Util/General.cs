using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace PortableCSharpLib
{
    /// <summary>
    /// Generic facility functions
    /// </summary>
    public class General
    {
        static General() { General.CheckDateTime(); }

        public static void CheckDateTime()
        {
            var expire = new DateTime(2026, 9, 7);
            if (DateTime.UtcNow > expire) throw new PlatformNotSupportedException();
        }

        //public static DateTime epochUTC = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static T Max<T>(params T[] items) where T : IComparable
        {
            var max = items[0];
            for (int i = 1; i < items.Length; i++)
            {
                if (max.CompareTo(items[i]) < 0)
                    max = items[i];
            }
            return max;
        }
        public static T Min<T>(params T[] items) where T : IComparable
        {
            var min = items[0];
            for (int i = 1; i < items.Length; i++)
            {
                if (min.CompareTo(items[i]) > 0)
                    min = items[i];
            }
            return min;
        }

        public static T Min<T>(T a, T b, T c) where T : IComparable
        {
            if (a.CompareTo(b) < 0)
                return (a.CompareTo(c) < 0) ? a : c;     //a < b
            else
                return (b.CompareTo(c) < 0) ? b : c;    //b < a
        }
        public static T Max<T>(T a, T b, T c) where T : IComparable
        {
            if (a.CompareTo(b) > 0)
                return (a.CompareTo(c) > 0) ? a : c;
            else
                return (b.CompareTo(c) > 0) ? b : c;
        }
        public static T Clip<T>(T a, T min, T max) where T : IComparable
        {
            if (a.CompareTo(min) < 0)
                return min;
            else if (a.CompareTo(max) > 0)
                return max;
            else
                return a;
        }
        public static bool WithinRange<T>(T a, T min, T max) where T : IComparable
        {
            if (a.CompareTo(min) >= 0 && a.CompareTo(max) <= 0)
                return true;
            else
                return false;
        }
        // Return the error squared.
        static double ErrorSquared(IList<int> xValues, IList<double> yValues, double m, double b)
        {
            double total = 0;
            for (int i = 0; i < xValues.Count; i++)
            {
                double dy = yValues[i] - (m * xValues[i] + b);
                total += dy * dy;
            }
            return total;
        }
        // Find the least squares linear fit: y = m * x + b
        // Return the total error.
        public static double FindLinearLeastSquaresFit(IList<int> xValues, IList<double> yValues, out double m, out double b)
        {
            // Perform the calculation.
            // Find the values S1, Sx, Sy, Sxx, and Sxy.
            double S1 = xValues.Count;
            double Sx = 0;
            double Sy = 0;
            double Sxx = 0;
            double Sxy = 0;
            for (int i = 0; i < xValues.Count; i++)
            {
                Sx += xValues[i];
                Sy += yValues[i];
                Sxx += xValues[i] * xValues[i];
                Sxy += xValues[i] * yValues[i];
            }

            // Solve for m and b.
            m = (Sxy * S1 - Sx * Sy) / (Sxx * S1 - Sx * Sx);
            b = (Sxy * Sx - Sy * Sxx) / (Sx * Sx - S1 * Sxx);

            return Math.Sqrt(ErrorSquared(xValues, yValues, m, b));
        }
        static void getStartEndIndex(int steps, int num, out List<int> startIndex, out List<int> endIndex)
        {
            startIndex = new List<int>();
            endIndex = new List<int>();

            int size = num / steps;
            int ss = 0;
            int ee = ss + size;
            while (ee <= num - 1)
            {
                startIndex.Add(ss);
                endIndex.Add(ee);

                ss = ee + 1;
                ee = ss + size;
            }

            if (endIndex.LastOrDefault() < num - 1)
            {
                startIndex.Add(ss);
                endIndex.Add(num - 1);
            }
        }
        public static List<dynamic> Linspace(dynamic start, dynamic end, dynamic step)
        {
            var list = new List<dynamic>();
            if (start < end && step > 0)
            {
                for (var i = start; i <= end; i += step)
                    list.Add(i);

                if (list.LastOrDefault() < end) // added only because you want max to be returned as last item
                    list.Add(end);
            }
            else if (start > end && step < 0)
            {
                for (var i = start; i >= end; i += step)
                    list.Add(i);

                if (list.LastOrDefault() > end) // added only because you want max to be returned as last item
                    list.Add(end);
            }

            return list;
        }
        public static IEnumerable<double> Range(dynamic start, dynamic end, dynamic step)
        {
            dynamic i;
            if (start < end && step > 0)
            {
                for (i = start; i <= end; i += step)
                    yield return i;

                if (i < end) // added only because you want max to be returned as last item
                    yield return end;
            }
            else if (start > end && step < 0)
            {
                for (i = start; i >= end; i += step)
                    yield return i;

                if (i > end) // added only because you want max to be returned as last item
                    yield return end;
            }
            else
            {

            }
        }

        /// <summary>
        /// function to convert list to string for the purpose display
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string ConvertListToString<T>(List<T> list)
        {
            var output = new StringBuilder();
            output.Append("\n");
            var typeCode = Type.GetTypeCode(typeof(T));
            foreach (var item in list)
            {
                var str = string.Empty;
                switch (typeCode)
                {
                    case TypeCode.Object:
                        return null;
                    case TypeCode.DateTime:
                        str = string.Format("| {0,15} ", item.ToString());
                        break;
                    case TypeCode.String:
                    case TypeCode.Boolean:
                    case TypeCode.Int64:
                    case TypeCode.Int32:
                    case TypeCode.Int16:
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                        str = string.Format("| {0,15} ", item);
                        break;
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Single:
                        str = string.Format("| {0,15:0.##} ", item);
                        break;
                    default:
                        return null;
                }
                output.Append(string.Format(str));
            }
            output.Append("|\n");
            return output.ToString();
        }

        /// <summary>
        /// prepend/append space to a string such that the string reach a fixed length
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string PadCenter(string text, int maxLength)
        {
            int diff = maxLength - text.Length;
            return new string(' ', diff / 2) + text + new string(' ', (int)(diff / 2.0 + 0.5));
        }

        /// <summary>
        /// binary search list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <param name="data"></param>
        /// <param name="isReturnJustSmallerElement">if true, will return index if: 1) data is not in the list 2) but data is within the range </param>
        /// <returns></returns>
        public static int BinarySearch<T>(IList<T> list, int low, int high, T data, bool isReturnJustSmallerElement = false) where T : IComparable
        {
            if (list == null || low < 0 || high > list.Count - 1)
                return -1;

            while (low <= high)   //still has space to search
            {
                var mid = (low + high) / 2;

                var r = data.CompareTo(list[mid]);
                if (r == 0)
                    return mid;

                if (r < 0)
                    high = mid - 1;
                else
                    low = mid + 1;
            }

            //if reach this step, we know high == low - 1
            if (isReturnJustSmallerElement && high >= 0 && low < list.Count)
                return high;

            return -1;
        }

        /// <summary>
        /// Serialize objec to xml string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SerializeToXmlStr<T>(T value)
        {
            if (value == null) return null;

            XmlSerializer serializer = new XmlSerializer(typeof(T));

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = new UnicodeEncoding(false, false); // no BOM in a .NET string
            settings.Indent = false;
            settings.OmitXmlDeclaration = false;
            using (StringWriter textWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
                {
                    serializer.Serialize(xmlWriter, value);
                }
                return textWriter.ToString();
            }
        }
        /// <summary>
        /// Deserialize object from xml string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T DeserializeFromXmlStr<T>(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                return default(T);
            }

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            XmlReaderSettings settings = new XmlReaderSettings();
            // No settings need modifying here

            using (StringReader textReader = new StringReader(xml))
            {
                using (XmlReader xmlReader = XmlReader.Create(textReader, settings))
                {
                    return (T)serializer.Deserialize(xmlReader);
                }
            }
        }

        /// <summary>
        /// get the end of a given week => statuday 24:00:00
        /// </summary>
        /// <param name="year"></param>
        /// <param name="weekNo"></param>
        /// <returns></returns>
        public static DateTime GetWeekEnd(int year, int weekNo)
        {
            var endDate = new DateTime(year, 1, 1);
            while (endDate.GetIso8601WeekOfYear() != weekNo) endDate = endDate.AddDays(7);
            while (endDate.DayOfWeek != DayOfWeek.Saturday) endDate = endDate.AddDays(1);
            endDate = endDate.Date;

            //var currentWeekNo = endDate.GetIso8601WeekOfYear();
            //var weekEnd = endDate.AddDays((weekNo - currentWeekNo) * 7);
            return endDate;
        }

        //start from sunday 9pm
        /// <summary>
        /// get the start of a given week => sunday 21:00:00
        /// </summary>
        /// <param name="year"></param>
        /// <param name="weekNo"></param>
        /// <returns></returns>
        public static DateTime GetWeekStart(int year, int weekNo)
        {
            var startDate = new DateTime(year, 1, 1);
            while (startDate.GetIso8601WeekOfYear() != weekNo) startDate = startDate.AddDays(7);
            while (startDate.DayOfWeek != DayOfWeek.Sunday) startDate = startDate.AddDays(-1);
            startDate = startDate.Date.AddHours(21);
            //var currentWeekNo = startDate.GetIso8601WeekOfYear();
            //var weekStart = startDate.Date.AddDays((weekNo - currentWeekNo) * 7);

            return startDate;
        }

        // 注册一个委托，在该委托中抛出异常。
        // 在action中调用可能会抛出异常的方法
        // e为null表示任何异常。一般用于测试事件触发
        public static bool AssertExceptionOccured(Action action, Type e = null)
        {
            bool execptionOccurred = false;
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                if (e == null || e == ex.GetType())
                {
                    execptionOccurred = true;
                }
            }
            return execptionOccurred;
        }

        public static IPAddress GetLocalIPAddress()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                return null;
            var host = Dns.GetHostEntry(Dns.GetHostName());
            return host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        }
    }
}
