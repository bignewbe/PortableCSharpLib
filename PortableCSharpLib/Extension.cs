using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Globalization;
using PortableCSharpLib.DataType;
using PortableCSharpLib.Interace;

namespace PortableCSharpLib
{
    /// <summary>
    /// Extension methods that can be used across platforms
    /// </summary>
    public static class Extension
    {
        //public static void UpdateListViewData<T>(this ListViewData<T> data, T item) where T : class, IIdEqualCopy<T>
        //{
        //    var items = data.Items;
        //    var bindingData = data.BindingItems;
        //    var indiceToResetBinding = data.IndiceToResetBinding;
        //    var idToIndex = data.ItemIdToIndex;

        //    if (!idToIndex.ContainsKey(item.Id))
        //    {
        //        idToIndex.Add(item.Id, bindingData.Count);
        //        bindingData.Add((T)Activator.CreateInstance(typeof(T)));
        //    }

        //    var index = idToIndex[item.Id];
        //    if (!bindingData[index].Equals(item))
        //    {
        //        bindingData[index].Copy(item);
        //        indiceToResetBinding.Add(index);
        //        bindingData.ResetItem(index);
        //    }
        //}

        //public static void UpdateListViewData<T>(this ListViewData<T> data, params T[] items) where T : class, IIdEqualCopy<T>
        //{
        //    foreach (var item in items) data.AddUpdateItems(item);
        //    //data.UpdateBindingItems();

        //    var bindingData = data.BindingItems;
        //    var indiceToResetBinding = data.IndiceToResetBinding;
        //    var idToIndex = data.ItemIdToIndex;
        //    indiceToResetBinding.Clear();
        //    if (items.Length != bindingData.Count)
        //    {
        //        idToIndex.Clear();
        //        bindingData.Clear();
        //    }

        //    //update OpenOrders
        //    foreach (var item in items)
        //    {
        //        if (!idToIndex.ContainsKey(item.Id))
        //        {
        //            idToIndex.Add(item.Id, bindingData.Count);
        //            bindingData.Add((T)Activator.CreateInstance(typeof(T)));
        //        }

        //        var index = idToIndex[item.Id];
        //        if (!bindingData[index].Equals(item))
        //        {
        //            bindingData[index].Copy(item);
        //            indiceToResetBinding.Add(index);
        //        }
        //    }

        //    if (indiceToResetBinding.Count > 10)
        //    {
        //        bindingData.ResetBindings();
        //    }
        //    else
        //    {
        //        foreach (var i in indiceToResetBinding)
        //            bindingData.ResetItem(i);
        //    }
        //}

        public new static bool Equals(this object obj, object other)
        {
            if (other == null) return false;
            var properties = obj.GetType().GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToList();
            foreach (var p in properties)
            {
                var v1 = p.GetValue(obj);
                var v2 = p.GetValue(other);
                if (v1 == null && v2 == null) continue;
                if (v1 == null || v2 == null) return false;
                if (!v1.Equals(v2)) return false;
            }
            return true;
        }

        public static void Copy(this object obj, object other)
        {
            if (other == null) return;
            var properties = obj.GetType().GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToList();
            foreach (var p in properties)
            {
                var v2 = p.GetValue(other);
                p.SetValue(obj, v2);
            }
        }

        static Extension() { General.CheckDateTime(); }
        /// <summary>
        /// convert list to string for printing
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="width"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string ConvertListToString<T>(this IList<T> list, int width = 15, char separator = '|')
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
                        {
                            var format = string.Format("{0} {{0,{1}}} ", separator, width);
                            str = string.Format(format, item.ToString());
                            break;
                        }
                    case TypeCode.String:
                    case TypeCode.Boolean:
                    case TypeCode.Int64:
                    case TypeCode.Int32:
                    case TypeCode.Int16:
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                        {
                            var format = string.Format("{0} {{0,{1}}} ", separator, width);
                            str = string.Format(format, item);
                            break;
                        }
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Single:
                        {
                            var format = string.Format("{0} {{0,{1}:0.##}} ", separator, width);
                            //var format = string.Format("| {{0,{0}:0.##}} ", width);
                            str = string.Format(format, item);
                            break;
                        }
                    default:
                        return null;
                }
                //string str = "";
                //if (typeof(T) == typeof(double))
                //    str = string.Format("| {0,15:0.00} ", item);
                //else if (typeof(T) == typeof(DateTime))
                //    str = string.Format("| {0,15} ", item.ToString());
                //else if (typeof(T) == typeof(string))
                //    str = string.Format("{0}\n", item);
                //else
                //    break;
                output.Append(string.Format(str));
            }
            output.Append("|\n");

            return output.ToString();
        }

        /// <summary>
        /// Extension function to iterate each element of IEnumerable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ie"></param>
        /// <param name="action"></param>
        public static void Each<T>(this IEnumerable<T> ie, Action<T, int> action)
        {
            var i = 0;
            foreach (var e in ie) action(e, i++);
        }

        #region time manipulation
        /// <summary>
        /// convert time in seconds to universal time zone
        /// </summary>
        /// <param name="seconds">time in seconds since 1970/01/01 00:00:00</param>
        /// <returns></returns>
        public static DateTime GetUTCFromUnixTime(this long seconds)
        {
            return Constant.EpochUTC.AddSeconds(seconds);
        }
        /// <summary>
        /// convert 
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long GetUnixTimeFromUTC(this DateTime time)
        {
            return Convert.ToInt64((time - Constant.EpochUTC).TotalSeconds);      //note this round to the closest seconds
        }
        public static long GetMiliSecondsFromUTC(this DateTime time)
        {
            return Convert.ToInt64((time - Constant.EpochUTC).TotalMilliseconds);  //note this round to the closest seconds
        }
        public static DateTime GetUTCFromMiliSeconds(this long miliSeconds)
        {
            return Constant.EpochUTC.AddMilliseconds(miliSeconds);
        }
        /// <summary>
        /// create json date {Year, Month, Day}
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static object GetJsonFromUnixTime(this long time)
        {
            return time.GetUTCFromUnixTime().GetJsonFromUTC();
        }
        /// <summary>
        /// create json date {Year, Month, Day}
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static object GetJsonFromUTC(this DateTime time)
        {
            return new { Year = time.Year, Month = time.Month, Day = time.Day };
        }

        /// <summary>
        /// Get week number of given date
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int GetIso8601WeekOfYear(this DateTime time)
        {
            //always moves to the saturday and calculate the week number
            while (time.DayOfWeek != DayOfWeek.Saturday) time = time.AddDays(1);

            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            //DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            //if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            //    time = time.AddDays(3);

            // Return the week of our adjusted day
            var weekNo = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            //var year = time.Year;
            //var result = (year - 2000) * 100 + weekNo
            return weekNo;
        }

        /// <summary>
        /// get year of given date
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int GetIsoYear(this DateTime time)
        {
            //always moves to the saturday and calculate the week number
            while (time.DayOfWeek != DayOfWeek.Saturday) time = time.AddDays(1);
            return time.Year;
        }
        #endregion

        #region reflection
        /// <summary>
        /// get list of properties of an object which are writable
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<PropertyInfo> GetWritableProperties(this Type type)
        {
            return type
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) //BindingFlags.Static | 
                .Where(x => x.CanWrite)
                .ToList();
        }

        /// <summary>
        /// Get the value of a propery given by name
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static dynamic GetPropertyValueByName(this object obj, string propertyName)
        {
            var type = obj.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToList();
            if (properties == null) return null;

            var prop = properties.First(p => p.Name == propertyName);
            if (prop == null) return null;

            return prop.GetValue(obj, null);
        }

        /// <summary>
        /// Set the value of a propery given by name
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetPropertyValueByName(this object obj, string paramName, dynamic value)
        {
            var type = obj.GetType();
            var properties = type.GetWritableProperties();
            if (properties == null) return false;

            var prop = properties.First(p => p.Name == paramName);
            if (prop == null) return false;

            prop.SetValue(obj, value, null);
            return true;
        }

        /// <summary>
        /// Get attribute for given property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static T GetAttributeForProperty<T>(this object instance, string propertyName) where T : Attribute
        {
            var attrType = typeof(T);
            var property = instance.GetType().GetProperty(propertyName);
            return (T)property.GetCustomAttributes(attrType, false).First();
        }
        #endregion

        #region type manipulation
        /// <summary>
        /// Check if a type has implemented the given interface
        /// </summary>
        /// <param name="type"></param>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        public static bool HasInterface(this Type type, Type interfaceType)
        {
            var ifs = typeof(List<int>).GetInterfaces();
            if (ifs == null || ifs.Length <= 0)
                return false;
            return ifs.Contains(interfaceType);
        }
        /// <summary>
        /// Check if a type has implemented the given interface
        /// </summary>
        /// <param name="type"></param>
        /// <param name="interfaceName"></param>
        /// <returns></returns>
        public static bool HasInterface(this Type type, string interfaceName)
        {
            var ifs = type.GetInterfaces().ToList().FirstOrDefault(f => f.Name.Contains(interfaceName));
            return ifs != null;
        }
        /// <summary>
        /// check if a type is numeric
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNumericType(this Type type)
        {
            if (type.IsEnum) return false;
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// check if given type is a boolean
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsBoolean(this Type type)
        {
            return Type.GetTypeCode(type) == TypeCode.Boolean;
        }
        /// <summary>
        /// check if type is nullable
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        /// <summary>
        /// check if a type is string
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsString(this Type type)
        {
            return TypeCode.String == Type.GetTypeCode(type);
        }
        /// <summary>
        /// check if a type is DateTime
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsDateTime(this Type type)
        {
            return TypeCode.DateTime == Type.GetTypeCode(type);
        }
        /// <summary>
        /// check if a type is numeric of string
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNumericOrString(this Type type)
        {
            return type.IsNumericType() || type.IsString();
        }
        #endregion
    }
}
