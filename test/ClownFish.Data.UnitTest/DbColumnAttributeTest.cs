using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Data.UnitTest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Data.UnitTest
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
    }
}
