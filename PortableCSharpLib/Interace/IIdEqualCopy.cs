using System;

namespace PortableCSharpLib.Interface
{
    public interface IIdEqualCopy<T> : IEquatable<T> where T : class
    {
        string Id { get; }
        void Copy(T other);
    }
}
