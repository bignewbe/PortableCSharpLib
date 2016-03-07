using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    #region test cases
    /// <summary>
    /// Test different senarios
    /// 1. It will fail if a class implement IDictionary/IList/ICollection/IEnumerable, while the property/filed is defined with HiPerfMember attributes.
    ///    In this case, we can remove the HiPerfMember attribute and let interface take control of serialization.
    /// 2. ICollection and IEnumerable are not yet tested.
    /// </summary>
    //static public void RunTestCases()
    //{
    //    var testCaseID = 0;
    //    var testCaseName = "";
    //    dynamic obj = null;

    //    var stock = new CGoogleFinancical("GOOG", 100);
    //    //stock.Update();
    //    //ObjectSerializer.SerializeObject("stockfortestcase.dat", stock);
    //    ObjectSerializer.DeSerializeObject("stockfortestcase.dat", ref stock);

    //    #region test CSortedList<string, CGoogleFinancical>
    //    {
    //        var list = new CSortedList<string, CGoogleFinancical>();
    //        list.Add(stock.Symbol, stock);
    //        list.Add("aa", stock);
    //        list.Add("bb", stock);
    //        list.Add("cc", stock);
    //        testCaseName = "Test CSortedList<string, CGoogleFinancical>";
    //        using (var stream = new MemoryStream())
    //        {
    //            BinarySerializer.SerializeObject(stream, list);
    //            stream.Position = 0;
    //            var t = BinarySerializer.DeSerializeObject(stream, list.GetType());
    //            Console.WriteLine("Test-{0,2}: {1,80} => {2}\n", testCaseID++, testCaseName, CompareObjects(t, list));
    //        }
    //    }
    //    #endregion

    //#region test Tuple
    //        var val = new Tuple<long, CSelectParams>(1, new CSelectParams());

    //BinarySerializer.SerializeObject("tuple.dat", val);
    //var v2 = (Tuple<long, CSelectParams>)BinarySerializer.DeSerializeObject("tuple.dat", val.GetType());

    //var lst = new List<Tuple<long, CSelectParams>>();
    //lst.Add(val);

    //v2.Item2.S_halfSizeForMinMax = 1000;
    //lst.Add(v2);

    //BinarySerializer.SerializeObject("tuple.dat", lst);
    //var v3 = (List<Tuple<long, CSelectParams>>)BinarySerializer.DeSerializeObject("tuple.dat", lst.GetType());
    //#endregion

    //    #region test DataTable
    //    var table = new DataTable("TableName");
    //    table.Columns.Add("Symbol");
    //    table.Columns.Add("Date", typeof(DateTime));
    //    table.Columns.Add("Price", typeof(decimal));
    //    table.Columns.Add("Num", typeof(int));
    //    table.Rows.Add("GOOG", DateTime.Now, 100.0, 999);
    //    testCaseName = "Test DataTable";

    //    using (var stream = new MemoryStream())
    //    {
    //        var writer = new BinaryWriter(stream);
    //        BinarySerializer.SerializeObject(stream, table);
    //        stream.Position = 0;
    //        var t = BinarySerializer.DeSerializeObject(stream, table.GetType());
    //        Console.WriteLine("Test-{0,2}: {1,80} => {2}\n", testCaseID++, testCaseName, CompareObjects(t, table));
    //    }
    //    #endregion

    //    #region test CGoogleFinancical
    //    testCaseName = "Test CGoogleFinancical";

    //    using (var stream = new MemoryStream())
    //    {
    //        BinarySerializer.SerializeObject(stream, stock);
    //        stream.Position = 0;
    //        var t = BinarySerializer.DeSerializeObject(stream, stock.GetType());
    //        Console.WriteLine("Test-{0,2}: {1,80} => {2}\n", testCaseID++, testCaseName, CompareObjects(t, stock));
    //    }
    //    #endregion


    //    #region test Decimal[]
    //    {
    //        var list = new Decimal[10];
    //        testCaseName = "Test Decimal[]";

    //        using (var stream = new MemoryStream())
    //        {
    //            BinarySerializer.SerializeObject(stream, list);
    //            stream.Position = 0;
    //            var t = BinarySerializer.DeSerializeObject(stream, list.GetType());
    //            Console.WriteLine("Test-{0,2}: {1,80} => {2}\n", testCaseID++, testCaseName, CompareObjects(t, list));
    //        }
    //    }
    //    #endregion

    //    #region test CSortedList<string, CStockQuoteVolume>
    //    {
    //        obj = new CSortedList<string, CStockQuoteVolume>();
    //        var goog = new CStockQuoteVolume("GOOG");
    //        //goog.GetQuotes(CConstant.quote_start_date);
    //        //ObjectSerializer.WriteSupported("goog.dat", goog);
    //        ObjectSerializer.DeSerializeObject("goog.dat", ref goog);
    //        obj.Add(goog.Symbol, goog);
    //        obj.Add("MSFT", goog);
    //        testCaseName = "Test CSortedList<string, CStockQuoteVolume>";

    //        using (var stream = new MemoryStream())
    //        {
    //            BinarySerializer.SerializeObject(stream, obj);
    //            stream.Position = 0;
    //            var t = BinarySerializer.DeSerializeObject(stream, obj.GetType());
    //            Console.WriteLine("Test-{0,2}: {1,80} => {2}\n", testCaseID++, testCaseName, CompareObjects(t, obj));
    //        }
    //    }
    //    #endregion

    //    #region test List<CStockQuoteVolume>
    //    {
    //        var stock1 = new CStockQuoteVolume("GOOG");
    //        //stock1.GetQuotes(CConstant.quote_start_date);
    //        ObjectSerializer.DeSerializeObject("stock.dat", ref stock1);
    //        obj = new List<CStockQuoteVolume>();
    //        obj.AddRange(Enumerable.Repeat(stock1, 10));
    //        testCaseName = "Test List<CStockQuoteVolume>";

    //        using (var stream = new MemoryStream())
    //        {
    //            BinarySerializer.SerializeObject(stream, obj);
    //            stream.Position = 0;
    //            var t = BinarySerializer.DeSerializeObject(stream, obj.GetType());
    //            Console.WriteLine("Test-{0,2}: {1,80} => {2}\n", testCaseID++, testCaseName, CompareObjects(t, obj));
    //        }
    //    }
    //    #endregion

    //    #region test List<DateTime>
    //    {
    //        var list = new List<DateTime>();
    //        var dnow = DateTime.Now;
    //        list.Add(dnow.AddDays(-3));
    //        list.Add(dnow.AddDays(-2));
    //        list.Add(dnow.AddDays(-1));
    //        list.Add(dnow);

    //        testCaseName = "Test List<DateTime>";
    //        using (var stream = new MemoryStream())
    //        {
    //            BinarySerializer.SerializeObject(stream, list);
    //            stream.Position = 0;
    //            var t = BinarySerializer.DeSerializeObject(stream, list.GetType());
    //            Console.WriteLine("Test-{0,2}: {1,80} => {2}\n", testCaseID++, testCaseName, CompareObjects(t, list));
    //        }
    //    }
    //    #endregion

    //    #region test int[]
    //    {
    //        var list = new List<int>();
    //        Enumerable.Range(100, 10).ToList().ForEach(t => list.Add(t));
    //        var arr = list.ToArray();

    //        testCaseName = "Test int[]";

    //        using (var stream = new MemoryStream())
    //        {
    //            BinarySerializer.SerializeObject(stream, arr);
    //            stream.Position = 0;
    //            var t = BinarySerializer.DeSerializeObject(stream, arr.GetType());
    //            Console.WriteLine("Test-{0,2}: {1,80} => {2}\n", testCaseID++, testCaseName, CompareObjects(t, arr));
    //        }
    //    }
    //    #endregion

    //    #region test List<double>
    //    {
    //        var list = new List<double>();
    //        Enumerable.Range(100, 10).ToList().ForEach(t => list.Add(t));
    //        testCaseName = "Test List<double>";

    //        using (var stream = new MemoryStream())
    //        {
    //            BinarySerializer.SerializeObject(stream, list);
    //            stream.Position = 0;
    //            var t = BinarySerializer.DeSerializeObject(stream, list.GetType());
    //            Console.WriteLine("Test-{0,2}: {1,80} => {2}\n", testCaseID++, testCaseName, CompareObjects(t, list));
    //        }
    //    }
    //    #endregion

    //    #region test List<CGoogleFinancical>
    //    {
    //        var list = new List<CGoogleFinancical>();
    //        list.Add(stock);
    //        testCaseName = "Test List<CGoogleFinancical>";

    //        using (var stream = new MemoryStream())
    //        {
    //            BinarySerializer.SerializeObject(stream, list);
    //            stream.Position = 0;
    //            var t = BinarySerializer.DeSerializeObject(stream, list.GetType());
    //            Console.WriteLine("Test-{0,2}: {1,80} => {2}\n", testCaseID++, testCaseName, CompareObjects(t, list));
    //        }
    //    }
    //    #endregion

    //    #region test Dictionary<int, double>
    //    {
    //        var dict1 = new Dictionary<int, double>();
    //        dict1.Add(100, 342);
    //        dict1.Add(102, 500);
    //        testCaseName = "Test Dictionary<int, double>";

    //        using (var stream = new MemoryStream())
    //        {
    //            BinarySerializer.SerializeObject(stream, dict1);
    //            stream.Position = 0;
    //            var t = BinarySerializer.DeSerializeObject(stream, dict1.GetType());
    //            Console.WriteLine("Test-{0,2}: {1,80} => {2}\n", testCaseID++, testCaseName, CompareObjects(t, dict1));
    //        }
    //    }
    //    #endregion

    //    #region test Dictionary<int, Dictionary<string, CGoogleFinancical>>
    //    {
    //        var dict1 = new Dictionary<string, CGoogleFinancical>();
    //        var dict2 = new Dictionary<int, Dictionary<string, CGoogleFinancical>>();
    //        dict1.Add(stock.Symbol, stock);
    //        dict2.Add(1, dict1);
    //        testCaseName = "Test Dictionary<int, Dictionary<string, CGoogleFinancical>>";

    //        using (var stream = new MemoryStream())
    //        {
    //            BinarySerializer.SerializeObject(stream, dict2);
    //            stream.Position = 0;
    //            var t = BinarySerializer.DeSerializeObject(stream, dict2.GetType());
    //            Console.WriteLine("Test-{0,2}: {1,80} => {2}\n", testCaseID++, testCaseName, CompareObjects(t, dict2));
    //        }
    //    }
    //    #endregion

    //    #region test List<KeyValuePair<string, CGoogleFinancical>>
    //    var listKV = new List<KeyValuePair<string, CGoogleFinancical>>();
    //    listKV.Add(new KeyValuePair<string, CGoogleFinancical>(stock.Symbol, stock));
    //    testCaseName = "Test List<KeyValuePair<string, CGoogleFinancical>>";

    //    using (var stream = new MemoryStream())
    //    {
    //        BinarySerializer.SerializeObject(stream, listKV);
    //        stream.Position = 0;
    //        var t = BinarySerializer.DeSerializeObject(stream, listKV.GetType());
    //        Console.WriteLine("Test-{0,2}: {1,80} => {2}\n", testCaseID++, testCaseName, CompareObjects(t, listKV));
    //    }
    //    #endregion

    //    #region test KeyValuePair<string, CGoogleFinancical>[]
    //    {
    //        var arr = new KeyValuePair<string, CGoogleFinancical>[] { new KeyValuePair<string, CGoogleFinancical>(stock.Symbol, stock) };
    //        testCaseName = "Test KeyValuePair<string, CGoogleFinancical>[]";
    //        using (var stream = new MemoryStream())
    //        {
    //            BinarySerializer.SerializeObject(stream, arr);
    //            stream.Position = 0;
    //            var t = BinarySerializer.DeSerializeObject(stream, arr.GetType());
    //            Console.WriteLine("Test-{0,2}: {1,80} => {2}\n", testCaseID++, testCaseName, CompareObjects(t, arr));
    //        }
    //    }
    //    #endregion

    //    #region test CSortedList<int, CStockQuoteVolume>
    //    {
    //        var list = new CSortedList<int, CStockQuoteVolume>();
    //        list.Add(139, null);
    //        list.Add(134, null);
    //        list.Add(136, null);
    //        testCaseName = "Test CSortedList<int, CStockQuoteVolume>";

    //        using (var stream = new MemoryStream())
    //        {
    //            BinarySerializer.SerializeObject(stream, list);
    //            stream.Position = 0;
    //            var t = BinarySerializer.DeSerializeObject(stream, list.GetType());
    //            Console.WriteLine("Test-{0,2}: {1,80} => {2}\n", testCaseID++, testCaseName, CompareObjects(t, list));
    //        }
    //    }
    //    #endregion

    //    #region test CSortedList<string, CStockQuoteVolume>
    //    {
    //        var stockList = new CSortedList<string, CStockQuoteVolume>();
    //        var goog = new CStockQuoteVolume("GOOG");
    //        //goog.GetQuotes(CConstant.quote_start_date);
    //        //ObjectSerializer.WriteAllTypes("goog.dat", goog);
    //        ObjectSerializer.DeSerializeObject("goog.dat", ref goog);
    //        stockList.Add(goog.Symbol, goog);
    //        testCaseName = "Test CSortedList<string, CStockQuoteVolume>";

    //        using (var stream = new MemoryStream())
    //        {
    //            BinarySerializer.SerializeObject(stream, stockList);
    //            stream.Position = 0;
    //            var t = BinarySerializer.DeSerializeObject(stream, stockList.GetType());
    //            Console.WriteLine("Test-{0,2}: {1,80} => {2}\n", testCaseID++, testCaseName, CompareObjects(t, stockList));
    //        }
    //    }
    //    #endregion

    //    #region test FixedList<CGoogleFinancical>
    //    {
    //        var flist = new FixedList<CGoogleFinancical>();
    //        flist.AddItem(stock);
    //        testCaseName = "Test FixedList<CGoogleFinancical>";

    //        using (var stream = new MemoryStream())
    //        {
    //            BinarySerializer.SerializeObject(stream, flist);
    //            stream.Position = 0;
    //            var t = BinarySerializer.DeSerializeObject(stream, flist.GetType());
    //            Console.WriteLine("Test-{0,2}: {1,80} => {2}\n", testCaseID++, testCaseName, CompareObjects(t, flist));
    //        }
    //    }
    //    #endregion

    //    #region test CFixedList<double>
    //    {
    //        var list = new CFixedList<double>(1000);
    //        Enumerable.Range(234, 20).ToList().ForEach(e => list.AddElement(e));
    //        testCaseName = "Test CFixedList<double>";

    //        using (var stream = new MemoryStream())
    //        {
    //            BinarySerializer.SerializeObject(stream, list);
    //            stream.Position = 0;
    //            var t = BinarySerializer.DeSerializeObject(stream, list.GetType());
    //            Console.WriteLine("Test-{0,2}: {1,80} => {2}\n", testCaseID++, testCaseName, CompareObjects(t, list));
    //        }
    //    }
    //    #endregion
    //}
    //static public void PerformanceTest()
    //{
    //    var stock1 = new CStockQuoteVolume("GOOG");
    //    var stock2 = new CStockQuoteVolume("GOOG");
    //    //stock1.GetQuotes(CConstant.quote_start_date);
    //    //ObjectSerializer.WriteSupported("stock.dat", stock1);
    //    ObjectSerializer.DeSerializeObject("stock.dat", ref stock1);

    //    var stopwatch = new Stopwatch();

    //    #region binary serializer CStockQuoteVolume with file
    //    {
    //        Console.WriteLine("BinarySerializer with file");
    //        BinarySerializer.SerializeObject("stock2.dat", stock1);
    //        stock2 = BinarySerializer.DeSerializeObject("stock2.dat", stock1.GetType());
    //        Console.WriteLine(stock1.Equals(stock2));
    //        Console.WriteLine("\n\n");
    //    }
    //    #endregion

    //    #region binary serializer CStockQuoteVolume with memory stream
    //    {
    //        Console.WriteLine("BinarySerializer with memory tream");
    //        using (MemoryStream stream = new MemoryStream())
    //        {
    //            BinarySerializer.SerializeObject(stream, stock1);
    //            stream.Position = 0;
    //            stock2 = BinarySerializer.DeSerializeObject(stream, stock1.GetType());
    //            Console.WriteLine(stock1.Equals(stock2));
    //            Console.WriteLine("\n\n");
    //        }
    //    }
    //    #endregion
    //    #region protobuf CStockQuoteVolume with file
    //    {
    //        Console.WriteLine("ProtoBut with filestream");
    //        stopwatch.Reset();
    //        stopwatch.Start();
    //        ObjectSerializer.ProtoBuf_Serialize("stock1.dat", stock1);
    //        stopwatch.Stop();
    //        Console.WriteLine("Proto serialize time: {0}", stopwatch.ElapsedMilliseconds);
    //        stopwatch.Reset();
    //        stopwatch.Start();
    //        ObjectSerializer.ProtoBuf_DeSerialize("stock1.dat", out stock2);
    //        stopwatch.Stop();
    //        Console.WriteLine("Proto de-serialize time: {0}", stopwatch.ElapsedMilliseconds);
    //        Console.WriteLine(stock1.Equals(stock2));
    //        Console.WriteLine("\n\n");
    //    }
    //    #endregion
    //    #region protobuf CStockQuoteVolume with memory
    //    {
    //        Console.WriteLine("ProtoBuf with memory stream");
    //        using (MemoryStream stream = new MemoryStream())
    //        {
    //            stopwatch.Reset();
    //            stopwatch.Start();
    //            ProtoBuf.Serializer.Serialize(stream, stock1);
    //            stopwatch.Stop();
    //            Console.WriteLine("Binary serialize time: {0}; stream length = {1}", stopwatch.ElapsedMilliseconds, stream.Length);
    //            stopwatch.Reset();
    //            stopwatch.Start();
    //            stream.Position = 0;
    //            stock2 = ProtoBuf.Serializer.Deserialize<CStockQuoteVolume>(stream);
    //            stopwatch.Stop();
    //            Console.WriteLine("Proto de-serialize time: {0}", stopwatch.ElapsedMilliseconds);
    //            Console.WriteLine(stock1.Equals(stock2));
    //        }
    //    }
    //    #endregion

    //    Console.WriteLine("\n\n-----------------------------\nTest ppForAllStocks of Stocks");
    //    var list1 = new List<CStockQuoteVolume>();
    //    list1.AddRange(Enumerable.Repeat(stock1, 10));

    //    #region binary serialize List<CStockQuoteVolume> with file
    //    {
    //        Console.WriteLine("BinarySerializer with file");
    //        BinarySerializer.SerializeObject("list1.dat", list1);
    //        var list2 = BinarySerializer.DeSerializeObject("list1.dat", list1.GetType());
    //        Console.WriteLine(BinarySerializer.CompareObjects(list1, list2));
    //        Console.WriteLine("\n\n");
    //    }
    //    #endregion
    //    #region binary serialie List<CStockQuoteVolume> with memory
    //    {
    //        Console.WriteLine("BinarySerializer with memory tream");
    //        using (MemoryStream stream = new MemoryStream())
    //        {
    //            BinarySerializer.SerializeObject(stream, list1);
    //            stream.Position = 0;
    //            var list2 = BinarySerializer.DeSerializeObject(stream, list1.GetType());
    //            Console.WriteLine(BinarySerializer.CompareObjects(list1, list2));
    //            Console.WriteLine("\n\n");
    //        }
    //    }
    //    #endregion
    //    #region protobuf serialize List<CStockQuoteVolume> with file
    //    {
    //        Console.WriteLine("ProtoBut with filestream");
    //        stopwatch.Reset();
    //        stopwatch.Start();
    //        ObjectSerializer.ProtoBuf_Serialize("list1.dat", list1);
    //        stopwatch.Stop();
    //        Console.WriteLine("Proto serialize time: {0}", stopwatch.ElapsedMilliseconds);
    //        stopwatch.Reset();
    //        stopwatch.Start();
    //        var list2 = ProtoBuf.Serializer.Deserialize<List<CStockQuoteVolume>>(File.OpenRead("list1.dat"));
    //        stopwatch.Stop();
    //        Console.WriteLine("Proto de-serialize time: {0}", stopwatch.ElapsedMilliseconds);
    //        Console.WriteLine(BinarySerializer.CompareObjects(list1, list2));
    //        Console.WriteLine("\n\n");
    //    }
    //    #endregion
    //    #region protobuf serialize List<CStockQuoteVolume> with memory
    //    {
    //        Console.WriteLine("ProtoBuf with memory stream");
    //        using (MemoryStream stream = new MemoryStream())
    //        {
    //            stopwatch.Reset();
    //            stopwatch.Start();
    //            ProtoBuf.Serializer.Serialize(stream, list1);
    //            stopwatch.Stop();
    //            Console.WriteLine("Binary serialize time: {0}; stream length = {1}", stopwatch.ElapsedMilliseconds, stream.Length);
    //            stopwatch.Reset();
    //            stopwatch.Start();
    //            stream.Position = 0;
    //            var list2 = ProtoBuf.Serializer.Deserialize<List<CStockQuoteVolume>>(stream);
    //            stopwatch.Stop();
    //            Console.WriteLine("Proto de-serialize time: {0}", stopwatch.ElapsedMilliseconds);
    //            Console.WriteLine(BinarySerializer.CompareObjects(list1, list2));
    //        }
    //    }
    //    #endregion

    //    Console.WriteLine("");
    //}
    #endregion

    class TestSerialize
    {
    }
}
