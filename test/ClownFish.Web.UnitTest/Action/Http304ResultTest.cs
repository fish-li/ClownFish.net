using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.MockAspnetRuntime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.Action
{
	[TestClass]
	public class Http304ResultTest
	{
		[TestMethod]
		public void Test()
		{
			string requestText = @"
GET http://www.fish-mvc-demo.com/Ajax/test/DataTypeTest/Input_string_ToUpper.aspx?input=fish HTTP/1.1
";
			using( WebContext context = WebContext.FromRawText(requestText) ) {
				Http304Result result = new Http304Result();

				result.Ouput(context.HttpContext);

				Assert.AreEqual(304, context.HttpContext.Response.StatusCode);
			}
		}
	}
}
