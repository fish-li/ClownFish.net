using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data;
using ClownFish.UnitTest.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Data.Utils
{
    [TestClass]
    public class EntityHelperTest
    {
        [TestMethod]
        public void Test_GetDbFieldName()
        {
            PropertyInfo p1 = typeof(OrderDetailX2).GetProperty("Quantity");
            Assert.AreEqual("Quantity", p1.GetDbFieldName());


            PropertyInfo p2 = typeof(OrderDetailX3).GetProperty("Quantity");
            Assert.AreEqual("Quantity2", p2.GetDbFieldName());


            PropertyInfo p3 = typeof(OrderDetailX3).GetProperty("Remark");
            Assert.AreEqual("Remark", p3.GetDbFieldName());
        }


        [TestMethod]
        public void Test_GetDbTableName()
        {
            Assert.AreEqual("OrderDetailX2", typeof(OrderDetailX2).GetDbTableName());

            Assert.AreEqual("OrderDetail_x3", typeof(OrderDetailX3).GetDbTableName());

            Assert.AreEqual("OrderDetailX4", typeof(OrderDetailX4).GetDbTableName());
        }

    }
}
