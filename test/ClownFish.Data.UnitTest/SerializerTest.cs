using System;
using ClownFish.Base;
using ClownFish.Data.UnitTest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Data.UnitTest
{
	[TestClass]
	public class SerializerTest
	{
		[TestMethod]
		public void Test_代理对象序列化_二进制()
		{
			Product p = Entity.BeginEdit(new Product());
			p.ProductID = 2;
			p.CategoryID = 3;
			p.ProductName = "汉字";


			var bb = BinSerializerHelper.Serialize(p);
			var p3 = BinSerializerHelper.DeserializeObject(bb) as Product;

			Assert.AreEqual(typeof(Product), p3.GetType().BaseType);
			Assert.IsTrue(typeof(IEntityProxy).IsAssignableFrom(p3.GetType()));

			Assert.AreEqual(p.ProductID, p3.ProductID);
			Assert.AreEqual(p.CategoryID, p3.CategoryID);
			Assert.AreEqual(p.ProductName, p3.ProductName);
			Assert.AreEqual(p.Quantity, p3.Quantity);

			var bb3 = BinSerializerHelper.Serialize(p3);
			Assert.AreEqual(bb.Length, bb3.Length);
			for( int i = 0; i < bb.Length; i++ )
				Assert.AreEqual((int)bb[i], (int)bb3[i]);
		}


		[TestMethod]
		public void Test_代理对象序列化_XML()
		{
			Product p = Entity.BeginEdit(new Product());
			p.ProductID = 2;
			p.CategoryID = 3;
			p.ProductName = "汉字";


			string xml = p.ToXml();
			Console.WriteLine(xml);
			var p3 = xml.FromXml(p.GetType()) as Product;

			Assert.AreEqual(typeof(Product), p3.GetType().BaseType);
			Assert.IsTrue(typeof(IEntityProxy).IsAssignableFrom(p3.GetType()));
			

			Assert.AreEqual(p.ProductID, p3.ProductID);
			Assert.AreEqual(p.CategoryID, p3.CategoryID);
			Assert.AreEqual(p.ProductName, p3.ProductName);
			Assert.AreEqual(p.Quantity, p3.Quantity);

			bool[] bb1 = p.GetType().GetProperty("XEntityDataChangeFlags").GetValue(p) as bool[];
			bool[] bb3 = p3.GetType().GetProperty("XEntityDataChangeFlags").GetValue(p) as bool[];
			Assert.AreEqual(bb1.Length, bb3.Length);
			for( int i = 0; i < bb1.Length; i++ )
				Assert.AreEqual(bb1[i], bb3[i]);

			string xml3 = p3.ToXml();
			Assert.AreEqual(xml, xml3);
		}


		[TestMethod]
		public void Test_代理对象序列化_JSON()
		{
			Product p = Entity.BeginEdit(new Product());
			p.ProductID = 2;
			p.CategoryID = 3;
			p.ProductName = "汉字";


			string json = p.ToJson(true);
			Console.WriteLine(json);
			var p3 = json.FromJson(p.GetType()) as Product;

			Assert.AreEqual(typeof(Product), p3.GetType().BaseType);
			Assert.IsTrue(typeof(IEntityProxy).IsAssignableFrom(p3.GetType()));
			

			Assert.AreEqual(p.ProductID, p3.ProductID);
			Assert.AreEqual(p.CategoryID, p3.CategoryID);
			Assert.AreEqual(p.ProductName, p3.ProductName);
			Assert.AreEqual(p.Quantity, p3.Quantity);

			bool[] bb1 = p.GetType().GetProperty("XEntityDataChangeFlags").GetValue(p) as bool[];
			bool[] bb3 = p3.GetType().GetProperty("XEntityDataChangeFlags").GetValue(p) as bool[];
			Assert.AreEqual(bb1.Length, bb3.Length);
			for( int i = 0; i < bb1.Length; i++ )
				Assert.AreEqual(bb1[i], bb3[i]);


			string json3 = p3.ToJson(true);
			Assert.AreEqual(json, json3);
		}
	}
}
