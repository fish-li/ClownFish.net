using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.AspnetMock;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.TestCase
{
	[TestClass]
	public class AuthorizeAttributeTest
	{
		[TestMethod]
		public void Test1()
		{
			AuthorizeAttribute a = new AuthorizeAttribute();
			a.Users = "fish, abc";

			Assert.AreEqual("fish, abc", a.Users);

			string requestText = @"
POST http://www.fish-web-demo.com/Ajax/test/DataTypeTest/Input_int_Add.aspx HTTP/1.1
";
			bool result = false;
			using( WebContext context = WebContext.FromRawText(requestText) ) {
				context.SetUserName("fish");				
				result = a.AuthenticateRequest(context.HttpContext);				
			}
			Assert.AreEqual(true, result);


			using( WebContext context = WebContext.FromRawText(requestText) ) {
				context.SetUserName("abc");				
				result = a.AuthenticateRequest(context.HttpContext);
			}
			Assert.AreEqual(true, result);
		}



		[TestMethod]
		public void Test2()
		{
			AuthorizeAttribute a = new AuthorizeAttribute();
			a.Users = "fish, abc";

			string requestText = @"
POST http://www.fish-web-demo.com/Ajax/test/DataTypeTest/Input_int_Add.aspx HTTP/1.1
";
			bool result = false;
			using( WebContext context = WebContext.FromRawText(requestText) ) {
				context.SetUserName("aaa");
				result = a.AuthenticateRequest(context.HttpContext);
			}
			Assert.AreEqual(false, result);
		}


		[TestMethod]
		public void Test3()
		{
			AuthorizeAttribute a = new AuthorizeAttribute();
			a.Roles = "2,3,4";

			Assert.AreEqual("2,3,4", a.Roles);
			
			string requestText = @"
POST http://www.fish-web-demo.com/Ajax/test/DataTypeTest/Input_int_Add.aspx HTTP/1.1
";
			bool result = false;
			using( WebContext context = WebContext.FromRawText(requestText) ) {
				context.SetUserName("fish", new string[] {"1", "2"});
				result = a.AuthenticateRequest(context.HttpContext);
			}
			Assert.AreEqual(true, result);


			using( WebContext context = WebContext.FromRawText(requestText) ) {
				context.SetUserName("fish", new string[] { "1", "3" });
				result = a.AuthenticateRequest(context.HttpContext);
			}
			Assert.AreEqual(true, result);
		}


		[TestMethod]
		public void Test4()
		{
			AuthorizeAttribute a = new AuthorizeAttribute();
			a.Roles = "2,3,4";

			string requestText = @"
POST http://www.fish-web-demo.com/Ajax/test/DataTypeTest/Input_int_Add.aspx HTTP/1.1
";
			bool result = false;
			using( WebContext context = WebContext.FromRawText(requestText) ) {
				context.SetUserName("fish", new string[] { "5", "6" });
				result = a.AuthenticateRequest(context.HttpContext);
			}
			Assert.AreEqual(false, result);


			using( WebContext context = WebContext.FromRawText(requestText) ) {
				context.SetUserName("fish");
				result = a.AuthenticateRequest(context.HttpContext);
			}
			Assert.AreEqual(false, result);
		}

	}
}
