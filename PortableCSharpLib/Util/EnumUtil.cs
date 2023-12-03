using System;

namespace PortableCSharpLib.Util
{
    public static class UtilExtension
    {
        public static T ParseEnum<T>(this string value, T defaultValue) where T : struct
        {
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            T result;
            return Enum.TryParse<T>(value, true, out result) ? result : defaultValue;
        }
    }
}
