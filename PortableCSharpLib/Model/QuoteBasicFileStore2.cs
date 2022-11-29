﻿using PortableCSharpLib.Facility;
using PortableCSharpLib.Interface;
using PortableCSharpLib;
using PortableCSharpLib.TechnicalAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PortableCSharpLib.Model
{
    public class QuoteBasicFileStore : IQuoteBasicFileStore
    {
        int _maxNumBarsInFile = 1000000;

        public string Exchange { get; private set; }
        public string DataFolder { get; private set; }

        private ICache<QuoteBasicBase> _cache = new Cache<QuoteBasicBase>(50);
        //private IQuoteBasicDownloader _download;
        public event EventHandlers.QuoteSavedEventHandler OnQuoteSaved;

        public QuoteBasicFileStore(string exchange, string folder, int maxNumBarsInFile)
        {
            if (!Directory.Exists(folder))
                throw new DirectoryNotFoundException(folder);

            if (!this.IsDirectoryWritable(folder))
                throw new Exception($"{folder} not writable");

            this.DataFolder = folder;

            //_download = download;
            _maxNumBarsInFile = maxNumBarsInFile;
            this.Exchange = exchange;
            this.OnQuoteSaved += HistoricalQuote_OnQuoteSaved;
        }

        bool IsDirectoryWritable(string dirPath, bool throwIfFails = false)
        {
            try
            {
                using (FileStream fs = File.Create(Path.Combine(dirPath, Path.GetRandomFileName()), 1, FileOptions.DeleteOnClose))
                {
                }
                return true;
            }
            catch
            {
                if (throwIfFails)
                    throw;
                else
                    return false;
            }
        }

        private void HistoricalQuote_OnQuoteSaved(object sender, string exchange, string filename)
        {
            if (this.Exchange == exchange)
            {
                var q = _cache.GetItem(filename);
                if (q != null) _cache.RemoveItem(filename);
            }
        }

        private string GetQuoteFileName(string symbol, int interval, int index)
        {
            var fn = Path.Combine(this.DataFolder, string.Format("{0}_{1}_{2}_{3}.txt", Exchange, symbol, interval, index));
            return fn;
        }

        private List<string> FindQuoteFiles(string symbol, int interval)
        {
            return FileOperation.FindFilesContainingStr(this.DataFolder, string.Format("{0}_{1}_{2}", Exchange, symbol, interval));
        }

        //file naming convention: exchange_symbol_interval_index.txt
        public async Task<bool> Update(IQuoteBasicDownloader download, string symbol, int interval, int numBarsToRemoveGap = -1, int timeout = 50000)
        {
            //lock (this)
            {
                var files = this.FindQuoteFiles(symbol, interval);
                if (files != null && files.Count > 0)
                {
                    files.Sort();
                    var readtream = new FileStream(files.Last(), FileMode.Open);
                    var q1 = QuoteBasicBase.InitByStream(readtream);
                    readtream.Close();
                    if (numBarsToRemoveGap > 0)
                    {
                        for (int i = Math.Max(0, q1.Count - numBarsToRemoveGap); i < q1.Count - 1; i++)
                        {
                            if (q1.Time[i + 1] - q1.Time[i] > q1.Interval)
                            {
                                q1.Clear(i + 1, q1.Count - 1);
                                break;
                            }
                        }
                    }

                    //if time difference is too small => skip download
                    var utcNow = DateTime.UtcNow.GetUnixTimeFromUTC();
                    if (utcNow - q1.LastTime <= interval)
                        return false;

                    var q = await download.Download(symbol, interval, timeout);
                    if (q == null || q.Count == 0)
                        return false;

                    if (q1.Count < _maxNumBarsInFile)
                    {
                        q1.Append(q, false);
                        var writestream = new FileStream(files.Last(), FileMode.Truncate);
                        q1.AppendStream(writestream);
                        writestream.Close();
                        OnQuoteSaved?.Invoke(this, this.Exchange, files.Last());
                    }
                    else
                    {
                        var index = int.Parse(files.Last().Split('_')[4].Split('.')[0]);
                        var fn = this.GetQuoteFileName(symbol, interval, index + 1);
                        var writestream = new FileStream(fn, FileMode.OpenOrCreate);
                        q.AppendStream(writestream);
                        writestream.Close();

                        OnQuoteSaved?.Invoke(this, this.Exchange, fn);
                    }
                }
                else
                {
                    var q = await download.Download(symbol, interval, timeout);
                    if (q == null || q.Count == 0) return false;

                    var fn = this.GetQuoteFileName(symbol, interval, 0);
                    var writestream = new FileStream(fn, FileMode.OpenOrCreate);
                    q.AppendStream(writestream);
                    writestream.Close();

                    OnQuoteSaved?.Invoke(this, this.Exchange, fn);
                }

                return true;
            }
        }

        public static QuoteBasicBase LoadQuote(string filename)
        {
            try
            {
                Console.WriteLine($"QuoteBasicFileStore: load... {filename}");
                var readtream = new FileStream(filename, FileMode.Open);
                var q1 = QuoteBasicBase.InitByStream(readtream);
                readtream.Close();
                return q1;
            }
            catch (FileFormatNotSupportedException ex)
            {
                Console.WriteLine("remove last line from file");
                var lines = File.ReadAllLines(filename);
                lines = lines.Take(lines.Length - 1).ToArray();
                File.WriteAllLines(filename, lines);
                throw ex;
            }
        }

        public QuoteBasicBase Load(string symbol, int interval, long? startTime, int maxCount = 500)
        {
            lock (this)
            {
                var files = this.FindQuoteFiles(symbol, interval);
                if (files == null || files.Count <= 0) return null;

                var quotes = new List<QuoteBasicBase>();
                var quote = new QuoteBasicBase(symbol, interval);
                files.Sort();

                var count = 0;
                for (int i = files.Count - 1; i >= 0; i--)
                {
                    var q = _cache.GetItem(files[i]);
                    if (q == null)
                    {
                        q = QuoteBasicFileStore.LoadQuote(files[i]);
                        _cache.AddItem(files[i], q);
                    }

                    quotes.Add(q);
                    count += q.Count;

                    if (startTime.HasValue && q.FirstTime <= startTime)
                        break;

                    if (count >= maxCount)
                        break;
                }

                for (int i = 0; i < quotes.Count; i++)
                    quote.Append(quotes[i], false);

                var ind1 = startTime.HasValue ? quote.FindIndexForGivenTime(startTime.Value) : 0;
                var ind2 = quote.Count - maxCount;
                var index = Math.Max(0, Math.Max(ind1, ind2));

                if (index != 0)
                    return quote.Extract(index, quote.Count - 1) as QuoteBasicBase;
                else
                    return quote;
            }
        }

        //file naming convention: exchange_symbol_interval_index.txt
        public bool Save(IQuoteBasicBase quote, int numBarsToRemoveGap = -1)
        {
            lock (this)
            {
                if (quote == null || quote.Count == 0) return false;
                var files = this.FindQuoteFiles(quote.Symbol, quote.Interval);

                if (files != null && files.Count > 0)
                {
                    files.Sort();
                    var q1 = QuoteBasicExension.LoadFile(files.Last());

                    if (numBarsToRemoveGap > 0)
                    {
                        for (int i = Math.Max(0, q1.Count - numBarsToRemoveGap); i < q1.Count - 1; i++)
                        {
                            if (q1.Time[i + 1] - q1.Time[i] > q1.Interval)
                            {
                                q1.Clear(i + 1, q1.Count - 1);
                                break;
                            }
                        }
                    }

                    //var readtream = new FileStream(files.Last(), FileMode.Open);
                    //var q1 = QuoteBasicBase.InitByStream(readtream);
                    //readtream.Close();

                    if (quote.LastTime <= q1.LastTime) return false;

                    if (q1.Count < _maxNumBarsInFile)
                    {
                        var fn = files.Last();
                        q1.Append(quote);
                        q1.SaveToFile(fn);
                        //var writestream = new FileStream(files.Last(), FileMode.Truncate);
                        //q1.AppendStream(writestream);
                        //writestream.Close();
                        OnQuoteSaved?.Invoke(this, this.Exchange, fn);
                    }
                    else
                    {
                        var last = files.Last().Split('_').Last();
                        var index = int.Parse(last.Split('.')[0]);
                        var fn = this.GetQuoteFileName(quote.Symbol, quote.Interval, index + 1);

                        //var writestream = new FileStream(fn, FileMode.OpenOrCreate);
                        if (q1.LastTime >= quote.FirstTime)  //remove redundant sticks from new files
                        {
                            var sind = quote.FindIndexForGivenTime(q1.LastTime);
                            var q2 = new QuoteBasicBase(quote);
                            q2.Clear(0, sind);
                            q2.SaveToFile(fn);
                            //q2.AppendStream(writestream);
                        }
                        else
                        {
                            quote.SaveToFile(fn);
                            //quote.AppendStream(writestream);
                        }
                        //writestream.Close();
                        OnQuoteSaved?.Invoke(this, this.Exchange, fn);
                    }
                }
                else
                {
                    var fn = this.GetQuoteFileName(quote.Symbol, quote.Interval, 0);
                    quote.SaveToFile(fn);
                    //var writestream = new FileStream(fn, FileMode.OpenOrCreate);
                    //quote.AppendStream(writestream);
                    //writestream.Close();
                    OnQuoteSaved?.Invoke(this, this.Exchange, fn);
                }

                return true;
            }
        }
    }

    public class QuoteBasicFileStore2: IQuoteBasicFileStore
    {
        public string Exchange { get; private set; }
        public string DataFolder { get; private set; }

        public event EventHandlers.QuoteSavedEventHandler OnQuoteSaved;

        public QuoteBasicFileStore2(string exchange, string folder)
        {
            if (!Directory.Exists(folder))
                throw new DirectoryNotFoundException(folder);

            if (!this.IsDirectoryWritable(folder))
                throw new Exception($"{folder} not writable");

            this.DataFolder = folder;
            this.Exchange = exchange;
        }

        bool IsDirectoryWritable(string dirPath, bool throwIfFails = false)
        {
            try
            {
                using (FileStream fs = File.Create(Path.Combine(dirPath, Path.GetRandomFileName()), 1, FileOptions.DeleteOnClose))
                {
                }
                return true;
            }
            catch
            {
                if (throwIfFails)
                    throw;
                else
                    return false;
            }
        }

        private string GetQuoteFileName(string symbol, int interval) =>
            Path.Combine(this.DataFolder, $"{Exchange}_{symbol}_{interval}.txt");

        public bool Update(Func<string, int, IQuoteBasicBase> funcDownload, string symbol, int interval, int numBarsToRemoveGap=-1)
        {
            var fn = this.GetQuoteFileName(symbol, interval);
            if (File.Exists(fn))
            {
                var readtream = new FileStream(fn, FileMode.Open);
                var q1 = QuoteBasicBase.InitByStream(readtream);
                readtream.Close();
                if (numBarsToRemoveGap > 0)
                {
                    for (int i = Math.Max(0, q1.Count - numBarsToRemoveGap); i < q1.Count - 1; i++)
                    {
                        if (q1.Time[i + 1] - q1.Time[i] > q1.Interval)
                        {
                            q1.Clear(i + 1, q1.Count - 1);
                            break;
                        }
                    }
                }

                //if time difference is too small => skip download
                var utcNow = DateTime.UtcNow.GetUnixTimeFromUTC();
                if (utcNow - q1.LastTime <= interval)
                    return false;

                var q = funcDownload(symbol, interval);
                if (q == null || q.Count == 0)
                    return false;

                q1.Append(q, false);
                var writestream = new FileStream(fn, FileMode.Truncate);
                q1.AppendStream(writestream);
                writestream.Close();
                OnQuoteSaved?.Invoke(this, this.Exchange, fn);

                return true;
            }
            else
            {
                var q = funcDownload(symbol, interval);
                if (q == null || q.Count == 0) return false;
                var writestream = new FileStream(fn, FileMode.OpenOrCreate);
                q.AppendStream(writestream);
                writestream.Close();
                OnQuoteSaved?.Invoke(this, this.Exchange, fn);

                return true;
            }
        }

        public static QuoteBasicBase LoadQuote(string filename)
        {
            try
            {
                Console.WriteLine($"QuoteBasicFileStore: load... {filename}");
                var readtream = new FileStream(filename, FileMode.Open);
                var q1 = QuoteBasicBase.InitByStream(readtream);
                readtream.Close();
                return q1;
            }
            catch (FileFormatNotSupportedException ex)
            {
                Console.WriteLine("remove last line from file");
                var lines = File.ReadAllLines(filename);
                lines = lines.Take(lines.Length - 1).ToArray();
                File.WriteAllLines(filename, lines);
                throw ex;
            }
        }

        public QuoteBasicBase Load(string symbol, int interval, long? startTime, int maxCount = 500)
        {
            lock (this)
            {
                var fn = this.GetQuoteFileName(symbol, interval);

                if (!File.Exists(fn)) return null;                    
                var quote = QuoteBasicFileStore2.LoadQuote(fn);
                var ind1 = startTime.HasValue ? quote.FindIndexForGivenTime(startTime.Value) : 0;
                var ind2 = quote.Count - maxCount;
                var index = Math.Max(0, Math.Max(ind1, ind2));
                if (index != 0)
                    return quote.Extract(index, quote.Count - 1) as QuoteBasicBase;
                else
                    return quote;
            }
        }

        public bool Save(IQuoteBasicBase quote, int numBarsToRemoveGap = -1)
        {
            lock (this)
            {
                if (quote == null || quote.Count == 0) return false;
                var fn = this.GetQuoteFileName(quote.Symbol, quote.Interval);
                if (File.Exists(fn))
                {
                    var q1 = QuoteBasicExension.LoadFile(fn);
                    if (numBarsToRemoveGap > 0)
                    {
                        for (int i = Math.Max(0, q1.Count - numBarsToRemoveGap); i < q1.Count - 1; i++)
                        {
                            if (q1.Time[i + 1] - q1.Time[i] > q1.Interval)
                            {
                                q1.Clear(i + 1, q1.Count - 1);
                                break;
                            }
                        }
                    }
                    if (quote.LastTime <= q1.LastTime) return false;
                    q1.Append(quote);
                    q1.SaveToFile(fn);
                    OnQuoteSaved?.Invoke(this, this.Exchange, fn);
                }
                else
                {
                    quote.SaveToFile(fn);
                    OnQuoteSaved?.Invoke(this, this.Exchange, fn);
                }
                return true;
            }
        }
    }
}
