using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ClownFish.MockAspnetRuntime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.WebTest
{
	[TestClass]
	public class AuthorizeTest : BaseTest
	{
		[TestMethod]
		public void Test1()
		{
			// 访问一个没有权限检查的地址
			string requestText = @"
GET http://www.fish-mvc-demo.com/Pages/Demo/Authorize/Everyone.aspx HTTP/1.1
";
			string responseText = null;

			using( WebContext context = WebContext.FromRawText(requestText) ) {
				responseText = ExecutePage(context);
			}

			Assert.AreEqual(responseText, "Hello ClownFish.net");
		}



		[TestMethod]
		[ExpectedException(typeof(HttpException))]
		public void Test2()
		{
			// 访问一个必须是【已登录】用户才能访问的地址
			string requestText = @"
GET http://www.fish-mvc-demo.com/Pages/Demo/Authorize/LoginUser.aspx HTTP/1.1
";
			string responseText = null;
			using( WebContext context = WebContext.FromRawText(requestText) ) {
				// 没有设置登录用户，下面的调用将会引发异常
				responseText = ExecutePage(context);
			}
		}

		[TestMethod]
		public void Test3()
		{
			// 访问一个必须是【已登录】用户才能访问的地址
			string requestText = @"
GET http://www.fish-mvc-demo.com/Pages/Demo/Authorize/LoginUser.aspx HTTP/1.1
";
			string responseText = null;

			using( WebContext context = WebContext.FromRawText(requestText) ) {
				context.SetUserName("tester");	// 模拟 tester 用户登录
				responseText = ExecutePage(context);
			}

			Assert.AreEqual(responseText, "Hello ClownFish.net");
		}


		[TestMethod]
		[ExpectedException(typeof(HttpException))]
		public void Test4()
		{
			// 访问一个必须是【fish】用户才能访问的地址
			string requestText = @"
GET http://www.fish-mvc-demo.com/Pages/Demo/Authorize/Fish.aspx HTTP/1.1
";
			string responseText = null;

			try {
				using( WebContext context = WebContext.FromRawText(requestText) ) {
					// 没有设置登录用户，下面的调用将会引发异常
					responseText = ExecutePage(context);
				}
			}
			catch( HttpException ) {
				using( WebContext context = WebContext.FromRawText(requestText) ) {
					context.SetUserName("tester");	// 模拟 tester 用户登录，这里仍然会抛出异常，因为只有 fish 这个用户才能访问
					responseText = ExecutePage(context);
				}
			}
		}

		[TestMethod]
		public void Test5()
		{
			// 访问一个必须是【fish】用户才能访问的地址
			string requestText = @"
GET http://www.fish-mvc-demo.com/Pages/Demo/Authorize/Fish.aspx HTTP/1.1
";
			string responseText = null;

			using( WebContext context = WebContext.FromRawText(requestText) ) {
				context.SetUserName("fish");	// 模拟 fish 用户登录
				responseText = ExecutePage(context);
			}

			Assert.AreEqual(responseText, "Hello Fish Li");
		}


		[TestMethod]
		[ExpectedException(typeof(HttpException))]
		public void Test6()
		{
			// 访问一个必须是【权限号=23】的用户才能访问的地址
			string requestText = @"
GET http://www.fish-mvc-demo.com/Pages/Demo/Authorize/RightNo23.aspx HTTP/1.1
";
			string responseText = null;

			using( WebContext context = WebContext.FromRawText(requestText) ) {
				context.SetUserName("fish");	// 模拟 fish 用户登录，还是会有异常，因为没有 23 这个权限
				responseText = ExecutePage(context);
			}

		}



		[TestMethod]
		public void Test7()
		{
			// 访问一个必须是【权限号=23】的用户才能访问的地址
			string requestText = @"
GET http://www.fish-mvc-demo.com/Pages/Demo/Authorize/RightNo23.aspx HTTP/1.1
Cookie: rightNo_demo=23
";
			// 注意：上面的请求头中已经指定的权限Cookie

			string responseText = null;

			using( WebContext context = WebContext.FromRawText(requestText) ) {
				context.SetUserName("fish");	// 模拟 fish 用户登录，并且指定了Cookie（按CheckRightAttribute的要求）
				responseText = ExecutePage(context);
			}

		}


		[ExpectedException(typeof(HttpException))]
		[TestMethod]
		public void Test8()
		{
			// 访问一个必须是【权限号=23】的用户才能访问的地址
			string requestText = @"
GET http://www.fish-mvc-demo.com/Pages/Demo/Authorize/RightNo23.aspx HTTP/1.1
Cookie: rightNo_demo=22
";
			// 注意：上面的请求头中已经指定的权限Cookie，但是权限号不对

			string responseText = null;

			using( WebContext context = WebContext.FromRawText(requestText) ) {
				context.SetUserName("fish");	// 模拟 fish 用户登录，并且指定了Cookie（按CheckRightAttribute的要求）
				responseText = ExecutePage(context);
			}

		}


	}
}
