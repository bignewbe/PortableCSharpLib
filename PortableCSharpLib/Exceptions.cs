using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortableCSharpLib
{
    public class FileFormatNotSupportedException : Exception
    {
        public FileFormatNotSupportedException(string msg) : base(msg)
        { }
        public FileFormatNotSupportedException(string msg, Exception e) : base(msg, e)
        { }
    }
}
