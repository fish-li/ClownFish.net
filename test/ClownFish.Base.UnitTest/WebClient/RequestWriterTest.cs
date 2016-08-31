using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Http;
using ClownFish.Base.WebClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Base.UnitTest.WebClient
{
	[TestClass]
	public class RequestWriterTest
	{
		private MethodInfo _writeMethod = typeof(RequestWriter).GetMethod("Write", BindingFlags.Instance | BindingFlags.NonPublic, null, 
			new Type[] { typeof(Stream), typeof(object), typeof(SerializeFormat) }, 
			null);


		[TestMethod]
		public void Test_RequestWriter_Write_Text()
		{
			var data = new { a = 1, b = 2, c = "xyz中文汉字" };
			Tuple<string, string> actual = WriteStream(data, SerializeFormat.Text);

			Assert.AreEqual("text/plain", actual.Item1);
			Assert.AreEqual("{ a = 1, b = 2, c = xyz中文汉字 }", actual.Item2);
		}

		[TestMethod]
		public void Test_RequestWriter_Write_Json()
		{
			var data = new { a = 1, b = 2, c = "xyz中文汉字" };
			Tuple<string, string> actual = WriteStream(data, SerializeFormat.Json);

			Assert.AreEqual("application/json", actual.Item1);
			Assert.AreEqual("{'a':1,'b':2,'c':'xyz中文汉字'}", actual.Item2.Replace('\"', '\''));
		}

		[TestMethod]
		public void Test_RequestWriter_Write_Xml()
		{
			Product p = new Product { ProductID = 2, ProductName = "abc" };
			Tuple<string, string> actual = WriteStream(p, SerializeFormat.Xml);

			Assert.AreEqual("application/xml", actual.Item1);
			Assert.AreEqual(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<Product xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
    <ProductID>2</ProductID>
    <ProductName>abc</ProductName>
    <CategoryID>0</CategoryID>
    <UnitPrice>0</UnitPrice>
    <Quantity>0</Quantity>
</Product>
".Trim(), actual.Item2);
		}

		[TestMethod]
		public void Test_RequestWriter_Write_Form()
		{
			var data = new { a = 1, b = 2, c = "xyz中文汉字" };
			Tuple<string, string> actual = WriteStream(data, SerializeFormat.Form);

			Assert.AreEqual("application/x-www-form-urlencoded", actual.Item1);
			Assert.AreEqual("a=1&b=2&c=xyz%e4%b8%ad%e6%96%87%e6%b1%89%e5%ad%97", actual.Item2);
		}


		private Tuple<string, string> WriteStream(object data, SerializeFormat format)
		{
			HttpWebRequest request = WebRequest.CreateHttp("http://www.bing.com");
			RequestWriter writer = new RequestWriter(request);

			using(MemoryStream ms = new MemoryStream() ) {
				_writeMethod.Invoke(writer, new object[] { ms, data, format });

				ms.Position = 0;

				return new Tuple<string, string>(
					request.ContentType,
					Encoding.UTF8.GetString(ms.ToArray())
					);
			}
		}
	}
}
