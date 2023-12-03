using PortableCSharpLib.Interface;
using PortableCSharpLib.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace PortableCSharpLib.DataType
{
    public class ListViewData<T> : CINotification where T : class, IIdEqualCopy<T>
    {
        public List<int> IndiceToResetBinding { get; set; } = new List<int>();
        public Dictionary<string, int> ItemIdToIndex { get; set; } = new Dictionary<string, int>();
        public ConcurrentDictionary<string, T> Items { get; set; } = new ConcurrentDictionary<string, T>();
        BindingList<T> _BindingItems = new BindingList<T>();
        private bool _isResetBindingBusy = false;

        public BindingList<T> BindingItems { get { return _BindingItems; } private set { SetField(ref _BindingItems, value); } }

        private T GetDefaultValue(Type type)
        {
            return type.IsValueType ? (T)Activator.CreateInstance(type) : null;
        }

        public void AddUpdateItems(params T[] items)
        {
            if (items == null || items.Length == 0) return;
            foreach (var item in items)
            {
                if (!this.Items.ContainsKey(item.Id))
                    this.Items.TryAdd(item.Id, (T)Activator.CreateInstance(typeof(T), item));
                else
                {
                    if (this.Items[item.Id] == null)
                        this.Items[item.Id] = (T)Activator.CreateInstance(typeof(T), item);
                    else if (!this.Items[item.Id].Equals(item))
                        this.Items[item.Id].Copy(item);
                }
            }
        }
                
        public void ResetItermAndBinding(params T[] items)
        {
            //foreach (var item in items) data.AddUpdateItems(item);
            ////data.UpdateBindingItems();
            lock (this)
            {
                this.Items.Clear();
                this.ItemIdToIndex.Clear();
                this.BindingItems.Clear();
                this.AddUpdateItems(items);
                this.UpdateBinding(items.Select(i=>i.Id));
            }
        }

        public void UpdateBinding(IEnumerable<string> keys=null)
        {
            lock (this)
            {
                if (_isResetBindingBusy) return;
                _isResetBindingBusy = !_isResetBindingBusy;
            }

            try
            {
                this.IndiceToResetBinding.Clear();
                if (Items.Count != BindingItems.Count)
                {
                    this.ItemIdToIndex.Clear();
                    this.BindingItems.Clear();
                    keys = keys ?? this.Items.Keys.OrderBy(k => k).ToList();
                }

                keys = keys ?? this.Items.Keys;
                foreach (var key in keys)
                {
                    var value = Items[key];
                    if (!this.ItemIdToIndex.ContainsKey(key))
                    {
                        this.ItemIdToIndex.Add(key, this.BindingItems.Count);
                        this.BindingItems.Add((T)Activator.CreateInstance(typeof(T), value));
                        this.IndiceToResetBinding.Add(ItemIdToIndex[key]);
                    }
                    else
                    {
                        var index = this.ItemIdToIndex[key];
                        if (!this.BindingItems[index].Equals(value))
                        {
                            this.BindingItems[index].Copy(value);
                            this.IndiceToResetBinding.Add(index);
                        }
                    }
                }
                if (this.IndiceToResetBinding.Count > 10)
                {
                    this.BindingItems.ResetBindings();
                }
                else
                {
                    foreach (var i in this.IndiceToResetBinding)
                        this.BindingItems.ResetItem(i);
                }
            }
            finally
            { 
                _isResetBindingBusy = !_isResetBindingBusy;
            }
        }

        //public void ResetBinding()
        //{
        //    lock (this)
        //    {
        //        if (this.IndiceToResetBinding.Count > 10)
        //        {
        //            this.BindingItems.ResetBindings();
        //        }
        //        else
        //        {
        //            foreach (var i in this.IndiceToResetBinding)
        //                this.BindingItems.ResetItem(i);
        //        }
        //    }
        //}

        //public static void UpdateListViewData(this ListViewData<T> data, IList<T> items, int threadshouldToResetAll = 10)
        //{
        //    var bindingData = data.BindingItems;
        //    var indiceToResetBinding = data.IndiceToResetBinding;
        //    var idToIndex = data.ItemIdToIndex;

        //    {
        //        indiceToResetBinding.Clear();
        //        if (items.Count != bindingData.Count)
        //        {
        //            idToIndex.Clear();
        //            bindingData.Clear();
        //        }

        //        //update OpenOrders
        //        foreach (var item in items)
        //        {
        //            if (!idToIndex.ContainsKey(item.Id))
        //            {
        //                idToIndex.Add(item.Id, bindingData.Count);
        //                bindingData.Add((T)Activator.CreateInstance(typeof(T)));
        //            }

        //            var index = idToIndex[item.Id];
        //            if (!bindingData[index].Equals(item))
        //            {
        //                bindingData[index].Copy(item);
        //                indiceToResetBinding.Add(index);
        //            }
        //        }

        //        if (indiceToResetBinding.Count > threadshouldToResetAll)
        //        {
        //            bindingData.ResetBindings();
        //        }
        //        else
        //        {
        //            foreach (var i in indiceToResetBinding)
        //                bindingData.ResetItem(i);
        //        }
        //    }
        //}
    }
}
