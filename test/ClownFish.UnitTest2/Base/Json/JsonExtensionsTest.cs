using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Base;

namespace ClownFish.UnitTest.Base.Json
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

			Assert.IsTrue(p.IsEqual(p2));
		}


		public class TestData
        {
			public object Data { get; set; }
        }

        [TestMethod]
        public void Test_ToJsonKeepTypeInfo()
        {
            Product p = Product.CreateByFixedData();
			TestData data = new TestData { Data = p };

            string json = data.ToJson(JsonStyle.KeepType);

            Assert.IsTrue(json.Contains("{\"$type\":\"ClownFish.UnitTest.Base.Product, ClownFish.UnitTest\""));
        }

        [TestMethod]
		public void Test_FromJson_ObjectType()
		{
			Product p = Product.CreateByRandomData();

			string json = p.ToJson();
			Product p2 = json.FromJson(typeof(Product)) as Product;

			Assert.IsTrue(p.IsEqual(p2));
		}

        [TestMethod]
        public void Test_ToMultiLineJson()
        {
            List<Product> list = new List<Product>();
            list.Add(Product.CreateByFixedData());
            list.Add(Product.CreateByFixedData());
            list.Add(Product.CreateByFixedData());

            string lines = list.ToMultiLineJson().Trim();
            Assert.AreEqual(2, lines.Where(x => x == '\n').Count());
            Assert.IsTrue(lines.StartsWith("{"));
            Assert.IsTrue(lines.EndsWith("}"));


            List<Product> list2 = lines.FromMultiLineJson<Product>();
            Assert.AreEqual(3, list2.Count);

            MyAssert.AreEqual(list, list2);
        }


        [TestMethod]
		public void Test_FromMultiLineJson()
        {
			StringBuilder sb = new StringBuilder();
			sb.Append(Product.CreateByFixedData().ToJson()).Append("\n");
			sb.Append(Product.CreateByRandomData().ToJson()).Append("\n");
			sb.Append(Product.CreateByRandomData().ToJson()).Append("\n");

			List<Product> list = sb.ToString().FromMultiLineJson<Product>();
			Assert.AreEqual(3, list.Count);


			List<Product> list2 = JsonExtensions.FromMultiLineJson<Product>(string.Empty);
			Assert.AreEqual(0, list2.Count);

			List<Product> list3 = JsonExtensions.FromMultiLineJson<Product>(null);
			Assert.IsNull(list3);
		}

		[TestMethod]
		public void Test_ToJsonSerializerSettings()
        {
			JsonSerializerSettings jss = JsonStyle.Indented.ToJsonSerializerSettings();
			Assert.AreEqual(Formatting.Indented, jss.Formatting);

		}
	}
}
