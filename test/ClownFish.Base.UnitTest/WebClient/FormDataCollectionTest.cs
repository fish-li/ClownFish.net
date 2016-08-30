using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.WebClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Base.UnitTest.WebClient
{
	[TestClass]
	public class FormDataCollectionTest
	{
		[TestMethod]
		public void Test_FormDataCollection_object_ToString()
		{
			var data = new { a = 1, b = 2, c = "xyz" };

			FormDataCollection form = FormDataCollection.Create(data);
			string actual = form.ToString();
			Assert.AreEqual("a=1&b=2&c=xyz", actual);
		}


		[TestMethod]
		public void Test_FormDataCollection_IDictionary_ToString()
		{
			Hashtable table = new Hashtable();
			table["a"] = 1;
			table["b"] = 2;
			table["c"] = "xyz";

			FormDataCollection form = FormDataCollection.Create(table);
			string actual = form.ToString();
			Assert.AreEqual("a=1&b=2&c=xyz", actual);
		}

		[TestMethod]
		public void Test_FormDataCollection_NameValueCollection_ToString()
		{
			NameValueCollection collection = new NameValueCollection();
			collection["a"] = "1";
			collection["b"] = "2";

			collection["c"] = "xyz";
			collection.Add("c", "789");


			FormDataCollection form = FormDataCollection.Create(collection);
			string actual = form.ToString();
			Console.WriteLine(actual);
			Assert.AreEqual("a=1&b=2&c=xyz&c=789", actual);
		}

	}
}
