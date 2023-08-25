using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Base;


namespace ClownFish.UnitTest.Base.Common
{
	[TestClass]
	public class HttpExtensionsTest
	{
		[TestMethod]
		public void Test_WebHeaderCollection_AddWithoutValidate()
		{
			WebHeaderCollection collection = new WebHeaderCollection();

			collection.InternalAdd("a1", "111");
			Assert.AreEqual("111", collection["a1"]);
		}
	}
}
