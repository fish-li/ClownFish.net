using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Base.Xml;
using ClownFish.Web.UnitTest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.TestCase
{
	[TestClass]
	public class SerializerTest
	{
		[TestMethod]
		public void TestJson()
		{
			Product p1 = new Product {
				ProductID = 3,
				CategoryID = 5,
				ProductName = "abc"
			};

			string json = JsonExtensions.ToJson(p1);

			Product p2 = JsonExtensions.FromJson<Product>(json);

			Assert.AreEqual(3, p2.ProductID);
			Assert.AreEqual(5, p2.CategoryID);
			Assert.AreEqual("abc", p2.ProductName);
		}



		[TestMethod]
		public void TestXml()
		{
			Product p1 = new Product {
				ProductID = 3,
				CategoryID = 5,
				ProductName = "abc"
			};

			string json = XmlExtensions.ToXml(p1);

			Product p2 = XmlExtensions.FromXml<Product>(json);

			Assert.AreEqual(3, p2.ProductID);
			Assert.AreEqual(5, p2.CategoryID);
			Assert.AreEqual("abc", p2.ProductName);
		}

	}
}
