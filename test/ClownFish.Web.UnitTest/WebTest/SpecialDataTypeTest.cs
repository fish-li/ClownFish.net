using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.AspnetMock;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.WebTest
{
	[TestClass]
	public class SpecialDataTypeTest : BaseTest
	{
		[TestMethod]
		public void Input_string_ToUpper()
		{
			string requestText = @"
GET http://www.fish-mvc-demo.com/Ajax/test/SpecialDataType/Input_HttpContext.aspx HTTP/1.1
";

			using( WebContext context = WebContext.FromRawText(requestText) ) {
				context.HttpContext.Items["unit-test"] = "ClownFish.Web";


				string result = ExecuteService(context);
				Assert.AreEqual("ClownFish.Web", result);
			}
		}


		[TestMethod]
		public void Input_querystring()
		{
			string requestText = @"
GET http://www.fish-mvc-demo.com/Ajax/test/SpecialDataType/Input_querystring.aspx?a=fish HTTP/1.1
";
			string result = ExecuteService(requestText);
			Assert.AreEqual("fish", result);
		}

		[TestMethod]
		public void Input_form()
		{
			string requestText = @"
POST http://www.fish-mvc-demo.com/Ajax/test/SpecialDataType/Input_form.aspx HTTP/1.1

a=fish
";
			string result = ExecuteService(requestText);
			Assert.AreEqual("fish", result);
		}



		[TestMethod]
		public void Input_headers()
		{
			string requestText = @"
GET http://www.fish-mvc-demo.com/Ajax/test/SpecialDataType/Input_headers.aspx HTTP/1.1
a: fish
";
			string result = ExecuteService(requestText);
			Assert.AreEqual("fish", result);
		}








	}
}
