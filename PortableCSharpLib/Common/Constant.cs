using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortableCSharpLib
{
    public class Constant
    {
        /// <summary>
        /// Define the origin of time
        /// </summary>
        public static DateTime EpochUTC = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }
}
