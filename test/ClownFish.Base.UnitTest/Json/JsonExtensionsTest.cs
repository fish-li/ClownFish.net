using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Base;

namespace ClownFish.Base.UnitTest.Json
{
	[TestClass]
	public class JsonExtensionsTest
	{
		[TestMethod]
		public void Test_ToJson_FromJson()
		{
			Product p = Product.CreateByRandomData();

			string json = p.ToJson();
			Product p2 = json.FromJson<Product>();

			Assert.IsTrue(p.IsEquals(p2));
		}


		[TestMethod]
		public void Test_ToJsonKeepTypeInfo()
		{
			Product p = Product.CreateByFixedData();

			string json = p.ToJson(true);
			Product p2 = json.FromJson<Product>();

			Assert.IsTrue(p.IsEquals(p2));
			Assert.IsTrue(json.StartsWith("{\"$type\":\"ClownFish.Base.UnitTest.Product, ClownFish.Base.UnitTest\""));
		}

		[TestMethod]
		public void Test_FromJson_ObjectType()
		{
			Product p = Product.CreateByRandomData();

			string json = p.ToJson();
			Product p2 = json.FromJson(typeof(Product)) as Product;

			Assert.IsTrue(p.IsEquals(p2));
		}
	}
}
