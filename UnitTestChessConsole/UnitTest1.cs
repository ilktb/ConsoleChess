using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestChessConsole
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Assert.AreEqual(2, 2, "They are equal.");
        }
    }
}
