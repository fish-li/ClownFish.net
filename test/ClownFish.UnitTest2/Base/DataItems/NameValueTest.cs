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
}
