using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace PortableCSharpLib.Interface
{
    public interface IGenericProxy<T>
    {
        ConcurrentDictionary<string, T> Items { get; }
        T GetItemById(string id);

        void Start();
        void Stop();
        bool Enqueue(string symbol);
        void Dequeue(string symbol);
        void Update(params T[] items);

        event PortableCSharpLib.EventHandlers.ItemChangedEventHandler<T> OnItemAdded;
        event PortableCSharpLib.EventHandlers.ItemChangedEventHandler<T> OnItemUpdated;
        event PortableCSharpLib.EventHandlers.ItemChangedEventHandler<List<T>> OnItemListUpdated;
        event PortableCSharpLib.EventHandlers.ItemWithIdChangedEventHandler<string> OnExceptionOccured;
    }
}
