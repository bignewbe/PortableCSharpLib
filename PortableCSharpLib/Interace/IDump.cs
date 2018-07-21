using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortableCSharpLib.Interace
{
    /// <summary>
    /// Interface to save and restore status of a class
    /// </summary>
    public interface IDump
    {
        void Dump();
        void Recover();
    }
}
