using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using PortableCSharpLib;
using System.Collections.Generic;

namespace TestPortable
{
    [TestClass]
    public class TestGeneral
    {
        [TestMethod]
        public void TestBinarySearch()
        {
            Console.WriteLine(DateTime.UtcNow.GetIso8601WeekOfYear()); 

            //good weather
            var list = new List<int> { 1, 3, 6, 9, 10 };
            Assert.AreEqual(General.BinarySearch(list, 0, list.Count - 1, 6, false), 2);
            Assert.AreEqual(General.BinarySearch(list, 0, list.Count - 1, 5,  false), -1);
            Assert.AreEqual(General.BinarySearch(list, 0, list.Count - 1, 0,  false), -1);
            Assert.AreEqual(General.BinarySearch(list, 0, list.Count - 1, 12, false), -1);

            Assert.AreEqual(General.BinarySearch(list, 0, list.Count - 1, 6, true), 2);
            Assert.AreEqual(General.BinarySearch(list, 0, list.Count - 1, 5, true), 1);
            Assert.AreEqual(General.BinarySearch(list, 0, list.Count - 1, 0,  true), -1);
            Assert.AreEqual(General.BinarySearch(list, 0, list.Count - 1, 12, true), -1);

            //bad weather
            Assert.AreEqual(General.BinarySearch(null, 0, list.Count - 1, 3, true), -1);
            Assert.AreEqual(General.BinarySearch(list, -1, list.Count - 1, 3, true), -1);
            Assert.AreEqual(General.BinarySearch(list, -1, list.Count, 3, true), -1);
        }
    }
}
