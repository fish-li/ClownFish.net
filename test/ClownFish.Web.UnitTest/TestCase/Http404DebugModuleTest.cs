using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ClownFish.AspnetMock;
using ClownFish.Web.Debug404;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.TestCase
{
	[TestClass]
	public class Http404DebugModuleTest : BaseTest
	{
		[TestMethod]
		public void Test1()
		{
			string requestText = @"
POST http://www.fish-mvc-demo.com/DataTypeTest/xxxxxxxxxxx HTTP/1.1
";
			// 上面URL指定了一个ServiceHandlerFactory不能接受的格式，会引发404错误

			IHttpHandler handler = null;			
			Http404DebugModule debugModule = new Http404DebugModule();

			using( WebContext context = WebContext.FromRawText(requestText) ) {
				debugModule.app_BeginRequest(context.Application.Instance, null);

				var factory = new ServiceHandlerFactory();
				handler = factory.GetHandler(context.HttpContext, "POST", null, null);
			}

			Assert.IsInstanceOfType(handler, typeof(Http404PageHandler));
		}


		[TestMethod]
		public void Test2()
		{
			string requestText = @"
POST http://www.fish-mvc-demo.com/Ajax/test/DataTypeTest/xxxxxxxxxxx.aspx HTTP/1.1
";
			// 上面URL指定了一个不存在的Action名称，会引发404错误

			IHttpHandler handler = null;
			Http404DebugModule debugModule = new Http404DebugModule();

			using( WebContext context = WebContext.FromRawText(requestText) ) {
				debugModule.app_BeginRequest(context.Application.Instance, null);

				var factory = new ServiceHandlerFactory();
				handler = factory.GetHandler(context.HttpContext, "POST", null, null);
			}

			Assert.IsInstanceOfType(handler, typeof(Http404PageHandler));
		}


	}
}
