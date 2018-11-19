using Microsoft.VisualStudio.TestTools.UnitTesting;
using PortableCSharpLib.TechnicalAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass]
    public class QuoteBasicTests
    {
        private QuoteBasic _quoteBasic;

        [TestInitialize]
        public void MethodInit()
        {
            _quoteBasic = new QuoteBasic("symbol1", 60);
        }


        [TestMethod]
        public void TestAdd()
        {
            _quoteBasic.Add(600, 1, 1, 1, 1, 1);
            Assert.AreEqual(1, _quoteBasic.Count);
            Assert.AreEqual(600, _quoteBasic.LastTime);

            // 小于lastTime时
            _quoteBasic.Add(10, 1, 1, 1, 1, 1);
            Assert.AreEqual(1, _quoteBasic.Count);
            Assert.AreEqual(600, _quoteBasic.LastTime);
        }

        [TestMethod]
        public void TestAppendIQuoteBasic()
        {
            _quoteBasic.Add(600, 1, 1, 1, 1, 1); // 600

            // 非整数倍
            IQuoteBasic qb = new QuoteBasic("symbol1", 20);
            qb.Add(640, 1, 1, 1, 1, 1);
            _quoteBasic.Append(qb); // 600
            Assert.AreEqual(1, _quoteBasic.Count);
            Assert.AreEqual(600, _quoteBasic.LastTime);

            // 整数倍
            qb.Add(660, 1, 1, 1, 1, 1);
            _quoteBasic.Append(qb);
            Assert.AreEqual(2, _quoteBasic.Count);
            Assert.AreEqual(600, _quoteBasic.Time[0]);
            Assert.AreEqual(660, _quoteBasic.Time[1]);

            // 为空
            qb.Clear();
            _quoteBasic.Append(qb);
            Assert.AreEqual(2, _quoteBasic.Count);
            Assert.AreEqual(600, _quoteBasic.Time[0]);
            Assert.AreEqual(660, _quoteBasic.Time[1]); ;

            // 为null
            qb = null;
            _quoteBasic.Append(qb);
            Assert.AreEqual(2, _quoteBasic.Count);
            Assert.AreEqual(600, _quoteBasic.Time[0]);
            Assert.AreEqual(660, _quoteBasic.Time[1]);

            // 不填补空洞
            qb = new QuoteBasic("symbol1", 20);
            qb.Add(6000, 1, 1, 1, 1, 1);
            _quoteBasic.Append(qb);
            Assert.AreEqual(3, _quoteBasic.Count);
            Assert.AreEqual(600, _quoteBasic.Time[0]);
            Assert.AreEqual(660, _quoteBasic.Time[1]);
            Assert.AreEqual(6000, _quoteBasic.Time[2]);

        }

        [TestMethod]
        public void TestAppendIQuoteBasic_SubInterval()
        {
            _quoteBasic.Add(600, 1, 1, 1, 1, 1); // 600

            IQuoteBasic qb = new QuoteBasic("symbol1", 20);
            qb.Add(620, 1, 1, 1, 1, 1);
            _quoteBasic.Append(qb, 20); // 600, 620
            Assert.AreEqual(2, _quoteBasic.Count);
            Assert.AreEqual(600, _quoteBasic.Time[0]);
            Assert.AreEqual(620, _quoteBasic.Time[1]);

            qb.Clear();
            qb.Add(640, 1, 1, 1, 1, 1);
            _quoteBasic.Append(qb, 20); // 600, 640
            Assert.AreEqual(2, _quoteBasic.Count);
            Assert.AreEqual(600, _quoteBasic.Time[0]);
            Assert.AreEqual(640, _quoteBasic.Time[1]);

            qb.Add(680, 1, 1, 1, 1, 1);
            _quoteBasic.Append(qb, 20); // 600, 660, 680
            Assert.AreEqual(3, _quoteBasic.Count);
            Assert.AreEqual(600, _quoteBasic.Time[0]);
            Assert.AreEqual(660, _quoteBasic.Time[1]);
            Assert.AreEqual(680, _quoteBasic.Time[2]);

            qb.Add(700, 1, 1, 1, 1, 1);
            _quoteBasic.Append(qb, 20); // 600, 660, 700
            Assert.AreEqual(3, _quoteBasic.Count);
            Assert.AreEqual(600, _quoteBasic.Time[0]);
            Assert.AreEqual(660, _quoteBasic.Time[1]);
            Assert.AreEqual(700, _quoteBasic.Time[2]);

            qb.Add(740, 1, 1, 1, 1, 1);
            qb.Add(760, 1, 1, 1, 1, 1);
            _quoteBasic.Append(qb, 20); // 600, 660, 720, 760
            Assert.AreEqual(4, _quoteBasic.Count);
            Assert.AreEqual(600, _quoteBasic.Time[0]);
            Assert.AreEqual(660, _quoteBasic.Time[1]);
            Assert.AreEqual(720, _quoteBasic.Time[2]);
            Assert.AreEqual(760, _quoteBasic.Time[3]);

            qb = new QuoteBasic("symbol1", 1);
            qb.Add(761, 1, 1, 1, 1, 1);
            qb.Add(762, 1, 1, 1, 1, 1);
            qb.Add(763, 1, 1, 1, 1, 1);
            _quoteBasic.Append(qb, 1); // 600, 660, 720, 763
            Assert.AreEqual(4, _quoteBasic.Count);
            Assert.AreEqual(600, _quoteBasic.Time[0]);
            Assert.AreEqual(660, _quoteBasic.Time[1]);
            Assert.AreEqual(720, _quoteBasic.Time[2]);
            Assert.AreEqual(763, _quoteBasic.Time[3]);

            qb.Add(780, 1, 1, 1, 1, 1);
            _quoteBasic.Append(qb, 20); // 600, 660, 720, 780
            Assert.AreEqual(4, _quoteBasic.Count);
            Assert.AreEqual(600, _quoteBasic.Time[0]);
            Assert.AreEqual(660, _quoteBasic.Time[1]);
            Assert.AreEqual(720, _quoteBasic.Time[2]);
            Assert.AreEqual(780, _quoteBasic.Time[3]);
        }

        [TestMethod]
        public void TestAppendIQuoteBasic_FillGap()
        {
            _quoteBasic.Add(600, 1, 1, 1, 1, 1);

            IQuoteBasic qb = new QuoteBasic("symbol1", 10);

            // 不填充
            qb.Add(720, 1, 1, 1, 1, 1);
            _quoteBasic.Append(qb, 10, false);
            Assert.AreEqual(600, _quoteBasic.Time[0]);
            Assert.AreEqual(720, _quoteBasic.Time[1]);

            // 填充
            _quoteBasic.Clear();
            _quoteBasic.Add(600, 1, 1, 1, 1, 1);
            _quoteBasic.Append(qb, 10, true);
            Assert.AreEqual(600, _quoteBasic.Time[0]);
            Assert.AreEqual(660, _quoteBasic.Time[1]);
            Assert.AreEqual(720, _quoteBasic.Time[2]);

            qb.Add(840, 12, 1, 1, 1, 1);
            _quoteBasic.Append(qb, 10, true);
            Assert.AreEqual(840, _quoteBasic.Time[4]);

            qb.Add(890, 12, 1, 1, 1, 1);
            _quoteBasic.Append(qb, 10, true);
            Assert.AreEqual(840, _quoteBasic.Time[4]);

            qb.Add(910, 12, 1, 1, 1, 1);
            _quoteBasic.Append(qb, 10, true);
            Assert.AreEqual(900, _quoteBasic.Time[5]);
            Assert.AreEqual(910, _quoteBasic.Time[6]);
        }

        [TestMethod]
        public void TestAppendIQuoteCapture()
        {
            _quoteBasic.Add(600, 1, 1, 1, 1, 1); // 600

            // 非整数倍
            IQuoteCapture qc = new QuoteCapture("symbol1");
            qc.Add(620, 20);
            _quoteBasic.Append(qc);
            Assert.AreEqual(1, _quoteBasic.Count);
            Assert.AreEqual(600, _quoteBasic.LastTime);

            // 整数倍
            qc.Add(660, 60);
            _quoteBasic.Append(qc);
            Assert.AreEqual(2, _quoteBasic.Count);
            Assert.AreEqual(600, _quoteBasic.Time[0]);
            Assert.AreEqual(660, _quoteBasic.Time[1]);

            // 为空
            qc = new QuoteCapture("symbol1");
            _quoteBasic.Append(qc);
            Assert.AreEqual(2, _quoteBasic.Count);
            Assert.AreEqual(600, _quoteBasic.Time[0]);
            Assert.AreEqual(660, _quoteBasic.Time[1]); ;

            // 为null
            qc = null;
            _quoteBasic.Append(qc);
            Assert.AreEqual(2, _quoteBasic.Count);
            Assert.AreEqual(600, _quoteBasic.Time[0]);
            Assert.AreEqual(660, _quoteBasic.Time[1]);

            //// 不填补空洞
            //qb = new QuoteBasic("symbol1", 20);
            //qb.Add(6000, 1, 1, 1, 1, 1, false);
            //_quoteBasic.Append(qb);
            //Assert.AreEqual(3, _quoteBasic.Count);
            //Assert.AreEqual(600, _quoteBasic.Time[0]);
            //Assert.AreEqual(660, _quoteBasic.Time[1]);
            //Assert.AreEqual(6000, _quoteBasic.Time[2]);
        }

        [TestMethod]
        public void TestAppendIQuoteCapture_SubInterval()
        {
            _quoteBasic.Add(600, 1, 1, 1, 1, 1, false); // 600

            IQuoteCapture qc = new QuoteCapture("symbol1");

            qc.Add(620, 20);
            _quoteBasic.Append(qc, 20); // 600, 620
            Assert.AreEqual(2, _quoteBasic.Count);
            Assert.AreEqual(600, _quoteBasic.Time[0]);
            Assert.AreEqual(620, _quoteBasic.Time[1]);

            qc = new QuoteCapture("symbol1");
            qc.Add(640, 40);
            _quoteBasic.Append(qc, 20); // 600, 640
            Assert.AreEqual(2, _quoteBasic.Count);
            Assert.AreEqual(600, _quoteBasic.Time[0]);
            Assert.AreEqual(640, _quoteBasic.Time[1]);

            qc.Add(680, 80);
            _quoteBasic.Append(qc, 20); // 600, 660, 680
            Assert.AreEqual(3, _quoteBasic.Count);
            Assert.AreEqual(600, _quoteBasic.Time[0]);
            Assert.AreEqual(660, _quoteBasic.Time[1]);
            Assert.AreEqual(680, _quoteBasic.Time[2]);

            qc.Add(700, 100);
            _quoteBasic.Append(qc, 20); // 600, 660, 700
            Assert.AreEqual(3, _quoteBasic.Count);
            Assert.AreEqual(600, _quoteBasic.Time[0]);
            Assert.AreEqual(660, _quoteBasic.Time[1]);
            Assert.AreEqual(700, _quoteBasic.Time[2]);

            qc.Add(740, 140);
            qc.Add(760, 160);
            _quoteBasic.Append(qc, 20); // 600, 660, 720, 760
            Assert.AreEqual(4, _quoteBasic.Count);
            Assert.AreEqual(600, _quoteBasic.Time[0]);
            Assert.AreEqual(660, _quoteBasic.Time[1]);
            Assert.AreEqual(720, _quoteBasic.Time[2]);
            Assert.AreEqual(760, _quoteBasic.Time[3]);

            qc = new QuoteCapture("symbol1");
            qc.Add(761, 1);
            qc.Add(762, 1);
            qc.Add(763, 1);
            _quoteBasic.Append(qc, 1);
            Assert.AreEqual(4, _quoteBasic.Count);
            Assert.AreEqual(600, _quoteBasic.Time[0]);
            Assert.AreEqual(660, _quoteBasic.Time[1]);
            Assert.AreEqual(720, _quoteBasic.Time[2]);
            Assert.AreEqual(763, _quoteBasic.Time[3]);

            qc.Add(780, 90);
            _quoteBasic.Append(qc, 20);
            Assert.AreEqual(4, _quoteBasic.Count);
            Assert.AreEqual(600, _quoteBasic.Time[0]);
            Assert.AreEqual(660, _quoteBasic.Time[1]);
            Assert.AreEqual(720, _quoteBasic.Time[2]);
            Assert.AreEqual(780, _quoteBasic.Time[3]);
        }

        [TestMethod]
        public void TestAppendIQuoteCapture_FillGap()
        {
            _quoteBasic.Add(600, 1, 1, 1, 1, 1);

            IQuoteCapture qc = new QuoteCapture("symbol1");

            // 不填充
            qc.Add(720, 12);
            _quoteBasic.Append(qc, 10, false);
            Assert.AreEqual(600, _quoteBasic.Time[0]);
            Assert.AreEqual(720, _quoteBasic.Time[1]);

            // 填充
            _quoteBasic.Clear();
            _quoteBasic.Add(600, 1, 1, 1, 1, 1);
            _quoteBasic.Append(qc, 10, true);
            Assert.AreEqual(600, _quoteBasic.Time[0]);
            Assert.AreEqual(660, _quoteBasic.Time[1]);
            Assert.AreEqual(720, _quoteBasic.Time[2]);

            qc.Add(840, 12);
            _quoteBasic.Append(qc, 10, true);
            Assert.AreEqual(840, _quoteBasic.Time[4]);

            qc.Add(890, 12);
            _quoteBasic.Append(qc, 10, true);
            Assert.AreEqual(840, _quoteBasic.Time[4]);

            qc.Add(910, 12);
            _quoteBasic.Append(qc, 10, true);
            Assert.AreEqual(900, _quoteBasic.Time[5]);
            Assert.AreEqual(910, _quoteBasic.Time[6]);

        }

        [TestMethod]
        public void TestAddEvent()
        {
            // 测试事件列表为空时
            AssertNoException(() => _quoteBasic.Add(100, 1, 1, 1, 1, 1, true), typeof(NullReferenceException));
            // 测试事件触发
            _quoteBasic.QuoteBasicDataAdded += TestAddEventInternal;
            AssertException(() =>
                _quoteBasic.Add(600, 1, 1, 1, 1, 1, true), null);

            // 测试不触发事件
            _quoteBasic.QuoteBasicDataAdded += TestAddEventInternal;
            AssertNoException(() => _quoteBasic.Add(100, 1, 1, 1, 1, 1), null);
        }

        private void TestAddEventInternal(object sender, string symbol, int interval, long time, double open, double close, double high, double low, double volume)
        {
            throw new Exception();
        }

        [TestMethod]
        public void TestFindIndexForGivenTime()
        {
            Assert.AreEqual(-1, _quoteBasic.FindIndexForGivenTime(100, false));

            _quoteBasic.Add(100, 1, 1, 1, 1, 1, false);
            Assert.AreEqual(0, _quoteBasic.FindIndexForGivenTime(100, false));

            _quoteBasic.Add(200, 1, 1, 1, 1, 1, false);
            Assert.AreEqual(0, _quoteBasic.FindIndexForGivenTime(150, true));
            Assert.AreEqual(-1, _quoteBasic.FindIndexForGivenTime(150, false));
        }

        [TestMethod]
        public void TestFindIndexWhereTimeLocated()
        {
            Assert.AreEqual(-1, _quoteBasic.FindIndexWhereTimeLocated(100));

            _quoteBasic.Add(100, 1, 1, 1, 1, 1, false);
            _quoteBasic.Add(200, 1, 1, 1, 1, 1, false);
            Assert.AreEqual(1, _quoteBasic.FindIndexWhereTimeLocated(150));
            Assert.AreEqual(-1, _quoteBasic.FindIndexWhereTimeLocated(300));
            Assert.AreEqual(-1, _quoteBasic.FindIndexWhereTimeLocated(-1));
        }

        [TestMethod]
        public void TestClear()
        {
            _quoteBasic.Clear();
            Assert.AreEqual(0, _quoteBasic.Count);

            _quoteBasic.Add(100, 1, 1, 1, 1, 1, false);
            _quoteBasic.Clear();
            Assert.AreEqual(0, _quoteBasic.Count);
        }

        [TestMethod]
        public void TestInsert()
        {
            QuoteBasic qb2 = new QuoteBasic("symbol1", 60);
            _quoteBasic.Insert(qb2);
            Assert.AreEqual(0, _quoteBasic.Count);

            qb2.Add(100, 1, 1, 1, 1, 1, false);
            _quoteBasic.Insert(qb2);
            Assert.AreEqual(1, _quoteBasic.Count);

            _quoteBasic.Clear();
            qb2.Clear();
            _quoteBasic.Add(100, 1, 1, 1, 1, 1, false);
            _quoteBasic.Add(500, 1, 1, 1, 1, 1, false);
            qb2.Add(10, 1, 1, 1, 1, 1, false);
            _quoteBasic.Insert(qb2);
            Assert.AreEqual(10, _quoteBasic.FirstTime);

            qb2.Clear();
            qb2.Add(600, 1, 1, 1, 1, 1, false);
            _quoteBasic.Insert(qb2);
            Assert.AreEqual(600, _quoteBasic.LastTime);
        }

        [TestMethod]
        public void TestExtract_LongLong()
        {
            IQuoteBasic qb1 = _quoteBasic.Extract(0L, 0L);
            Assert.IsNull(qb1);

            _quoteBasic.Add(100, 1, 1, 1, 1, 1, false);
            _quoteBasic.Add(200, 1, 1, 1, 1, 1, false);
            _quoteBasic.Add(300, 1, 1, 1, 1, 1, false);
            _quoteBasic.Add(400, 1, 1, 1, 1, 1, false);
            _quoteBasic.Add(500, 1, 1, 1, 1, 1, false);

            qb1 = _quoteBasic.Extract(250L, 350L);
            Assert.AreEqual(1, qb1.Count);
            Assert.AreEqual(300, qb1.Time[0]);

            qb1 = _quoteBasic.Extract(250L, 400L);
            Assert.AreEqual(2, qb1.Count);
            Assert.AreEqual(300, qb1.Time[0]);
            Assert.AreEqual(400, qb1.Time[1]);


            qb1 = _quoteBasic.Extract(350L, 250L);
            Assert.IsNull(qb1);

            qb1 = _quoteBasic.Extract(550L, 750L);
            Assert.IsNull(qb1);
        }

        [TestMethod]
        public void TestExtract_IntInt()
        {
            AssertException(() => _quoteBasic.Extract(0, 0));

            //IQuoteBasic qb1 = _quoteBasic.Extract(0, 0);
            //Assert.IsNull(qb1);

            _quoteBasic.Add(100, 1, 1, 1, 1, 1, false);
            _quoteBasic.Add(200, 1, 1, 1, 1, 1, false);
            _quoteBasic.Add(300, 1, 1, 1, 1, 1, false);
            _quoteBasic.Add(400, 1, 1, 1, 1, 1, false);
            _quoteBasic.Add(500, 1, 1, 1, 1, 1, false);

            var qb1 = _quoteBasic.Extract(1, 3);
            Assert.AreEqual(3, qb1.Count);

            AssertException(()=> _quoteBasic.Extract(3, 1));
            AssertException(() => _quoteBasic.Extract(5, 7));
        }


        // 注册一个委托，在该委托中抛出异常。
        // 在action中调用可能会抛出异常的方法
        // e为null表示任何异常。一般用于测试事件触发
        public void AssertException(Action action, Type e = null)
        {
            bool isSuccessed = false;
            try {
                action.Invoke();
            }
            catch (Exception ex) {
                if (e == null || e == ex.GetType()) {
                    isSuccessed = true;
                }
            }
            if (!isSuccessed) {
                Assert.Fail();
            }
        }

        // 测试没有抛出指定的异常
        public void AssertNoException(Action action, Type e = null)
        {
            bool isSuccessed = false;
            try {
                action.Invoke();
            }
            catch (Exception ex) {
                if (e == null || e == ex.GetType()) {
                    isSuccessed = true;
                }
            }
            if (isSuccessed) {
                Assert.Fail();
            }
        }

    }
}
