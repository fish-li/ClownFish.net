using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.AspnetMock;
using ClownFish.Web.UnitTest.Ext;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.TestCase
{
	[TestClass]
	public class BaseControllerTest : BaseTest
	{
		[TestMethod]
		public void Test1()
		{
			string requestText = @"
POST http://www.fish-mvc-demo.com/Ajax/test/Some/TestCookieAndHeader.aspx HTTP/1.1
Cookie: cookie1=fish
header1: ClownFish.net
";
			// 上面URL指定了一个ServiceHandlerFactory不能接受的格式，会引发404错误

			Hashtable table = (WebRuntimeExt.Instance as WebRuntimeExt).CallMessage;
			table.Clear();

			using( WebContext context = WebContext.FromRawText(requestText) ) {
				ExecuteService(context);

				Assert.AreEqual("hello_fish", context.HttpContext.Response.Cookies["cookie2"].Value);
			}

			Assert.AreEqual("hello_ClownFish.net", table["header2"]);
		}


	}
}
