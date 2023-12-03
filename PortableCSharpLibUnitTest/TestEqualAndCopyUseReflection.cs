using Microsoft.VisualStudio.TestTools.UnitTesting;
using PortableCSharpLib.Util;
using System;

namespace PortableCSharpLibUnitTest
{
    class A : EqualAndCopyUseReflection<A>
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime Time { get; set; }
    }

    [TestClass]
    public class TestEqualAndCopyUseReflection
    {
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
        }

        [TestInitialize]
        public void Initialize()
        {
        }

        [TestCleanup]
        public void Cleanup()
        {
        }

        [TestMethod]
        public void TestCopyAndEqual()
        {
            var utcnow = DateTime.UtcNow;
            var a1 = new A { Name = "aaa", Time = utcnow };
            var a2 = new A();

            Assert.IsFalse(a2.Equals(a1));
            a2.Copy(a1);
            Assert.IsTrue(a2.Equals(a1));
        }
    }
}
