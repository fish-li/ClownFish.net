using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.UnitTest.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Data;

namespace ClownFish.UnitTest.Data.Attributes
{
    [TestClass]
    public class DbColumnAttributeTest : BaseTest
    {
        [TestMethod]
        public void TestDbColumnAttributeAlias()
        {
            string sql = "select top 1 * from dbo.Products";

            using( ConnectionScope scope = ConnectionScope.Create() ) {

                Product2 product = CPQuery.Create(sql).ToSingle<Product2>();

                Assert.IsNotNull(product);

                Console.Write(product.ToJson());

                Assert.IsNotNull(product.PName);                

                Assert.IsTrue(product.PName.Length > 0);

                
            }
        }


        [TestMethod]
        public void Error1()
        {
            MyAssert.IsError<ArgumentNullException>(() => {
                DbColumnAttribute attr = new DbColumnAttribute();
                var xx = new ColumnInfo(null, attr);
            });
        }
    }
}
