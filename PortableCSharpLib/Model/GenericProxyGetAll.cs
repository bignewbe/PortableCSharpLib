using Newtonsoft.Json;
using PortableCSharpLib.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PortableCSharpLib.Model
{
    public class GenericProxyGetAll<T> : IGenericProxy<T> where T : class, IIdEqualCopy<T>
    {
        public static IGenericProxy<T> CreateProxyWithoutPoll()
        {
            return new GenericProxyGetAll<T>();
        }

        Func<List<T>> _funcGetItems;

        //private bool _isProcessItemBusy = false;
        //private System.Timers.Timer _timer1 = new System.Timers.Timer();
        CancellationTokenSource _cts = new CancellationTokenSource();

        public bool IsStarted { get; private set; }
        public ConcurrentDictionary<string, T> Items { get; private set; } = new ConcurrentDictionary<string, T>();

        private long _lastUpdateTime;
        private int _pollingInterval;       //interval to update data from server
        //private bool IsPollingEnabled => _pollingInterval < 99999999 && _funcGetItems != null;

        //file
        private string _fileNameDump;
        private bool _isDataChangedSinceLastSave = false;

        public event PortableCSharpLib.EventHandlers.ItemChangedEventHandler<T> OnItemAdded;
        public event PortableCSharpLib.EventHandlers.ItemChangedEventHandler<T> OnItemUpdated;
        public event PortableCSharpLib.EventHandlers.ItemChangedEventHandler<List<T>> OnItemListUpdated;
        public event EventHandlers.ItemWithIdChangedEventHandler<string> OnExceptionOccured;

        public GenericProxyGetAll(Func<List<T>> getItem = null, string filename = null, int pollingInterval = 99999999)
        {
            _fileNameDump = filename;
            _funcGetItems = getItem;
            _pollingInterval = pollingInterval;

            //_timerinterval = timerInterval;
            //_timer1.Interval = timerInterval;
            //_timer1.Elapsed += (s, e) => this.UpdateFromServer();
        }

        private void ProcessUpdate()
        {
            Task.Factory.StartNew(() =>
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    var tnow = DateTime.UtcNow.GetUnixTimeFromUTC();
                    if (tnow - _lastUpdateTime >= _pollingInterval)
                    {
                        try
                        {
                            _lastUpdateTime = tnow;
                            var lst = _funcGetItems();
                            if (lst != null)
                                this.Update(lst.ToArray());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"error when update {ex.ToString()}");
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

        public T GetItemById(string id)
        {
            if (Items.ContainsKey(id)) return Items[id];
            return default(T);
        }

        public void Update(params T[] items)
        {
            foreach (var t in items)
            {
                if (!Items.ContainsKey(t.Id))
                {
                    Items.TryAdd(t.Id, t);
                    OnItemAdded?.Invoke(this, t);
                    this.SetDataChanged(true);
                }

                if (!Items[t.Id].Equals(t))
                {
                    Items[t.Id].Copy(t);
                    OnItemUpdated?.Invoke(this, t);
                    this.SetDataChanged(true);
                }
            }
        }

        void SetDataChanged(bool flag)
        {
            lock (this)
            {
                _isDataChangedSinceLastSave = flag;
            }
        }

        public void Start()
        {
            if (!IsStarted)
            {
                IsStarted = true;

                this.Initialize();
                //if (this.IsPollingEnabled)
                this.ProcessUpdate();
            }
        }

        public void Stop()
        {
            if (IsStarted)
            {
                IsStarted = false;
                //if (this.IsPollingEnabled)
                _cts.Cancel();
            }
        }

        void Initialize()
        {
            this.LoadFromFile();

            if (_funcGetItems != null)
            {
                var lst = _funcGetItems();
                if (lst != null)
                {
                    foreach (var t in lst)
                        this.Items.TryAdd(t.Id, t);
                }
                _lastUpdateTime = DateTime.UtcNow.GetUnixTimeFromUTC();
            }
            if (this.Items.Count > 0)
            {
                OnItemListUpdated?.Invoke(this, this.Items.Values.ToList());
                this.DumpToFile();
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

        public bool Enqueue(string symbol)
        {
            throw new NotImplementedException();
        }

        public void Dequeue(string symbol)
        {
            throw new NotImplementedException();
        }
    }
}


//private void UpdateFromServer()
//{
//    if (!this.IsPollingEnabled) return;
//    var tnow = DateTime.UtcNow.GetUnixTimeFromUTC();
//    if (tnow - _lastUpdateTime < _pollingInterval) return;

//    lock (this)
//    {
//        if (_isProcessItemBusy) return;
//        _isProcessItemBusy = !_isProcessItemBusy;
//    }

//    Task.Run(() =>
//    {
//        try
//        {
//            _lastUpdateTime = tnow;
//            if (_funcGetItems != null)
//            {
//                try
//                {
//                    var lst = _funcGetItems();
//                    if (lst != null)
//                        this.Update(lst.ToArray());
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"{ex.ToString()}");
//                }
//            }
//        }
//        finally
//        {
//            _isProcessItemBusy = !_isProcessItemBusy;
//        }
//    });
//}
