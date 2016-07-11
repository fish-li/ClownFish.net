using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.Common
{
	[TestClass]
	public class NameValueTest
	{
		[TestMethod]
		public void Test()
		{
			NameValue nv = new NameValue { Name = "key1", Value = "abc" };

			Assert.AreEqual("key1=abc", nv.ToString());
		}
	}
}
