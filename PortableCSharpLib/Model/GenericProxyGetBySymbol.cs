using Newtonsoft.Json;
using PortableCSharpLib.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PortableCSharpLib.Model
{
    public class GenericProxyGetBySymbol<T> : IGenericProxy<T> where T : class, IIdEqualCopy<T>
    {
        Func<string, T> _funcGetItem;

        //private bool _isUpdateItemBusy;
        //private System.Timers.Timer _timer1 = new System.Timers.Timer();
        //private ConcurrentQueue<string> _queueUpdateItem = new ConcurrentQueue<string>();
        //private double _timerinterval;     //timmer interval
        CancellationTokenSource _cts = new CancellationTokenSource();

        //file
        private bool _isDataChangedSinceLastSave = false;
        private string _fileNameDump;
        private HashSet<string> _symbols;

        private int _pollingInterval;       //interval to update data from server
        private ConcurrentDictionary<string, long> _lastUpdateOrderTimePerSymbol = new ConcurrentDictionary<string, long>();
        private bool _isUpdateBusy;

        //private bool IsPollingEnabled => _pollingInterval < 999999999 && _funcGetItem != null;

        public bool IsStarted { get; private set; }
        public ConcurrentDictionary<string, T> Items { get; private set; } = new ConcurrentDictionary<string, T>();
        public event PortableCSharpLib.EventHandlers.ItemChangedEventHandler<T> OnItemAdded;
        public event PortableCSharpLib.EventHandlers.ItemChangedEventHandler<T> OnItemUpdated;
        public event PortableCSharpLib.EventHandlers.ItemChangedEventHandler<List<T>> OnItemListUpdated;
        public event EventHandlers.ItemWithIdChangedEventHandler<string> OnExceptionOccured;

        IRateGate _gate;

        public GenericProxyGetBySymbol(List<string> symbols = null, Func<string, T> funcGetItem = null, string filename = null, int pollingInterval = 999999999, int numReqPer2Seconds = 100)
        {
            if (symbols != null && symbols.Count > 0)
                _symbols = new HashSet<string>(symbols);

            _fileNameDump = filename;            //load from file when init
            //_timerinterval = timerInterval;      //interval for timer
            _pollingInterval = pollingInterval;  //means polling is enabled 
            _funcGetItem = funcGetItem;
            _gate = new RateGate(numReqPer2Seconds, new TimeSpan(0, 0, 2));

            //_timer1.Interval = timerInterval;
            //_timer1.Elapsed += (s, e) => this.UpdateFromServer();
            //if (this.IsPollingEnabled)
            //    this.ProcessUpdate();
        }

        private void ProcessUpdate()
        {
            Task.Factory.StartNew(() =>
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    var tnow = DateTime.UtcNow.GetUnixTimeFromUTC();
                    foreach (var symbol in _lastUpdateOrderTimePerSymbol.Keys)
                    {
                        if (_lastUpdateOrderTimePerSymbol.ContainsKey(symbol) &&
                            tnow - _lastUpdateOrderTimePerSymbol[symbol] >= _pollingInterval)
                        {
                            try
                            {
                                _lastUpdateOrderTimePerSymbol[symbol] = tnow;
                                //_gate.WaitToProceed();
                                Thread.Sleep(500);
                                var item = _funcGetItem(symbol);
                                if (item != null)
                                    this.Update(item);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"error when update {symbol} => {ex.ToString()}");
                                OnExceptionOccured?.Invoke(this, $"{_funcGetItem.Method.ReturnType.Name}_{symbol}", ex.ToString());
                            }
                            Thread.Sleep(500);
                        }
                    }
                    Thread.Sleep(1000);

                    if (_isDataChangedSinceLastSave && !string.IsNullOrEmpty(_fileNameDump))
                    {
                        this.DumpToFile();
                        this.SetDataChanged(false);
                    }
                }
            }, _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        void SetDataChanged(bool flag)
        {
            lock(this)
            {
                _isDataChangedSinceLastSave = flag;
            }
        }

        public T GetItemById(string id)
        {
            if (Items.ContainsKey(id))
                return Items[id];
            else if ((_symbols == null || _symbols.Contains(id)) && _funcGetItem != null)
            {
                var t = _funcGetItem(id);
                Thread.Sleep(500);

                if (t != null)
                {
                    this.Update(t);
                    var tnow = DateTime.UtcNow.GetUnixTimeFromUTC();
                    _lastUpdateOrderTimePerSymbol.TryAdd(id, tnow);
                    return t;
                }
            }
            return default(T);
        }

        public bool Enqueue(string symbol)
        {
            if (!_lastUpdateOrderTimePerSymbol.ContainsKey(symbol))
            {
                _lastUpdateOrderTimePerSymbol.TryAdd(symbol, 0);
                return true;
            }
            return false;
        }

        public void Dequeue(string symbol)
        {
            if (_lastUpdateOrderTimePerSymbol.ContainsKey(symbol))
            {
                long v;
                _lastUpdateOrderTimePerSymbol.TryRemove(symbol, out v);
            }
        }

        public void Update(params T[] items)
        {
            lock (this)
            {
                if (_isUpdateBusy) return;
                _isUpdateBusy = !_isUpdateBusy;
            }

            try
            {
                foreach (var t in items)
                {
                    if (!Items.ContainsKey(t.Id))
                    {
                        Items.TryAdd(t.Id, t);
                        OnItemAdded?.Invoke(this, Items[t.Id]);
                        this.SetDataChanged(true);
                    }

                    if (!Items[t.Id].Equals(t))
                    {
                        Items[t.Id].Copy(t);
                        OnItemUpdated?.Invoke(this, Items[t.Id]);
                        this.SetDataChanged(true);
                    }
                }
            }
            finally
            {
                _isUpdateBusy = !_isUpdateBusy;
            }
        }

        public void Start()
        {
            if (!IsStarted)
            {
                IsStarted = true;
                this.LoadFromFile();
                this.ProcessUpdate();
            }
        }

        public void Stop()
        {
            if (IsStarted)
            {
                IsStarted = false;
                _cts.Cancel();
            }
        }

        public void DumpToFile()
        {
            if (!string.IsNullOrEmpty(_fileNameDump))
            {
                var str = JsonConvert.SerializeObject(Items, Formatting.Indented);
                var datestr = DateTime.UtcNow.ToString("yyyyMMdd");
                var datapath = Path.GetDirectoryName(_fileNameDump);
                File.WriteAllText(_fileNameDump, str.ToString());
                File.WriteAllText(Path.Combine(datapath, $"{datestr}_{Path.GetFileName(_fileNameDump)}"), str.ToString());
            }
        }

        void LoadFromFile()
        {
            if (File.Exists(this._fileNameDump))
            {
                var str = File.ReadAllText(this._fileNameDump);
                this.Items = JsonConvert.DeserializeObject<ConcurrentDictionary<string, T>>(str);
            }
        }
    }
}
//void ProcessUpdateOrderQueue()
//{
//    lock (this)
//    {
//        if (_isUpdateItemBusy) return;
//        _isUpdateItemBusy = !_isUpdateItemBusy;
//    }

//    Task.Run(() =>
//    {
//        try
//        {
//            string symbol;
//            while (_queueUpdateItem.TryDequeue(out symbol))
//            {
//                try
//                {
//                    var item = _funcGetItem(symbol);
//                    if (item != null)
//                        this.Update(item);
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"error when update {symbol} => {ex.ToString()}");
//                }
//                Thread.Sleep(500);
//            }
//        }
//        finally
//        {
//            _isUpdateItemBusy = !_isUpdateItemBusy;
//        }
//    });
//}

//private void UpdateFromServer()
//{
//    if (this.IsPollingEnabled)
//    {
//        var tnow = DateTime.UtcNow.GetUnixTimeFromUTC();
//        foreach (var symbol in _lastUpdateOrderTimePerSymbol.Keys)
//        {
//            if (tnow - _lastUpdateOrderTimePerSymbol[symbol] >= _pollingInterval)
//            {
//                _lastUpdateOrderTimePerSymbol[symbol] = tnow;
//                if (!_queueUpdateItem.Contains(symbol))
//                    _queueUpdateItem.Enqueue(symbol);
//            }
//        }
//        //if (!_queueUpdateItem.IsEmpty)
//        //    this.ProcessUpdateOrderQueue();
//    }
//}
