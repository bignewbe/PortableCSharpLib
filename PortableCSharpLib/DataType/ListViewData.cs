using PortableCSharpLib.Interace;
using PortableCSharpLib.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PortableCSharpLib.DataType
{
    public class ListViewData<T> : CINotification where T : class, IIdEqualCopy<T>
    {
        public List<int> IndiceToResetBinding { get; set; } = new List<int>();
        public Dictionary<string, int> ItemIdToIndex { get; set; } = new Dictionary<string, int>();
        public ConcurrentDictionary<string, T> Items { get; set; } = new ConcurrentDictionary<string, T>();
        BindingList<T> _BindingItems = new BindingList<T>();
        public BindingList<T> BindingItems { get { return _BindingItems; } set { SetField(ref _BindingItems, value); } }

        //public T GetDefaultValue(Type type)
        //{
        //    return type.IsValueType ? (T) Activator.CreateInstance(type) : null;
        //}

        //public void UpdateBindingItems()
        //{
        //    foreach (var symbol in this.Items.Keys.OrderBy(s => s))
        //    {
        //        if (!this.ItemIdToIndex.ContainsKey(symbol))
        //        {
        //            this.ItemIdToIndex.Add(symbol, this.BindingItems.Count);
        //            this.BindingItems.Add(this.GetDefaultValue(typeof(T)));
        //        }
        //        var index = this.ItemIdToIndex[symbol];
        //        var ticker = this.Items[symbol];
        //        if (!this.CompareTickers(this.BindingItems[index], ticker))
        //        {
        //            this.BindingItems[index].Copy(ticker);
        //            this.IndiceToResetBinding.Add(index);
        //        }
        //    }
        //}

        public void ResetBinding()
        {
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
