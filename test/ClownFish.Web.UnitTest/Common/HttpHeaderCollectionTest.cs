using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.Common
{
	[TestClass]
	public class HttpHeaderCollectionTest
	{
		[TestMethod]
		public void Test1()
		{
			HttpHeaderCollection collection = new HttpHeaderCollection();

			collection.Add("k1", "abc");
			Assert.AreEqual("abc", collection["k1"]);

			int count = 0;
			try {
				collection.Add(null, "abc");
			}
			catch( ArgumentException ) {
				count++;
			}

			try {
				collection.Add("k2", null);
			}
			catch( ArgumentException ) {
				count++;
			}

			try {
				collection.Remove("");
			}
			catch( ArgumentException ) {
				count++;
			}

			Assert.AreEqual(3, count);


			collection.Remove("k1");
			Assert.IsNull(collection["k1"]);

		}
	}
}
