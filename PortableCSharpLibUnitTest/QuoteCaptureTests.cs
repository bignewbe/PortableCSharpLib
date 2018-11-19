using Microsoft.VisualStudio.TestTools.UnitTesting;
using PortableCSharpLib;
using PortableCSharpLib.TechnicalAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace UnitTest
{
    [TestClass]
    public class TestQuoteCapture
    {
        private QuoteCapture _quoteCapture;
        private string fileName = "tmp.dat";

        [TestInitialize]
        public void MethodInit()
        {
            _quoteCapture = new QuoteCapture("symbol0");
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
        }


        [TestMethod]
        public void TestAdd()
        {
            _quoteCapture.Add(10, 20);
            Assert.AreEqual(10, _quoteCapture.Time[0]);
            Assert.AreEqual(20, _quoteCapture.Price[0]);

            _quoteCapture.Add(20, 30);
            Assert.AreEqual(20, _quoteCapture.Time[1]);
            Assert.AreEqual(30, _quoteCapture.Price[1]);
        }

        [TestMethod]
        public void TestAssign()
        {
            IList<long> time = new List<long>();
            IList<double> price = new List<double>();
            IList<double> volume = new List<double>();
            time.Add(10);
            price.Add(20);
            volume.Add(30);

            IQuoteCapture qc = new QuoteCapture("symbol", 1, time, price, volume);
            _quoteCapture.Assign(qc);
            Assert.AreEqual("symbol", _quoteCapture.Symbol);
            Assert.AreEqual(1, _quoteCapture.PipFactor);
            Assert.AreEqual(10, _quoteCapture.Time[0]);
            Assert.AreEqual(20, _quoteCapture.Price[0]);
            Assert.AreEqual(30, _quoteCapture.Volume[0]);
        }

        [TestMethod]
        public void TestAdd_LongDoubleDouble()
        {
            _quoteCapture.Add(10, 20, 30);
            Assert.AreEqual(10, _quoteCapture.Time[0]);
            Assert.AreEqual(20, _quoteCapture.Price[0]);
            Assert.AreEqual(30, _quoteCapture.Volume[0]);
        }

        [TestMethod]
        public void TestAppend()
        {
            IQuoteCapture qc = null;
            // 测试null
            _quoteCapture.Append(qc);
            Assert.AreEqual(0, _quoteCapture.Count);

            // 测试空
            qc = new QuoteCapture();
            _quoteCapture.Append(qc);
            Assert.AreEqual(0, _quoteCapture.Count);

            // 测试不同的symbol
            qc = new QuoteCapture("symbol1");
            qc.Add(10, 20, 30);
            _quoteCapture.Append(qc);
            Assert.AreEqual(0, _quoteCapture.Count);

            // 测试_quoteCapter.LastTime < qc.LastTime
            qc = new QuoteCapture("symbol0");
            _quoteCapture.Add(10, 20, 15);
            qc.Add(5, 20);
            _quoteCapture.Append(qc);
            Assert.AreEqual(1, _quoteCapture.Count);
            Assert.AreEqual(10, _quoteCapture.Time[0]);
            Assert.AreEqual(20, _quoteCapture.Price[0]);
            Assert.AreEqual(15, _quoteCapture.Volume[0]);

            // 测试不添加小于LastTime的数据
            qc.Add(10, 18, 18);
            qc.Add(20, 30, 17);
            _quoteCapture.Append(qc);
            Assert.AreEqual(2, _quoteCapture.Count);
            Assert.AreEqual(10, _quoteCapture.Time[0]);
            Assert.AreEqual(20, _quoteCapture.Time[1]);
            Assert.AreEqual(20, _quoteCapture.Price[0]);
            Assert.AreEqual(30, _quoteCapture.Price[1]);
            Assert.AreEqual(15, _quoteCapture.Volume[0]);
            Assert.AreEqual(17, _quoteCapture.Volume[1]);
        }

        [TestMethod]
        public void TestExtract_LongLong()
        {
            _quoteCapture.Add(100, 10, 21);
            _quoteCapture.Add(200, 20, 22);
            _quoteCapture.Add(300, 30, 23);
            _quoteCapture.Add(400, 40, 24);
            _quoteCapture.Add(500, 50, 26);

            IQuoteCapture qc = _quoteCapture.Extract(150L, 350L);
            Assert.AreEqual(2, qc.Count);
            Assert.AreEqual(200, qc.Time[0]);
            Assert.AreEqual(300, qc.Time[1]);
            Assert.AreEqual(20, qc.Price[0]);
            Assert.AreEqual(30, qc.Price[1]);
            Assert.AreEqual(30, qc.Price[1]);
            Assert.AreEqual(22, qc.Volume[0]);
            Assert.AreEqual(23, qc.Volume[1]);
            qc = _quoteCapture.Extract(10L, 110L);
            Assert.AreEqual(1, qc.Count);
            Assert.AreEqual(100, qc.Time[0]);

            qc = _quoteCapture.Extract(10L, 20L);
            Assert.AreEqual(0, qc.Count);

            qc = _quoteCapture.Extract(600L, 900L);
            Assert.AreEqual(0, qc.Count);

            qc = _quoteCapture.Extract(0L, 900L);
            Assert.AreEqual(5, qc.Count);

            _quoteCapture.Clear();
            _quoteCapture.Add(100, 10);

            qc = _quoteCapture.Extract(80L, 110L);
            Assert.AreEqual(1, qc.Count);
        }

        [TestMethod]
        public void TestExtract_IntInt()
        {
            IQuoteCapture qc = null;

            // 测试sindex > eindex
            TestHelper.AssertException(() => _quoteCapture.Extract(1, 0), typeof(ArgumentException));

            // 测试sindex < 0
            TestHelper.AssertException(() => _quoteCapture.Extract(-1, 0), typeof(ArgumentException));

            // 测试eindex < 0
            TestHelper.AssertException(() => _quoteCapture.Extract(0, -1), typeof(ArgumentException));

            // 测试eindex > _quoteCapture.Count - 1;
            _quoteCapture.Add(10, 12);
            TestHelper.AssertException(() => _quoteCapture.Extract(0, 5), typeof(ArgumentException));

            _quoteCapture = new QuoteCapture("xyz");
            _quoteCapture.Add(100, 10, 23);
            _quoteCapture.Add(200, 20, 24);
            _quoteCapture.Add(300, 30, 26);
            _quoteCapture.Add(400, 40, 27);
            _quoteCapture.Add(500, 50, 28);

            qc = _quoteCapture.Extract(1, 3);
            Assert.AreEqual(3, qc.Count);
            Assert.AreEqual(200, qc.Time[0]);
            Assert.AreEqual(300, qc.Time[1]);
            Assert.AreEqual(400, qc.Time[2]);
            Assert.AreEqual(20, qc.Price[0]);
            Assert.AreEqual(30, qc.Price[1]);
            Assert.AreEqual(40, qc.Price[2]);
            Assert.AreEqual(24, qc.Volume[0]);
            Assert.AreEqual(26, qc.Volume[1]);
            Assert.AreEqual(27, qc.Volume[2]);
        }

        #region load/append stream
        [TestMethod]
        public void TestLoadStream_Normal()
        {
            //test robutstness to empty line, tab, space etc.
            var str = @" Symbol:DAG_2;PipFactor:5   
                        Time;  Price; Volume;

                        1465235480;3531;0

                        1465235485;3532;0";

            var quoteCapture = new QuoteCapture("DAG_2", 5, new List<long>(), new List<double>(), new List<double>());
            File.WriteAllText(fileName, str);

            using (var stream = File.OpenRead(fileName))
                quoteCapture.LoadStream(stream);
            Assert.AreEqual(quoteCapture.Count, 2);
            Assert.AreEqual(quoteCapture.Time[0], 1465235480);
            Assert.AreEqual(quoteCapture.Price[0], 3531);
            Assert.AreEqual(quoteCapture.Time[1], 1465235485);
            Assert.AreEqual(quoteCapture.Price[1], 3532);
        }


        /// <summary>
        /// test fail when content contains illegal data 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FileFormatNotSupportedException))]
        public void TestLoad_IllegalContent()
        {
            //test robutstness to empty line, tab, space etc.
            var str = @" 
                        Symbol:DAG_2;PipFactor:5   
                        Time;   Price;

                        1465235480;3531
                        1465235485;3532_illegal";

            var quoteCapture = new QuoteCapture("DAG_2", 5, new List<long>(), new List<double>(), new List<double>());
            File.WriteAllText(fileName, str);

            using (var stream = File.OpenRead(fileName))
                quoteCapture.LoadStream(stream);
        }

        [TestMethod]
        public void TestLoad_Space()
        {
            //test robutstness to empty line, tab, space etc.
            var str = @" Symbol:DAG_2;PipFactor:5   
                        Time;   Price;Volume;

                        1465235480;3531;0
                        1465235485;3532;0";

            var quoteCapture = new QuoteCapture("DAG_2", 5, new List<long>(), new List<double>(), new List<double>());
            File.WriteAllText(fileName, str);

            using (var stream = File.OpenRead(fileName))
                quoteCapture.LoadStream(stream);
            Assert.AreEqual(quoteCapture.Count, 2);
            Assert.AreEqual(quoteCapture.Time[0], 1465235480);
            Assert.AreEqual(quoteCapture.Price[0], 3531);
            Assert.AreEqual(quoteCapture.Time[1], 1465235485);
            Assert.AreEqual(quoteCapture.Price[1], 3532);

        }

        /// <summary>
        /// test quote doesnt load when symbol does not match
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FileFormatNotSupportedException))]
        public void TestLoad_SymbolNotMatch()
        {
            var str = @"  Symbol:DAG_3;PipFactor:5   
                        Time;   Price;

                        1465235480;3531
                        1465235485;3532";

            var quoteCapture = new QuoteCapture("DAG_2", 5, new List<long>(), new List<double>(), new List<double>());
            File.WriteAllText(fileName, str);

            using (var stream = File.OpenRead(fileName))
                quoteCapture.LoadStream(stream);
        }

        [TestMethod]
        [ExpectedException(typeof(FileFormatNotSupportedException))]
        public void TestLoad_PipFactorNotMatch()
        {
            var str = @"  Symbol:DAG_2;PipFactor:51
                        Time;   Price;

                        1465235480;3531
                        1465235485;3532";

            var quoteCapture = new QuoteCapture("DAG_2", 5, new List<long>(), new List<double>(), new List<double>());
            File.WriteAllText(fileName, str);

            using (var stream = File.OpenRead(fileName))
                quoteCapture.LoadStream(stream);
        }

        [TestMethod]
        [ExpectedException(typeof(FileFormatNotSupportedException))]
        public void TestLoad_IllegalHeader()
        {
            //test robutstness to empty line, tab, space etc.
            // Time;Price; not in the same line
            var str = @" Symbol:DAG_2;PipFactor:5
                        Time;   
                        Price;

                        1465235480;3531
                        1465235485;3532";

            var quoteCapture = new QuoteCapture("DAG_2", 5, new List<long>(), new List<double>(), new List<double>());
            File.WriteAllText(fileName, str);

            using (var stream = File.OpenRead(fileName))
                quoteCapture.LoadStream(stream);
        }

        [TestMethod]
        public void TestAppendStream_Normal()
        {
            var quoteBasicCapture = new QuoteCapture("DAG_2", 5, new List<long>(), new List<double>(), new List<double>());

            for (int i = 0; i <= 2; i++)
                quoteBasicCapture.Add(i, i);

            using (var stream = File.OpenWrite(fileName))
                quoteBasicCapture.AppendStream(stream);
            var contentStr = File.ReadAllText(fileName);
            Assert.AreEqual(contentStr, "Symbol:DAG_2;PipFactor:5\r\nTime;Price;Volume;\r\n0;0;0\r\n1;1;0\r\n2;2;0\r\n");
            var time1 = File.GetLastWriteTime(fileName);
            Thread.Sleep(100);
            using (var stream = File.OpenWrite(fileName))
                quoteBasicCapture.AppendStream(stream);
            // test overrite existing file
            contentStr = File.ReadAllText(fileName);
            Assert.AreEqual(contentStr, "Symbol:DAG_2;PipFactor:5\r\nTime;Price;Volume;\r\n0;0;0\r\n1;1;0\r\n2;2;0\r\n");
            var time2 = File.GetLastWriteTime(fileName);

            Assert.IsTrue(time2 > time1);
            File.Delete(fileName);
        }

        [TestMethod]
        public void TestAppendStream_Append()
        {
            var quoteBasic = new QuoteCapture("DAG_2", 5, new List<long>(), new List<double>(), new List<double>());
            for (int i = 0; i <= 2; i++)
                quoteBasic.Add(i, i);

            using (var stream = File.OpenWrite(fileName))
                quoteBasic.AppendStream(stream);

            using (var stream = new FileStream(fileName, FileMode.Append, FileAccess.Write))
                quoteBasic.AppendStream(stream);

            var contentStr = File.ReadAllText(fileName);
            Assert.AreEqual(contentStr, "Symbol:DAG_2;PipFactor:5\r\nTime;Price;Volume;\r\n0;0;0\r\n1;1;0\r\n2;2;0\r\n0;0;0\r\n1;1;0\r\n2;2;0\r\n");
        }

        [TestMethod]
        [ExpectedException(typeof(FileFormatNotSupportedException))]
        public void TestAppendStream_NoData()
        {
            var quoteCapture = new QuoteCapture("DAG_2", 5, new List<long>(), new List<double>(), new List<double>());
            using (var stream = File.OpenWrite(fileName))
                quoteCapture.AppendStream(stream);

            using (var stream = File.OpenRead(fileName))
                quoteCapture.LoadStream(stream);
        }
        #endregion
        #region compress and uncompress QC
        #region compress
        [TestMethod]
        public void TestCompress_Normal()
        {//测试基本功能
            TestHelper.AssertException(() => QuoteCapture.Compress(null), typeof(ArgumentNullException));
            Assert.AreEqual(QuoteCapture.Compress(_quoteCapture).Count, 0);
            //重复数据在开始
            _quoteCapture.Add(1, 1);
            _quoteCapture.Add(2, 1);
            _quoteCapture.Add(3, 1);
            _quoteCapture.Add(4, 2);
            var result = QuoteCapture.Compress(_quoteCapture);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 3);
            Assert.AreEqual(result.Time[0], 1);
            Assert.AreEqual(result.Price[0], 1);
            Assert.AreEqual(result.Time[1], 3);
            Assert.AreEqual(result.Price[1], -1);
            Assert.AreEqual(result.Time[2], 4);
            Assert.AreEqual(result.Price[2], 2);
            //重复数据在末尾
            _quoteCapture.Clear();
            _quoteCapture.Add(1, 1);
            _quoteCapture.Add(2, 2);
            _quoteCapture.Add(3, 2);
            _quoteCapture.Add(4, 2);
            result = QuoteCapture.Compress(_quoteCapture);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 3);
            Assert.AreEqual(result.Time[0], 1);
            Assert.AreEqual(result.Price[0], 1);
            Assert.AreEqual(result.Time[1], 2);
            Assert.AreEqual(result.Price[1], 2);
            Assert.AreEqual(result.Time[2], 4);
            Assert.AreEqual(result.Price[2], -1);
            //重复数据在中间
            _quoteCapture.Clear();
            _quoteCapture.Add(1, 1);
            _quoteCapture.Add(2, 2);
            _quoteCapture.Add(3, 2);
            _quoteCapture.Add(4, 2);
            _quoteCapture.Add(5, 3);
            result = QuoteCapture.Compress(_quoteCapture);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 4);
            Assert.AreEqual(result.Time[0], 1);
            Assert.AreEqual(result.Price[0], 1);
            Assert.AreEqual(result.Time[1], 2);
            Assert.AreEqual(result.Price[1], 2);
            Assert.AreEqual(result.Time[2], 4);
            Assert.AreEqual(result.Price[2], -1);
            Assert.AreEqual(result.Time[3], 5);
            Assert.AreEqual(result.Price[3], 3);
            //复合数据 开始中间末尾都有重复数据
            _quoteCapture.Clear();
            _quoteCapture.Add(1, 1);
            _quoteCapture.Add(2, 1);
            _quoteCapture.Add(3, 1);

            _quoteCapture.Add(4, 2);
            _quoteCapture.Add(5, 2);
            _quoteCapture.Add(6, 2);
            _quoteCapture.Add(7, 2);
            _quoteCapture.Add(9, 2);

            _quoteCapture.Add(10, 3);
            _quoteCapture.Add(11, 3);
            _quoteCapture.Add(12, 3);

            result = QuoteCapture.Compress(_quoteCapture);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 7);
            Assert.AreEqual(result.Time[0], 1);
            Assert.AreEqual(result.Price[0], 1);
            Assert.AreEqual(result.Time[1], 3);
            Assert.AreEqual(result.Price[1], -1);
            Assert.AreEqual(result.Time[2], 4);
            Assert.AreEqual(result.Price[2], 2);
            Assert.AreEqual(result.Time[3], 7);
            Assert.AreEqual(result.Price[3], -1);

            Assert.AreEqual(result.Time[4], 9);
            Assert.AreEqual(result.Price[4], 2);
            Assert.AreEqual(result.Time[5], 10);
            Assert.AreEqual(result.Price[5], 3);
            Assert.AreEqual(result.Time[6], 12);
            Assert.AreEqual(result.Price[6], -1);
        }

        [TestMethod]
        public void TestCompress_Safety()
        {
            //不压缩重复数据
            _quoteCapture.Add(1, 1);
            _quoteCapture.Add(1, 1);
            _quoteCapture.Add(2, 2);
            _quoteCapture.Add(3, 2);
            _quoteCapture.Add(3, 2);
            _quoteCapture.Add(4, 2);
            _quoteCapture.Add(5, 2);
            _quoteCapture.Add(6, 7);
            var result = QuoteCapture.Compress(_quoteCapture);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 7);
            Assert.AreEqual(result.Time[0], 1);
            Assert.AreEqual(result.Price[0], 1);
            Assert.AreEqual(result.Time[1], 1);
            Assert.AreEqual(result.Price[1], 1);

            Assert.AreEqual(result.Time[2], 2);
            Assert.AreEqual(result.Price[2], 2);
            Assert.AreEqual(result.Time[3], 3);
            Assert.AreEqual(result.Price[3], -1);

            Assert.AreEqual(result.Time[4], 3);
            Assert.AreEqual(result.Price[4], 2);
            Assert.AreEqual(result.Time[5], 5);
            Assert.AreEqual(result.Price[5], -1);
            Assert.AreEqual(result.Time[6], 6);
            Assert.AreEqual(result.Price[6], 7);

            //time时间突然变小也不压缩
            _quoteCapture.Clear();
            _quoteCapture.Add(3, 1);
            _quoteCapture.Add(2, 1);
            _quoteCapture.Add(1, 1);
            _quoteCapture.Add(3, 2);
            _quoteCapture.Add(4, 2);
            _quoteCapture.Add(5, 2);
            result = QuoteCapture.Compress(_quoteCapture);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 5);
            Assert.AreEqual(result.Time[0], 3);
            Assert.AreEqual(result.Price[0], 1);
            Assert.AreEqual(result.Time[1], 2);
            Assert.AreEqual(result.Price[1], 1);
            Assert.AreEqual(result.Time[2], 1);
            Assert.AreEqual(result.Price[2], 1);
            Assert.AreEqual(result.Time[3], 3);
            Assert.AreEqual(result.Price[3], 2);
            Assert.AreEqual(result.Time[4], 5);
            Assert.AreEqual(result.Price[4], -1);

            //时间间隔大于1也不压缩
            _quoteCapture.Clear();
            _quoteCapture.Add(3, 2);
            _quoteCapture.Add(6, 2);
            _quoteCapture.Add(9, 2);
            _quoteCapture.Add(15, 2);
            result = QuoteCapture.Compress(_quoteCapture);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 4);
            Assert.AreEqual(result.Time[0], 3);
            Assert.AreEqual(result.Price[0], 2);
            Assert.AreEqual(result.Time[1], 6);
            Assert.AreEqual(result.Price[1], 2);
            Assert.AreEqual(result.Time[2], 9);
            Assert.AreEqual(result.Price[2], 2);
            Assert.AreEqual(result.Time[3], 15);
            Assert.AreEqual(result.Price[3], 2);

            //测试负值情况
            _quoteCapture.Clear();
            _quoteCapture.Add(3, 2);
            _quoteCapture.Add(-2, 2);
            _quoteCapture.Add(-1, 2);
            _quoteCapture.Add(0, 2);
            result = QuoteCapture.Compress(_quoteCapture);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 3);
            Assert.AreEqual(result.Time[0], 3);
            Assert.AreEqual(result.Price[0], 2);
            Assert.AreEqual(result.Time[1], -2);
            Assert.AreEqual(result.Price[1], 2);
            Assert.AreEqual(result.Time[2], 0);
            Assert.AreEqual(result.Price[2], -1);
        }
        #endregion
        #region uncompress

        [TestMethod]
        public void TestUncompress_Normal()
        {
            TestHelper.AssertException(() => QuoteCapture.Uncompress(null), typeof(ArgumentNullException));
            Assert.AreEqual(QuoteCapture.Uncompress(_quoteCapture).Count, 0);
            //重复数据在开始
            _quoteCapture.Add(1, 1);
            _quoteCapture.Add(3, -1);
            _quoteCapture.Add(4, 2);
            var result = QuoteCapture.Uncompress(_quoteCapture);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 4);
            Assert.AreEqual(result.Time[0], 1);
            Assert.AreEqual(result.Price[0], 1);
            Assert.AreEqual(result.Time[1], 2);
            Assert.AreEqual(result.Price[1], 1);
            Assert.AreEqual(result.Time[2], 3);
            Assert.AreEqual(result.Price[2], 1);
            Assert.AreEqual(result.Time[3], 4);
            Assert.AreEqual(result.Price[3], 2);
            //重复数据在末尾
            _quoteCapture.Clear();
            _quoteCapture.Add(1, 1);
            _quoteCapture.Add(2, 2);
            _quoteCapture.Add(5, -1);
            result = QuoteCapture.Uncompress(_quoteCapture);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 5);
            Assert.AreEqual(result.Time[0], 1);
            Assert.AreEqual(result.Price[0], 1);
            Assert.AreEqual(result.Time[1], 2);
            Assert.AreEqual(result.Price[1], 2);
            Assert.AreEqual(result.Time[2], 3);
            Assert.AreEqual(result.Price[2], 2);
            Assert.AreEqual(result.Time[3], 4);
            Assert.AreEqual(result.Price[3], 2);
            Assert.AreEqual(result.Time[4], 5);
            Assert.AreEqual(result.Price[4], 2);
            //重复数据在中间
            _quoteCapture.Clear();
            _quoteCapture.Add(1, 1);
            _quoteCapture.Add(2, 2);
            _quoteCapture.Add(4, -1);
            _quoteCapture.Add(5, 6);
            result = QuoteCapture.Uncompress(_quoteCapture);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 5);
            Assert.AreEqual(result.Time[0], 1);
            Assert.AreEqual(result.Price[0], 1);
            Assert.AreEqual(result.Time[1], 2);
            Assert.AreEqual(result.Price[1], 2);
            Assert.AreEqual(result.Time[2], 3);
            Assert.AreEqual(result.Price[2], 2);
            Assert.AreEqual(result.Time[3], 4);
            Assert.AreEqual(result.Price[3], 2);
            Assert.AreEqual(result.Time[4], 5);
            Assert.AreEqual(result.Price[4], 6);
            //复合重复数据 开始中间结尾都有
            _quoteCapture.Clear();
            _quoteCapture.Add(1, 1);
            _quoteCapture.Add(3, -1);
            _quoteCapture.Add(4, 2);
            _quoteCapture.Add(6, -1);
            _quoteCapture.Add(7, 3);
            _quoteCapture.Add(8, 4);
            _quoteCapture.Add(10, -1);
            result = QuoteCapture.Uncompress(_quoteCapture);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 10);

            Assert.AreEqual(result.Time[0], 1);
            Assert.AreEqual(result.Price[0], 1);
            Assert.AreEqual(result.Time[1], 2);
            Assert.AreEqual(result.Price[1], 1);
            Assert.AreEqual(result.Time[2], 3);
            Assert.AreEqual(result.Price[2], 1);
            Assert.AreEqual(result.Time[3], 4);
            Assert.AreEqual(result.Price[3], 2);
            Assert.AreEqual(result.Time[4], 5);
            Assert.AreEqual(result.Price[4], 2);
            Assert.AreEqual(result.Time[5], 6);
            Assert.AreEqual(result.Price[5], 2);
            Assert.AreEqual(result.Time[6], 7);
            Assert.AreEqual(result.Price[6], 3);
            Assert.AreEqual(result.Time[7], 8);
            Assert.AreEqual(result.Price[7], 4);
            Assert.AreEqual(result.Time[8], 9);
            Assert.AreEqual(result.Price[8], 4);
            Assert.AreEqual(result.Time[9], 10);
            Assert.AreEqual(result.Price[9], 4);
        }

        [TestMethod]
        public void TestUncompress_Safety()
        {
            //只有1个数据的时候
            _quoteCapture.Add(5, -1);
            var result = QuoteCapture.Uncompress(_quoteCapture);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 1);
            Assert.AreEqual(result.Time[0], 5);
            Assert.AreEqual(result.Price[0], -1);

            //处理负数时间
            _quoteCapture.Clear();
            _quoteCapture.Add(-3, 1);
            _quoteCapture.Add(-1, -1);
            _quoteCapture.Add(5, 2);
            result = QuoteCapture.Uncompress(_quoteCapture);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 4);
            Assert.AreEqual(result.Time[0], -3);
            Assert.AreEqual(result.Price[0], 1);
            Assert.AreEqual(result.Time[1], -2);
            Assert.AreEqual(result.Price[1], 1);
            Assert.AreEqual(result.Time[2], -1);
            Assert.AreEqual(result.Price[2], 1);
            Assert.AreEqual(result.Time[3], 5);
            Assert.AreEqual(result.Price[3], 2);

            //处理非递增序列
            _quoteCapture.Clear();
            _quoteCapture.Add(0, 1);
            _quoteCapture.Add(-2, -1);
            _quoteCapture.Add(3, -1);//用-1作为分段起点
            _quoteCapture.Add(5, -1);
            result = QuoteCapture.Uncompress(_quoteCapture);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 5);
            Assert.AreEqual(result.Time[0], 0);
            Assert.AreEqual(result.Price[0], 1);
            Assert.AreEqual(result.Time[1], -2);
            Assert.AreEqual(result.Price[1], -1);
            Assert.AreEqual(result.Time[2], 3);
            Assert.AreEqual(result.Price[2], -1);
            Assert.AreEqual(result.Time[3], 4);
            Assert.AreEqual(result.Price[3], -1);
            Assert.AreEqual(result.Time[4], 5);
            Assert.AreEqual(result.Price[4], -1);
        }
        #endregion
        #region 功能测试
        //使用虚拟数据
        [TestMethod]
        public void TestCompressAndUncompress_Normal()
        {
            _quoteCapture.Add(1, 1);
            _quoteCapture.Add(2, 1);
            _quoteCapture.Add(3, 1);

            _quoteCapture.Add(4, 2);
            _quoteCapture.Add(5, 2);
            _quoteCapture.Add(6, 2);
            _quoteCapture.Add(7, 2);

            _quoteCapture.Add(9, 2);

            _quoteCapture.Add(10, 3);
            _quoteCapture.Add(11, 3);
            _quoteCapture.Add(12, 3);

            var compress = QuoteCapture.Compress(_quoteCapture);
            Assert.AreNotEqual(compress.Count, _quoteCapture.Count);

            var uncompress = QuoteCapture.Uncompress(compress);
            Assert.AreEqual(_quoteCapture.Count, uncompress.Count);
            Assert.AreEqual(_quoteCapture.Symbol, uncompress.Symbol);
            Assert.AreEqual(_quoteCapture.PipFactor, uncompress.PipFactor);

            for (var i = 0; i < uncompress.Count; i++)
            {
                Assert.AreEqual(_quoteCapture.Time[i], uncompress.Time[i]);
                Assert.AreEqual(_quoteCapture.Price[i], uncompress.Price[i]);
            }
        }
        //使用实际数据
        //[TestMethod]
        //public void TestCompressAndUncompress_Actual()
        //{
        //    string fn = "HuaTong_HuaTong_AgJC_2017_10.dat";
        //    //实际文件不存在则跳过测试
        //    if (!File.Exists(fn))
        //        return;
        //    IQuoteCapture qc = new QuoteCapture("HuaTong_HuaTong_AgJC");
        //    using (var stream = File.OpenRead(fn))
        //    {
        //        qc.LoadStream(stream);
        //        var result = QuoteCapture.Compress(qc);
        //        Assert.AreNotEqual(qc.Count, result.Count);
        //        using (var save = File.OpenWrite(fileName))
        //            result.AppendStream(save);

        //        var newCapture = QuoteCapture.Uncompress(result);
        //        Assert.AreEqual(qc.Count, newCapture.Count);

        //        Assert.AreEqual(qc.Symbol, newCapture.Symbol);
        //        Assert.AreEqual(qc.PipFactor, newCapture.PipFactor);
        //        for (var i = 0; i < newCapture.Count; i++)
        //        {
        //            Assert.AreEqual(newCapture.Time[i], qc.Time[i]);
        //            Assert.AreEqual(newCapture.Price[i], qc.Price[i]);
        //        }
        //    }

        //}
        #endregion
        #endregion
        #region test add monotonic
        [TestMethod]
        public void TestAddMonotonic_Normal()
        {
            //测试无数据是 添加
            _quoteCapture.AddMonotonic(-1, -11, 15);
            Assert.AreEqual(_quoteCapture.Time[0], -1);
            Assert.AreEqual(_quoteCapture.Price[0], -11);
            Assert.AreEqual(_quoteCapture.Volume[0], 15);
            _quoteCapture.Clear();
            _quoteCapture.AddMonotonic(2, 12, 16);
            Assert.AreEqual(_quoteCapture.Time[0], 2);
            Assert.AreEqual(_quoteCapture.Price[0], 12);
            Assert.AreEqual(_quoteCapture.Volume[0], 16);
            //测试已经有数据的情况下添加数据
            //添加=lastTime的数据
            Assert.AreEqual(_quoteCapture.Count, 1);
            _quoteCapture.AddMonotonic(2, 112, 100);
            Assert.AreEqual(_quoteCapture.Count, 2);
            Assert.AreEqual(_quoteCapture.Time[0], 2);
            Assert.AreEqual(_quoteCapture.Price[0], 12);
            Assert.AreEqual(_quoteCapture.Volume[0], 16);
            //测试时间<lastTime的数据
            _quoteCapture.AddMonotonic(1, 11, 100);
            //测试时间>lastTime的数据
            _quoteCapture.AddMonotonic(6, 16, 101);
            _quoteCapture.AddMonotonic(5, 13, 102);
            _quoteCapture.AddMonotonic(9, 19, 103);
            Assert.AreEqual(_quoteCapture.Count, 4);
            Assert.AreEqual(_quoteCapture.Time[0], 2);
            Assert.AreEqual(_quoteCapture.Price[0], 12);
            Assert.AreEqual(_quoteCapture.Volume[0], 16);
            Assert.AreEqual(_quoteCapture.Time[1], 2);
            Assert.AreEqual(_quoteCapture.Price[1], 112);
            Assert.AreEqual(_quoteCapture.Volume[1], 100);
            Assert.AreEqual(_quoteCapture.Time[2], 6);
            Assert.AreEqual(_quoteCapture.Price[2], 16);
            Assert.AreEqual(_quoteCapture.Volume[2], 101);
        }
        #endregion
        #region Test Sort
        [TestMethod]
        public void TestSort_Increase()
        {
            //无数据排序？
            _quoteCapture.Sort(false, true);
            Assert.AreEqual(_quoteCapture.Count, 0);
            //只有1个数据排序
            _quoteCapture.Add(5, 15, 101.5);
            _quoteCapture.Sort(false, true);
            Assert.AreEqual(_quoteCapture.Count, 1);
            Assert.AreEqual(_quoteCapture.Time[0], 5);
            Assert.AreEqual(_quoteCapture.Price[0], 15);
            Assert.AreEqual(_quoteCapture.Volume[0], 101.5);
            //测试无重复数据的递增排序
            _quoteCapture.Clear();
            _quoteCapture.Add(0, 10, 100.1);
            _quoteCapture.Add(-1, -11, 100.2);
            _quoteCapture.Add(-2, -12, 100.3);
            _quoteCapture.Add(6, 16, 100.4);
            _quoteCapture.Add(7, 17, 100.5);
            _quoteCapture.Add(3, 13, 100.6);
            _quoteCapture.Add(9, 19, 100.7);
            _quoteCapture.Add(4, 11, 100.8);
            _quoteCapture.Sort(false, true);
            Assert.AreEqual(_quoteCapture.Count, 8);
            Assert.AreEqual(_quoteCapture.Time[0], -2);
            Assert.AreEqual(_quoteCapture.Price[0], -12);
            Assert.AreEqual(_quoteCapture.Volume[0], 100.3);
            Assert.AreEqual(_quoteCapture.Time[1], -1);
            Assert.AreEqual(_quoteCapture.Price[1], -11);
            Assert.AreEqual(_quoteCapture.Volume[1], 100.2);
            Assert.AreEqual(_quoteCapture.Time[2], 0);
            Assert.AreEqual(_quoteCapture.Price[2], 10);
            Assert.AreEqual(_quoteCapture.Volume[2], 100.1);
            Assert.AreEqual(_quoteCapture.Time[3], 3);
            Assert.AreEqual(_quoteCapture.Price[3], 13);
            Assert.AreEqual(_quoteCapture.Volume[3], 100.6);
            Assert.AreEqual(_quoteCapture.Time[4], 4);
            Assert.AreEqual(_quoteCapture.Price[4], 11);
            Assert.AreEqual(_quoteCapture.Volume[4], 100.8);
            Assert.AreEqual(_quoteCapture.Time[5], 6);
            Assert.AreEqual(_quoteCapture.Price[5], 16);
            Assert.AreEqual(_quoteCapture.Volume[5], 100.4);
            Assert.AreEqual(_quoteCapture.Time[6], 7);
            Assert.AreEqual(_quoteCapture.Price[6], 17);
            Assert.AreEqual(_quoteCapture.Volume[6], 100.5);
            Assert.AreEqual(_quoteCapture.Time[7], 9);
            Assert.AreEqual(_quoteCapture.Price[7], 19);
            Assert.AreEqual(_quoteCapture.Volume[7], 100.7);
            //测试有重复数据的递增排序
            _quoteCapture.Clear();
            _quoteCapture.Add(-1, -11);
            _quoteCapture.Add(-2, -12);
            _quoteCapture.Add(-2, -12);
            _quoteCapture.Add(6, 16);
            _quoteCapture.Add(7, 17);
            _quoteCapture.Add(3, 13);
            _quoteCapture.Add(9, 19);
            _quoteCapture.Add(7, 17);
            _quoteCapture.Add(4, 11);
            _quoteCapture.Add(7, 17);
            _quoteCapture.Sort(false, true);
            Assert.AreEqual(_quoteCapture.Count, 10);
            Assert.AreEqual(_quoteCapture.Time[0], -2);
            Assert.AreEqual(_quoteCapture.Price[0], -12);
            Assert.AreEqual(_quoteCapture.Time[1], -2);
            Assert.AreEqual(_quoteCapture.Price[1], -12);
            Assert.AreEqual(_quoteCapture.Time[2], -1);
            Assert.AreEqual(_quoteCapture.Price[2], -11);
            Assert.AreEqual(_quoteCapture.Time[3], 3);
            Assert.AreEqual(_quoteCapture.Price[3], 13);
            Assert.AreEqual(_quoteCapture.Time[4], 4);
            Assert.AreEqual(_quoteCapture.Price[4], 11);
            Assert.AreEqual(_quoteCapture.Time[5], 6);
            Assert.AreEqual(_quoteCapture.Price[5], 16);
            Assert.AreEqual(_quoteCapture.Time[6], 7);
            Assert.AreEqual(_quoteCapture.Price[6], 17);
            Assert.AreEqual(_quoteCapture.Time[7], 7);
            Assert.AreEqual(_quoteCapture.Price[7], 17);
            Assert.AreEqual(_quoteCapture.Time[8], 7);
            Assert.AreEqual(_quoteCapture.Price[8], 17);
            Assert.AreEqual(_quoteCapture.Time[9], 9);
            Assert.AreEqual(_quoteCapture.Price[9], 19);
            //测试随机数据进行排序
            _quoteCapture.Clear();
            var random = new Random();
            for (var i = 0; i < 100; i++)
            {
                var num = random.Next(1000);
                _quoteCapture.Add(num, (double)num - 10);
            }
            _quoteCapture.Sort(false, true);
            Assert.AreEqual(_quoteCapture.Count, 100);
            for (var i = 1; i < 100; i++)
            {
                Assert.IsTrue(_quoteCapture.Time[i] >= _quoteCapture.Time[i - 1]);
                Assert.AreEqual(_quoteCapture.Time[i], _quoteCapture.Price[i] + 10);
            }
        }
        [TestMethod]
        public void TestSort_Decrease()
        {
            //无数据排序？
            _quoteCapture.Sort(false, false);
            Assert.AreEqual(_quoteCapture.Count, 0);
            //只有1个数据排序
            _quoteCapture.Add(5, 15);
            _quoteCapture.Sort(false, false);
            Assert.AreEqual(_quoteCapture.Count, 1);
            Assert.AreEqual(_quoteCapture.Time[0], 5);
            Assert.AreEqual(_quoteCapture.Price[0], 15);
            //测试无重复数据的递减排序
            _quoteCapture.Clear();
            _quoteCapture.Add(0, 10);
            _quoteCapture.Add(-1, -11);
            _quoteCapture.Add(-2, -12);
            _quoteCapture.Add(6, 16);
            _quoteCapture.Add(7, 17);
            _quoteCapture.Add(3, 13);
            _quoteCapture.Add(9, 19);
            _quoteCapture.Add(4, 11);
            _quoteCapture.Sort(false, false);
            Assert.AreEqual(_quoteCapture.Count, 8);
            Assert.AreEqual(_quoteCapture.Time[0], 9);
            Assert.AreEqual(_quoteCapture.Price[0], 19);
            Assert.AreEqual(_quoteCapture.Time[1], 7);
            Assert.AreEqual(_quoteCapture.Price[1], 17);
            Assert.AreEqual(_quoteCapture.Time[2], 6);
            Assert.AreEqual(_quoteCapture.Price[2], 16);
            Assert.AreEqual(_quoteCapture.Time[3], 4);
            Assert.AreEqual(_quoteCapture.Price[3], 11);
            Assert.AreEqual(_quoteCapture.Time[4], 3);
            Assert.AreEqual(_quoteCapture.Price[4], 13);
            Assert.AreEqual(_quoteCapture.Time[5], 0);
            Assert.AreEqual(_quoteCapture.Price[5], 10);
            Assert.AreEqual(_quoteCapture.Time[6], -1);
            Assert.AreEqual(_quoteCapture.Price[6], -11);
            Assert.AreEqual(_quoteCapture.Time[7], -2);
            Assert.AreEqual(_quoteCapture.Price[7], -12);
            //测试有重复数据的递减排序
            _quoteCapture.Clear();
            _quoteCapture.Add(-1, -11);
            _quoteCapture.Add(-2, -12);
            _quoteCapture.Add(-2, -12);
            _quoteCapture.Add(6, 16);
            _quoteCapture.Add(7, 17);
            _quoteCapture.Add(3, 13);
            _quoteCapture.Add(9, 19);
            _quoteCapture.Add(7, 17);
            _quoteCapture.Add(4, 11);
            _quoteCapture.Add(7, 17);
            _quoteCapture.Sort(false, false);
            Assert.AreEqual(_quoteCapture.Count, 10);
            Assert.AreEqual(_quoteCapture.Time[0], 9);
            Assert.AreEqual(_quoteCapture.Price[0], 19);
            Assert.AreEqual(_quoteCapture.Time[1], 7);
            Assert.AreEqual(_quoteCapture.Price[1], 17);
            Assert.AreEqual(_quoteCapture.Time[2], 7);
            Assert.AreEqual(_quoteCapture.Price[2], 17);
            Assert.AreEqual(_quoteCapture.Time[3], 7);
            Assert.AreEqual(_quoteCapture.Price[3], 17);
            Assert.AreEqual(_quoteCapture.Time[4], 6);
            Assert.AreEqual(_quoteCapture.Price[4], 16);
            Assert.AreEqual(_quoteCapture.Time[5], 4);
            Assert.AreEqual(_quoteCapture.Price[5], 11);
            Assert.AreEqual(_quoteCapture.Time[6], 3);
            Assert.AreEqual(_quoteCapture.Price[6], 13);
            Assert.AreEqual(_quoteCapture.Time[7], -1);
            Assert.AreEqual(_quoteCapture.Price[7], -11);
            Assert.AreEqual(_quoteCapture.Time[8], -2);
            Assert.AreEqual(_quoteCapture.Price[8], -12);
            Assert.AreEqual(_quoteCapture.Time[9], -2);
            Assert.AreEqual(_quoteCapture.Price[9], -12);
            //测试随机数据进行排序
            _quoteCapture.Clear();
            var random = new Random();
            for (var i = 0; i < 100; i++)
            {
                var num = random.Next(1000);
                _quoteCapture.Add(num, (double)num - 10);
            }
            _quoteCapture.Sort(false, false);
            Assert.AreEqual(_quoteCapture.Count, 100);
            for (var i = 1; i < 100; i++)
            {
                Assert.IsTrue(_quoteCapture.Time[i] <= _quoteCapture.Time[i - 1]);
                Assert.AreEqual(_quoteCapture.Time[i], _quoteCapture.Price[i] + 10);
            }
        }

        [TestMethod]
        public void TestSort_Distinct()
        {
            //无数据排序？
            _quoteCapture.Sort(true, false);
            Assert.AreEqual(_quoteCapture.Count, 0);
            _quoteCapture.Add(5, 15);
            _quoteCapture.Sort(true, false);
            Assert.AreEqual(_quoteCapture.Count, 1);
            Assert.AreEqual(_quoteCapture.Time[0], 5);
            Assert.AreEqual(_quoteCapture.Price[0], 15);
            //测试无重复数据的递减排序
            _quoteCapture.Clear();
            _quoteCapture.Add(0, 10);
            _quoteCapture.Add(-1, -11);
            _quoteCapture.Add(-2, -12);
            _quoteCapture.Add(6, 16);
            _quoteCapture.Add(7, 17);
            _quoteCapture.Add(3, 13);
            _quoteCapture.Add(9, 19);
            _quoteCapture.Add(4, 11);
            _quoteCapture.Sort(true, false);
            Assert.AreEqual(_quoteCapture.Count, 8);
            Assert.AreEqual(_quoteCapture.Time[0], 9);
            Assert.AreEqual(_quoteCapture.Price[0], 19);
            Assert.AreEqual(_quoteCapture.Time[1], 7);
            Assert.AreEqual(_quoteCapture.Price[1], 17);
            Assert.AreEqual(_quoteCapture.Time[2], 6);
            Assert.AreEqual(_quoteCapture.Price[2], 16);
            Assert.AreEqual(_quoteCapture.Time[3], 4);
            Assert.AreEqual(_quoteCapture.Price[3], 11);
            Assert.AreEqual(_quoteCapture.Time[4], 3);
            Assert.AreEqual(_quoteCapture.Price[4], 13);
            Assert.AreEqual(_quoteCapture.Time[5], 0);
            Assert.AreEqual(_quoteCapture.Price[5], 10);
            Assert.AreEqual(_quoteCapture.Time[6], -1);
            Assert.AreEqual(_quoteCapture.Price[6], -11);
            Assert.AreEqual(_quoteCapture.Time[7], -2);
            Assert.AreEqual(_quoteCapture.Price[7], -12);

            //测试有重复数据的递减排序
            _quoteCapture.Clear();
            _quoteCapture.Add(-1, -11);
            _quoteCapture.Add(-2, -12);
            _quoteCapture.Add(-2, -12);
            _quoteCapture.Add(6, 16);
            _quoteCapture.Add(7, 17);
            _quoteCapture.Add(3, 13);
            _quoteCapture.Add(9, 19);
            _quoteCapture.Add(7, 17);
            _quoteCapture.Add(4, 11);
            _quoteCapture.Add(7, 17);
            _quoteCapture.Sort(true, false);
            Assert.AreEqual(_quoteCapture.Count, 7);
            Assert.AreEqual(_quoteCapture.Time[0], 9);
            Assert.AreEqual(_quoteCapture.Price[0], 19);
            Assert.AreEqual(_quoteCapture.Time[1], 7);
            Assert.AreEqual(_quoteCapture.Price[1], 17);
            Assert.AreEqual(_quoteCapture.Time[2], 6);
            Assert.AreEqual(_quoteCapture.Price[2], 16);
            Assert.AreEqual(_quoteCapture.Time[3], 4);
            Assert.AreEqual(_quoteCapture.Price[3], 11);
            Assert.AreEqual(_quoteCapture.Time[4], 3);
            Assert.AreEqual(_quoteCapture.Price[4], 13);
            Assert.AreEqual(_quoteCapture.Time[5], -1);
            Assert.AreEqual(_quoteCapture.Price[5], -11);
            Assert.AreEqual(_quoteCapture.Time[6], -2);
            Assert.AreEqual(_quoteCapture.Price[6], -12);

            //测试随机数据进行排序去重
            _quoteCapture.Clear();
            var random = new Random();
            for (var i = 0; i < 100; i++)
            {
                var num = random.Next(1000);
                _quoteCapture.Add(num, (double)num - 10);
            }
            _quoteCapture.Sort(true, true);
            Assert.IsTrue(_quoteCapture.Count <= 100);
            for (var i = 1; i < _quoteCapture.Count; i++)
            {
                Assert.IsTrue(_quoteCapture.Time[i] > _quoteCapture.Time[i - 1]);
                Assert.AreEqual(_quoteCapture.Time[i], _quoteCapture.Price[i] + 10);
            }
        }



        [TestMethod]
        public void TestSort_BigNumber()
        {
            Random rm = new Random();
            for (var i = 0; i < 50000; i++)
            {
                var time = rm.Next(1000);
                _quoteCapture.Add(time, time - 10);
            }
            _quoteCapture.Sort(true, true);
            for (var i = 1; i < _quoteCapture.Count; i++)
            {
                Assert.IsTrue(_quoteCapture.Time[i - 1] < _quoteCapture.Time[i]); ;
                Assert.AreEqual(_quoteCapture.Time[i] - 10, _quoteCapture.Price[i]);
            }

        }
        #endregion

        [TestMethod]
        public void TestDistinct_Normal()
        {
            //测试数据为空
            var result = _quoteCapture.Distinct();
            Assert.AreEqual(result.Count, 0);
            Assert.AreEqual(result.Symbol, _quoteCapture.Symbol);
            //测试重复数据在开头
            _quoteCapture.Clear();
            _quoteCapture.Add(1, 1, 100.1);
            _quoteCapture.Add(1, 2, 100.2);
            _quoteCapture.Add(2, 3, 100.3);
            result = _quoteCapture.Distinct();
            Assert.AreEqual(result.Symbol, _quoteCapture.Symbol);
            Assert.AreEqual(result.Count, 3);
            Assert.AreEqual(result.Time[0], 1);
            Assert.AreEqual(result.Price[0], 1);
            Assert.AreEqual(result.Volume[0], 100.1);
            Assert.AreEqual(result.Time[2], 2);
            Assert.AreEqual(result.Price[2], 3);
            Assert.AreEqual(result.Volume[2], 100.3);
            //重复数据在末尾
            _quoteCapture.Clear();
            _quoteCapture.Add(1, 1);
            _quoteCapture.Add(2, 3);
            _quoteCapture.Add(2, 4);
            result = _quoteCapture.Distinct();
            Assert.AreEqual(result.Count, 3);
            Assert.AreEqual(result.Time[0], 1);
            Assert.AreEqual(result.Price[0], 1);
            Assert.AreEqual(result.Time[2], 2);
            Assert.AreEqual(result.Price[2], 4);
            //重复数据在中间
            _quoteCapture.Clear();
            _quoteCapture.Add(1, 1);
            _quoteCapture.Add(2, 3);
            _quoteCapture.Add(2, 4);
            _quoteCapture.Add(4, 5);
            result = _quoteCapture.Distinct();
            Assert.AreEqual(result.Count, 4);
            Assert.AreEqual(result.Time[0], 1);
            Assert.AreEqual(result.Price[0], 1);
            Assert.AreEqual(result.Time[1], 2);
            Assert.AreEqual(result.Price[1], 3);
            Assert.AreEqual(result.Time[2], 2);
            Assert.AreEqual(result.Price[2], 4);
            //测试随机数据
            _quoteCapture.Clear();
            Random rm = new Random();
            for (var i = 0; i < 100; i++)
            {
                _quoteCapture.Add(i, i - 10);
                if (rm.Next(100) % 2 == 0)
                    _quoteCapture.Add(i, i - 10);
            }
            result = _quoteCapture.Distinct();
            Assert.AreEqual(result.Symbol, _quoteCapture.Symbol);
            for (var i = 1; i < result.Count; i++)
            {
                Assert.IsTrue(result.Time[i - 1] <= result.Time[i]); ;
                Assert.AreEqual(result.Time[i] - 10, result.Price[i]);
            }
        }
    }
}
