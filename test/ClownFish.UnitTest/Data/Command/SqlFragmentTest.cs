using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Data.Command
{
    [TestClass]
    public class SqlFragmentTest
    {
        [TestMethod]
        public void Test1()
        {
            SqlFragment s1 = new SqlFragment("xxx");
            Assert.AreEqual("xxx", s1.Value);

            MyAssert.IsError<ArgumentNullException>(() => {
                SqlFragment s2 = new SqlFragment("");
            });
        }
    }
}
