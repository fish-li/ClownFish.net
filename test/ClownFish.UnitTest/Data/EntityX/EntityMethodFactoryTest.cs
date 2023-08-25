using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data;
using ClownFish.Data.Internals;
using ClownFish.UnitTest.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Data.EntityX
{
    [TestClass]
    public class EntityMethodFactoryTest
    {
        [TestMethod]
        public void Test_Error()
        {
            MyAssert.IsError<ArgumentNullException>(() => {
                EntityMethodFactory factory = new EntityMethodFactory(null);
            });

        }

        [TestMethod]
        public void Test()
        {
            using( DbContext dbContext = DbContext.Create() ) {

                EntityMethodFactory factory = new EntityMethodFactory(dbContext);

                var table = factory.From<Customer>();
                Assert.IsNotNull(table);
                Assert.AreEqual(dbContext, table.GetFieldValue("_dbContext"));


                var query = factory.Query<Customer>();
                Assert.IsNotNull(query);
                Assert.AreEqual(dbContext, query.GetFieldValue("_provider").GetFieldValue("_dbContext"));

                Customer c1 = factory.BeginEdit<Customer>();
                IEntityProxy proxy1 = c1 as IEntityProxy;
                Assert.IsNotNull(proxy1);
                Assert.AreEqual(dbContext, proxy1.DbContext);


                Customer c2 = new Customer();
                Customer c3 = factory.BeginEdit(c2);
                IEntityProxy proxy2 = c3 as IEntityProxy;
                Assert.IsNotNull(proxy2);
                Assert.AreEqual(dbContext, proxy2.DbContext);


                MyAssert.IsError<InvalidOperationException>(() => {
                    Customer c4 = factory.BeginEdit(c3);
                });
            }
        }

    }
}
