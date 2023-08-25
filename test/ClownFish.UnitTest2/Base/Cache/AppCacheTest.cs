using System;
using System.Collections.Generic;
using System.Text;
using ClownFish.Base;
using ClownFish.UnitTest.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Cache
{
    [TestClass]
    public class AppCacheTest
    {
        [TestMethod]
        public void Test_GetObject()
        {
            string key = Guid.NewGuid().ToString();

            Product p1 = AppCache.GetObject<Product>(key);
            Assert.IsNull(p1);

            Product p2 = AppCache.GetObject<Product>(key, ()=> {
                return new Product { ProductID = 3, ProductName = "Name5" };
            });

            Assert.IsNotNull(p2);
            Assert.AreEqual("Name5", p2.ProductName);

            Assert.IsTrue(AppCache.GetCount() > 0);  // 这个判断仅仅为了代码覆盖，没其他用途！
        }

        [TestMethod]
        public void Test_SetObject()
        {
            string key = Guid.NewGuid().ToString();

            Product p1 = AppCache.GetObject<Product>(key);
            Assert.IsNull(p1);

            Product p2 = new Product { ProductID = 3, ProductName = "Name5" };
            AppCache.SetObject(key, p2, DateTime.Now.AddMinutes(1));

            p1 = AppCache.GetObject<Product>(key);
            Assert.IsNotNull(p1);

            Assert.AreEqual(p1, p2);


            AppCache.RemoveObject(key);

            Product p3 = AppCache.GetObject<Product>(key);
            Assert.IsNull(p3);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_GetObject_ArgsNull()
        {
            string key = null;
            Product value = AppCache.GetObject<Product>(key);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_SetObject_ArgsNull()
        {
            string key = null;
            AppCache.SetObject(key, new Product(), DateTime.Now.AddDays(1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_RemoveObject_ArgsNull()
        {
            string key = null;
            AppCache.RemoveObject(key);
        }

    }
}
