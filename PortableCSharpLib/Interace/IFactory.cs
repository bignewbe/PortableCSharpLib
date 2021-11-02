using System;
using System.Collections.Generic;
using System.Text;

namespace PortableCSharpLib.Interace
{
    public interface IFactory<T>
    {
        void Register(string name, T service);
        T Resolve(string name);
    }
}
