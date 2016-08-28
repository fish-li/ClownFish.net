using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.AspnetMock;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.Action
{
	[TestClass]
	public class Http404ResultTest
	{
		[TestMethod]
		public void Test()
		{
			string requestText = @"
GET http://www.fish-mvc-demo.com/Ajax/test/DataTypeTest/Input_string_ToUpper.aspx?input=fish HTTP/1.1
";
			using( WebContext context = WebContext.FromRawText(requestText) ) {
				Http404Result result = new Http404Result();

				result.Ouput(context.HttpContext);

				Assert.AreEqual(404, context.HttpContext.Response.StatusCode);
			}
		}
	}
}
