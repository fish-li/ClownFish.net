using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Http;
using ClownFish.Base.WebClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Common
{
	[TestClass]
	public class HttpHeaderCollectionTest
	{
		[TestMethod]
		public void Test_HttpHeaderCollection_Add_Remove()
		{
			HttpHeaderCollection collection = new HttpHeaderCollection();

			collection.Add("k1", "abc");
			Assert.IsTrue(collection.ContainsName("k1"));
			Assert.AreEqual("abc", collection["k1"]);

			collection.Add("k1", "abc2");
			int count = collection.Remove("k1");
			Assert.AreEqual(2, count);
			Assert.IsFalse(collection.ContainsName("k1"));
			Assert.IsNull(collection["k1"]);


			collection["k2"] = "2222";
			Assert.IsTrue(collection.ContainsName("k2"));
			Assert.AreEqual("2222", collection["k2"]);
		}

		[TestMethod]
		public void Test_HttpHeaderCollection_ArgumentException()
		{
			HttpHeaderCollection collection = new HttpHeaderCollection();

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
		}
	}
}
