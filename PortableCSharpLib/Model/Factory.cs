using PortableCSharpLib.Interace;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortableCSharpLib.Model
{
    public class Factory<T> : IFactory<T>
    {
        private readonly Dictionary<string, T> _clients = new Dictionary<string, T>();

        public void Register(string name, T service)
        {
            if (!_clients.ContainsKey(name))
                _clients.Add(name, service);
        }

        public T Resolve(string name)
        {
            if (_clients.ContainsKey(name)) return _clients[name];
            return default(T);
        }
    }
}
