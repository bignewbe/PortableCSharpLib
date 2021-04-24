using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortableCSharpLib
{
    public class MyException : Exception
    {
        public string Code { get; set; }
        //public string Message { get; set; }
        public MyException(string errorCode, string message) : base(message)
        {
            this.Code = errorCode;
            //this.Message = message;
        }
        public MyException(string code, string message, Exception innerException) : base(message, innerException)
        {
            this.Code = code;
            //this.Message = message;
        }
    }

    public class FileFormatNotSupportedException : Exception
    {
        public FileFormatNotSupportedException(string msg) : base(msg)
        { }
        public FileFormatNotSupportedException(string msg, Exception e) : base(msg, e)
        { }
    }
}
