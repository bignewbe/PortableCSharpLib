using Microsoft.VisualStudio.TestTools.UnitTesting;
using PortableCSharpLib.TechnicalAnalysis;
using System.Collections.Generic;

namespace UnitTest
{
    [TestClass]
    public class QuoteCaptureTests
    {
        private QuoteCapture _quoteCapture;

        [TestInitialize]
        public void MethodInit()
        {
            _quoteCapture = new QuoteCapture("symbol0");
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
            time.Add(10);
            price.Add(20);

            IQuoteCapture qc = new QuoteCapture("symbol", 1, time, price);
            _quoteCapture.Assign(qc);
            Assert.AreEqual("symbol", _quoteCapture.Symbol);
            Assert.AreEqual(1, _quoteCapture.PipFactor);
            Assert.AreEqual(10, _quoteCapture.Time[0]);
            Assert.AreEqual(20, _quoteCapture.Price[0]);
        }

        [TestMethod]
        public void TestAdd_LongDoubleDouble()
        {
            _quoteCapture.Add(10, 20, 30);
            Assert.AreEqual(10, _quoteCapture.Time[0]);
            Assert.AreEqual(25, _quoteCapture.Price[0]);
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
            qc.Add(10, 20);
            _quoteCapture.Append(qc);
            Assert.AreEqual(0, _quoteCapture.Count);

            // 测试_quoteCapter.LastTime < qc.LastTime
            qc = new QuoteCapture("symbol0");
            _quoteCapture.Add(10, 20);
            qc.Add(5, 20);
            _quoteCapture.Append(qc);
            Assert.AreEqual(1, _quoteCapture.Count);
            Assert.AreEqual(10, _quoteCapture.Time[0]);
            Assert.AreEqual(20, _quoteCapture.Price[0]);

            // 测试不添加小于LastTime的数据
            qc.Add(10, 20);
            qc.Add(20, 30);
            _quoteCapture.Append(qc);
            Assert.AreEqual(2, _quoteCapture.Count);
            Assert.AreEqual(10, _quoteCapture.Time[0]);
            Assert.AreEqual(20, _quoteCapture.Time[1]);
            Assert.AreEqual(20, _quoteCapture.Price[0]);
            Assert.AreEqual(30, _quoteCapture.Price[1]);
        }

        [TestMethod]
        public void TestExtract_LongLong()
        {
            _quoteCapture.Add(100, 10);
            _quoteCapture.Add(200, 20);
            _quoteCapture.Add(300, 30);
            _quoteCapture.Add(400, 40);
            _quoteCapture.Add(500, 50);

            IQuoteCapture qc = _quoteCapture.Extract(150L, 350L);
            Assert.AreEqual(2, qc.Count);
            Assert.AreEqual(200, qc.Time[0]);
            Assert.AreEqual(300, qc.Time[1]);
            Assert.AreEqual(20, qc.Price[0]);
            Assert.AreEqual(30, qc.Price[1]);

            qc = _quoteCapture.Extract(10L, 110L);
            Assert.AreEqual(1, qc.Count);
            Assert.AreEqual(100, qc.Time[0]);

            qc = _quoteCapture.Extract(10L, 20L);
            Assert.IsNull(qc);

            qc = _quoteCapture.Extract(600L, 900L);
            Assert.IsNull(qc);
        }

        [TestMethod]
        public void TestExtract_IntInt()
        {
            IQuoteCapture qc = null;

            // 测试sindex > eindex
            qc = _quoteCapture.Extract(1, 0);
            Assert.IsNull(qc);

            // 测试sindex < 0
            qc = _quoteCapture.Extract(-1, 0);
            Assert.IsNull(qc);

            // 测试eindex < 0
            qc = _quoteCapture.Extract(0, -1);
            Assert.IsNull(qc);

            // 测试eindex > _quoteCapture.Count - 1;
            qc = _quoteCapture.Extract(0, 1);
            Assert.IsNull(qc);

            _quoteCapture.Add(100, 10);
            _quoteCapture.Add(200, 20);
            _quoteCapture.Add(300, 30);
            _quoteCapture.Add(400, 40);
            _quoteCapture.Add(500, 50);

            qc = _quoteCapture.Extract(1, 3);
            Assert.AreEqual(3, qc.Count);
            Assert.AreEqual(200, qc.Time[0]);
            Assert.AreEqual(300, qc.Time[1]);
            Assert.AreEqual(400, qc.Time[2]);
            Assert.AreEqual(20, qc.Price[0]);
            Assert.AreEqual(30, qc.Price[1]);
            Assert.AreEqual(40, qc.Price[2]);
        }
    }
}
