using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.DataItems
{
	[TestClass]
	public class NameValueTest
	{
		[TestMethod]
		public void Test1()
		{
			NameValue nv = new NameValue { Name = "key1", Value = "abc" };

			Assert.AreEqual("key1=abc", nv.ToString());
        }


        [TestMethod]
        public void Test2()
        {
            NameValue nv2 = NameValue.Parse("key1=abc", '=');
            Assert.AreEqual("key1", nv2.Name);
            Assert.AreEqual("abc", nv2.Value);

            Assert.IsNull(NameValue.Parse("", '='));
            Assert.IsNull(NameValue.Parse("key1=abc", '.'));
            Assert.IsNull(NameValue.Parse("=abc", '='));
            Assert.AreEqual(string.Empty, NameValue.Parse("key1=", '=').Value);
        }       
    }


    [TestClass]
    public class NameValueExtensionsTest
    {
        [TestMethod]
        public void Test_Find()
        {
            List<NameValue> list1 = null;
            Assert.IsNull(list1.Find("aaa"));

            List<NameValue> list2 = new List<NameValue>();
            Assert.IsNull(list2.Find("aaa"));

            list2.Add(new NameValue("a1", "1111"));
            list2.Add(new NameValue("b1", "2222"));
            Assert.IsNotNull(list2.Find("b1"));
        }

        [TestMethod]
        public void Test_GetValue()
        {
            List<NameValue> list1 = null;
            Assert.IsNull(list1.GetValue("aaa"));

            List<NameValue> list2 = new List<NameValue>();
            Assert.IsNull(list2.GetValue("aaa"));

            list2.Add(new NameValue("a1", "1111"));
            list2.Add(new NameValue("b1", "2222"));
            Assert.AreEqual("2222", list2.GetValue("b1"));
        }
    }
}
