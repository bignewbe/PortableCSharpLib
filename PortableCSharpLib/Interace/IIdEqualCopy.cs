using System;

namespace PortableCSharpLib.Interace
{
    public interface IIdEqualCopy<T> : IEquatable<T> where T : class
    {
        string Id { get; }
        void Copy(T other);
    }
}
