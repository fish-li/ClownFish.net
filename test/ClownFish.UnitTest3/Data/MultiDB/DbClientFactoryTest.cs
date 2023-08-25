using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data;
using ClownFish.Data.MultiDB.MsSQL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Data.MultiDB
{
    [TestClass]
    public class DbClientFactoryTest
    {
        [TestMethod]
        public void Test()
        {

            DbProviderFactory factory4 = DbClientFactory.GetDbProviderFactory("Microsoft.Data.SqlClient");
            Assert.AreEqual(Microsoft.Data.SqlClient.SqlClientFactory.Instance, factory4);


        }


        [TestMethod]
        public void Test2()
        {
            using( DbContext dbContext = DbContext.Create("sqlserver") ) {

                string value1 = dbContext.CPQuery.Create("select getdate();").ExecuteScalar<string>();
                Console.WriteLine(dbContext.Connection.GetType().FullName);

                Assert.IsTrue(dbContext.Connection.GetType().FullName.StartsWith("Microsoft.Data.SqlClient.SqlConnection"));
            }

        }



        [TestMethod]
        public void Test_Error()
        {
            MyAssert.IsError<NotSupportedException>(() => {
                _ = DbClientFactory.GetDbProviderFactory("xxx");
            });


            MyAssert.IsError<ArgumentNullException>(() => {
                DbClientFactory.RegisterProvider(null, MsSqlClientProvider2.Instance);
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                DbClientFactory.RegisterProvider("xxx", null);
            });
        }

    }
}
