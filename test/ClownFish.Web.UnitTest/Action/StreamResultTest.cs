using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.MockAspnetRuntime;

namespace ClownFish.Web.UnitTest.Action
{
	[TestClass]
	public class StreamResultTest
	{
		[TestMethod]
		public void Test1()
		{
			// 测试 StreamResult的构造函数传递，及contentType的默认设置

			byte[] buffer = System.Text.Encoding.Unicode.GetBytes("中文汉字");
			StreamResult streamResult = new StreamResult(buffer);

			string actual = streamResult.GetValue("_contentType").ToString();
			Assert.AreEqual("application/octet-stream", actual);
		}


		[TestMethod]
		public void Test2()
		{
			string requestText = @"
GET http://www.fish-mvc-demo.com/Ajax/test/DataTypeTest/Input_string_ToUpper.aspx?input=fish HTTP/1.1
User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/44.0.2403.155 Safari/537.36 OPR/31.0.1889.174
";
			using( WebContext context = WebContext.FromRawText(requestText) ) {
				context.Response.EnableOutputStream();

				byte[] buffer = System.Text.Encoding.Unicode.GetBytes("中文汉字");

				// 注意：这个文件名中，有些字符是操作系统不允许的，它们将会变成_
				string filename = "中文 汉字,无乱码~`!@#$%^&*()_-+-=[]{}|:;',.<>?¥◆≠∞µαβπ™■.dat";
				StreamResult streamResult = new StreamResult(buffer, "text/test", filename);

				streamResult.Ouput(context.HttpContext);

				// 获取编码后的文件名标头
				string header = context.Response.GetCustomHeader("Content-Disposition");

				// TODO: 现在拿到编码后的文件名了，但是不知道该如何断言，暂时先不做判断。

				Assert.AreEqual("text/test", context.HttpContext.Response.ContentType);
			}
		}

	}
}
