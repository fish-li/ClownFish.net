using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data;
using ClownFish.Data.MultiDB.MySQL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Data.MySQL
{
    [TestClass]
    public class MySqlUtilsTest
    {
        [TestMethod]
        public void Test_RegisterMySqlProvider()
        {
            MySqlUtils.RegisterProvider(2);
            BaseClientProvider client1 = DbClientFactory.GetProvider(DatabaseClients.MySqlClient);
            Assert.AreEqual(MySqlConnectorClientProvider.Instance, client1);
            Assert.AreEqual("MySql.Data.MySqlClient", MySqlUtils.GetProviderName());
        }


    }
}
