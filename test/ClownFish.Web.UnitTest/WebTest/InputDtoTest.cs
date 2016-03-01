using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Base.Xml;
using ClownFish.Web.UnitTest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.WebTest
{
	[TestClass]
	public class InputDtoTest : BaseTest
	{
		private static readonly string s_ProductFormData = "productID=2&productName=name123&CategoryID=5&Unit=ge&UnitPrice=12.5&Quantity=53&Remark=mark369";
		private static readonly string s_ProductJson = "{'ProductID':2,'ProductName':'name123','CategoryID':5,'Unit':'ge','UnitPrice':12.5,'Quantity':53,'Remark':'mark369'}".Replace('\'', '\"');
		private static readonly string s_ProductXml = @"
<Product>
    <ProductID>2</ProductID>
    <ProductName>name123</ProductName>
    <CategoryID>5</CategoryID>
    <Unit>ge</Unit>
    <UnitPrice>12.5</UnitPrice>
    <Quantity>53</Quantity>
    <Remark>mark369</Remark>
</Product>
".Replace('\'', '\"').Trim();


		[TestMethod]
		public void Input_Product()
		{
			string requestText = @"
POST http://www.fish-mvc-demo.com/Ajax/test/ComplexDataType/Input_Product.aspx HTTP/1.1
X-Result-Format: JSON

" + s_ProductFormData;

			// 测试：按FROM的键值对方式输入一个对象，要求在服务端反序列化成对象，并以JSON形式返回结果

			string result = ExecuteService(requestText);
			Product p = JsonExtensions.FromJson<Product>(result);

			AssertProductFor_Input_Product(p);
		}


		private void AssertProductFor_Input_Product(Product p)
		{
			Assert.AreEqual(3, p.ProductID);
			Assert.AreEqual("name123_test", p.ProductName);
			Assert.AreEqual(6, p.CategoryID);
			Assert.AreEqual("ge", p.Unit);
			Assert.AreEqual(12.5m, p.UnitPrice);
			Assert.AreEqual(54, p.Quantity);
			Assert.AreEqual("mark369", p.Remark);
		}

		[TestMethod]
		public void Input_Product2()
		{
			string requestText = @"
POST http://www.fish-mvc-demo.com/Ajax/test/ComplexDataType/Input_Product.aspx HTTP/1.1
X-Result-Format: XML

" + s_ProductFormData;

			// 测试：按FROM的键值对方式输入一个对象，要求在服务端反序列化成对象，并以XML形式返回结果

			string result = ExecuteService(requestText);
			Product p = XmlExtensions.FromXml<Product>(result);

			AssertProductFor_Input_Product(p);
		}


		[TestMethod]
		public void Input_Product3()
		{
			string requestText = @"
POST http://www.fish-mvc-demo.com/Ajax/test/ComplexDataType/Input_Product.aspx HTTP/1.1
X-Result-Format: TEXT

" + s_ProductFormData;

			// 测试：按FROM的键值对方式输入一个对象，要求在服务端反序列化成对象，并以TEXT形式返回结果（调用对象的 ToString 方法）

			string result = ExecuteService(requestText);
			string expected = string.Format("id={0};name={1}", 3, "name123_test");

			Assert.AreEqual(result, expected);
		}



		[TestMethod]
		public void Input_Product4()
		{
			string requestText = @"
POST http://www.fish-mvc-demo.com/Ajax/test/ComplexDataType/Input_Product.aspx HTTP/1.1
X-Result-Format: JSON
Content-Type: application/json

" + s_ProductJson;

			// 测试：按JSON字符串方式输入一个对象，要求在服务端反序列化成对象，并以JSON形式返回结果

			string result = ExecuteService(requestText);
			Product p = JsonExtensions.FromJson<Product>(result);

			AssertProductFor_Input_Product(p);
		}


		[TestMethod]
		public void Input_Product5()
		{
			string requestText = @"
POST http://www.fish-mvc-demo.com/Ajax/test/ComplexDataType/Input_Product.aspx HTTP/1.1
X-Result-Format: JSON
Content-Type: application/xml

" + s_ProductXml;

			// 测试：按XML字符串方式输入一个对象，要求在服务端反序列化成对象，并以JSON形式返回结果

			string result = ExecuteService(requestText);
			Product p = JsonExtensions.FromJson<Product>(result);

			AssertProductFor_Input_Product(p);
		}



		[TestMethod]
		public void Input_2_Product()
		{
			string productFormData1 = "p1.productID=12&p1.productName=name123&p1.CategoryID=15&p1.Unit=ge&p1.UnitPrice=12.5&p1.Quantity=53&p1.Remark=mark369";
			string productFormData2 = "p2.productID=22&p2.productName=kkkp123&p2.CategoryID=25&p2.Unit=xx&p2.UnitPrice=32.5&p2.Quantity=13&p2.Remark=zzzz111";

			string requestText = @"
POST http://www.fish-mvc-demo.com/Ajax/test/ComplexDataType/Input_2_Product.aspx HTTP/1.1

" + productFormData1 + "&" + productFormData2;

			// 测试：按FROM的键值对方式输入 二个 对象

			string result = ExecuteService(requestText);
			Product p = JsonExtensions.FromJson<Product>(result);

			Assert.AreEqual(34, p.ProductID);
			Assert.AreEqual("name123kkkp123", p.ProductName);
			Assert.AreEqual(40, p.CategoryID);
			Assert.AreEqual("gexx", p.Unit);
			Assert.AreEqual(45m, p.UnitPrice);
			Assert.AreEqual(66, p.Quantity);
			Assert.AreEqual("mark369zzzz111", p.Remark);
		}


		[TestMethod]
		public void Input_int_Product()
		{
			string requestText = @"
POST http://www.fish-mvc-demo.com/Ajax/test/ComplexDataType/Input_int_Product.aspx HTTP/1.1

" + s_ProductFormData + "&a=10";

			// 测试：按FROM的键值对方式输入一个对象和一个整数参数

			string result = ExecuteService(requestText);
			Product p = JsonExtensions.FromJson<Product>(result);

			p.Quantity -= 9;
			AssertProductFor_Input_Product(p);
		}


		[TestMethod]
		public void Input_JSON()
		{
			string requestText = @"
POST http://www.fish-mvc-demo.com/Ajax/test/ComplexDataType/Input_JSON.aspx HTTP/1.1
Content-Type: application/json

" + s_ProductJson;

			// 测试：按FROM的键值对方式输入一个对象和一个整数参数

			string result = ExecuteService(requestText);
			Assert.AreEqual("2", result);
		}


		[TestMethod]
		public void Input_XML()
		{
			string requestText = @"
POST http://www.fish-mvc-demo.com/Ajax/test/ComplexDataType/Input_XML.aspx HTTP/1.1
Content-Type: application/xml

" + s_ProductXml;

			// 测试：按FROM的键值对方式输入一个对象和一个整数参数

			string result = ExecuteService(requestText);
			Assert.AreEqual("2", result);
		}


		[TestMethod]
		public void Input_XmlDocument()
		{
			string requestText = @"
POST http://www.fish-mvc-demo.com/Ajax/test/ComplexDataType/Input_XmlDocument.aspx HTTP/1.1
Content-Type: application/xml

" + s_ProductXml;

			// 测试：按FROM的键值对方式输入一个对象和一个整数参数

			string result = ExecuteService(requestText);
			Assert.AreEqual("2", result);
		}

	}
}
