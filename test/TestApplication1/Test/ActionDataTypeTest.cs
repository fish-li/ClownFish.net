using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Web.Client;
using ClownFish.TestApplication1.Common;

namespace ClownFish.TestApplication1.Test
{
	class ActionDataTypeTest : TestBase
	{
		[TestMethod("Action数据接收测试：Guid, ContextDataAttribute, Verb")]
		public async Task Test1()
		{
			HttpOption option = HttpOption.FromRawText(@"
POST http://www.fish-mvc-demo.com/Ajax/ns/Demo/TestGuid2.aspx HTTP/1.1

a=8679b2c7-75e5-47b7-86db-aa60addc10ab");

			// Action 要求以 PUT 方式提交，所以应该是404错误。

			int stateCode = await option.GetStatusCode();
			Assert.AreEqual(404, stateCode);


			option = HttpOption.FromRawText(@"
PUT http://www.fish-mvc-demo.com/Ajax/ns/Demo/TestGuid2.aspx HTTP/1.1

a=8679b2c7-75e5-47b7-86db-aa60addc10ab");

			string actual = await option.SendAsync<string>();
			string expected = "8679b2c7-75e5-47b7-86db-aa60addc10ab";
			Assert.AreEqual(expected, actual);



			option = HttpOption.FromRawText(@"
POST http://www.fish-mvc-demo.com/Ajax/ns/Demo/TestContextDataAttribute.aspx HTTP/1.1
User-Agent: Hello-Fish

a=8679b2c7-75e5-47b7-86db-aa60addc10ab");

			actual = await option.SendAsync<string>();
			expected = "Hello-Fish";
			Assert.AreEqual(expected, actual);

		}


		[TestMethod("上传文件测试")]
		public async Task Test2()
		{
			System.IO.FileInfo file1 = new System.IO.FileInfo("Newtonsoft.Json.dll");
			System.IO.FileInfo file2 = new System.IO.FileInfo("ClownFish.Web.dll");

			HttpOption option = new HttpOption {
				Method = "POST",
				Url = "http://www.fish-mvc-demo.com/Ajax/ns/UploadFile/Test1.aspx",
				Data = new {
					a = file1,
					b = file2,
					c = 2,
					d = 5
				}
			};


			string actual = await option.SendAsync<string>();

//{
//  "file1": {
//	"name": "E:\\ProjectFiles\\ClownFish.Web\\ClownFish.Web3\\Test\\ClownFish.TestApplication1\\bin\\Newtonsoft.Json.dll",
//	"lenght": 430080
//  },
//  "file2": {
//	"name": "E:\\ProjectFiles\\ClownFish.Web\\ClownFish.Web3\\Test\\ClownFish.TestApplication1\\bin\\ClownFish.Web.dll",
//	"lenght": 165888
//  },
//  "sum": 595975
//}
			string expected = (2 + 5 + file1.Length + file2.Length).ToString();
			Assert.IsTrue(actual.IndexOf(expected) > 0);




			HttpOption option2 = new HttpOption {
				Method = "POST",
				Url = "http://www.fish-mvc-demo.com/Ajax/ns/UploadFile/Test2.aspx",
				Data = new {
					a = file1,
					b = file2,
					c = 2,
					d = 5
				}
			};


			string actual2 = await option2.SendAsync<string>();
			Assert.AreEqual(actual2, actual);
		}


		[TestMethod("Action数据接收测试：自定义的类型转换器，int[]")]
		public async Task Test3()
		{
			HttpOption option = HttpOption.FromRawText(@"
POST http://www.fish-mvc-demo.com/Ajax/ns/TestFile/Sum.aspx HTTP/1.1
Content-Type: application/x-www-form-urlencoded

numbers=1&numbers=2&numbers=3&numbers=4&numbers=5");


			string actual = await option.SendAsync<string>();
			string expected = "15";
			Assert.AreEqual(expected, actual);
		}

	}
}
