using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.AspnetMock;
using ClownFish.Web.UnitTest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.Action
{

	[TestClass]
	public class JsonResultTest
	{
		[TestMethod]
		public void Test()
		{
			string requestText = @"
GET http://www.fish-mvc-demo.com/Ajax/test/DataTypeTest/Input_string_ToUpper.aspx?input=fish HTTP/1.1
";
			using( WebContext context = WebContext.FromRawText(requestText) ) {
				TM tm = new TM { intVal = 2, StrVal = "abc" };

				IActionResult result = new JsonResult(tm);

				result.Ouput(context.HttpContext);

				string json = JsonExtensions.ToJson(tm);

				string responseText = context.Response.GetText();
				Assert.AreEqual(json, responseText);
				Assert.AreEqual("application/json", context.HttpContext.Response.ContentType);
			}
		}


		[TestMethod]
		public void Test_Jonsp()
		{
			string requestText = @"
GET http://www.fish-mvc-demo.com/Ajax/test/DataTypeTest/Input_string_ToUpper.aspx?input=fish&callback=testjsonp HTTP/1.1
Referer: http://www.fish-test-demo.com/xxx.html
";
			using( WebContext context = WebContext.FromRawText(requestText) ) {
				TM tm = new TM { intVal = 2, StrVal = "abc" };

				IActionResult result = new JsonResult(tm);

				result.Ouput(context.HttpContext);

				string json = "testjsonp({\"intVal\":2,\"StrVal\":\"abc\"});";

				string responseText = context.Response.GetText();
				Assert.AreEqual(json, responseText);
				Assert.AreEqual("text/javascript", context.HttpContext.Response.ContentType);
			}
		}




		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void Test_ArgumentNullException()
		{
			JsonResult result = new JsonResult(null);
		}


	}
}
