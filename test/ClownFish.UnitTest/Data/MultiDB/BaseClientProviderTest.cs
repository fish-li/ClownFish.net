using System;
using System.Data.Common;
using ClownFish.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Data.MultiDB
{
    [TestClass]
    public class BaseClientProviderTest
    {
        [TestMethod]
        public void Test()
        {
            BaseClientProvider client = DbClientFactory.GetProvider(XxxBaseClientProvider.Name);

            Assert.AreEqual(DatabaseType.Unknow, client.DatabaseType);
            Assert.AreEqual(XxxProviderFactory.Instance, client.ProviderFactory);
            Assert.AreEqual("/abc/", client.GetObjectFullName("abc"));
            Assert.AreEqual("@abc", client.GetParamterName("abc"));
            Assert.AreEqual("@abc", client.GetParamterPlaceholder("abc"));

            MyAssert.IsError<NotImplementedException>(() => {
                client.GetPagedCommand(null, null);
            });

            Assert.IsFalse(client.IsDuplicateInsertException(null));

            MyAssert.IsError<ArgumentNullException>(() => {
                StdClientProvider.CreatePageQuery(null, new PagingInfo(), "xxx", "xxx");
            });


            using( DbContext db = DbContext.Create("sqlserver") ) {

                var query = db.CPQuery.Create("select getdate()");

                MyAssert.IsError<ArgumentNullException>(() => {
                    StdClientProvider.CreatePageQuery(query, null,  "xxx", "xxx");
                });
            }
        }
    }


    public class XxxBaseClientProvider : BaseClientProvider
    {
        public static readonly BaseClientProvider Instance = new XxxBaseClientProvider();

        public static readonly string Name = "Testx";

        public override DatabaseType DatabaseType => DatabaseType.Unknow;

        public override DbProviderFactory ProviderFactory => XxxProviderFactory.Instance;

        public override string GetObjectFullName(string symbol)
        {
            return "/" + base.GetObjectFullName(symbol) + "/";
        }


    }

    public class XxxProviderFactory : DbProviderFactory
    {
        public static XxxProviderFactory Instance = new XxxProviderFactory();
    }
}
