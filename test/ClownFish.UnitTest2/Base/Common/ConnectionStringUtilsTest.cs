using System;
using System.Collections.Generic;
using System.Text;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Common
{
    [TestClass]
    public class ConnectionStringUtilsTest
    {
        [TestMethod]
        public void Test()
        {
            string connectionString = "server=111;database=222;username=333;password=444;port=555;";
            string conn2 = ConnectionStringUtils.HidePwd(connectionString);
            Assert.AreEqual("server=111;database=222;username=333;password=********;port=555;", conn2);

            connectionString = "server=111;database=222;username=333;pwd=444;port=555;";
            conn2 = ConnectionStringUtils.HidePwd(connectionString);
            Assert.AreEqual("server=111;database=222;username=333;pwd=********;port=555;", conn2);

            connectionString = "server=111;database=222;username=333;pwd=444";
            conn2 = ConnectionStringUtils.HidePwd(connectionString);
            Assert.AreEqual("server=111;database=222;username=333;pwd=********", conn2);

            Assert.AreEqual(null, ConnectionStringUtils.HidePwd(null));
            Assert.AreEqual(string.Empty, ConnectionStringUtils.HidePwd(string.Empty));
        }
    }
}
