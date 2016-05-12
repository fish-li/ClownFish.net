using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ClownFish.MockAspnetRuntime;
using ClownFish.Web.Debug404;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.TestCase
{
	[TestClass]
	public class MvcPageHandlerFactoryTest : BaseTest
	{
		[TestMethod]
		public void Test1()
		{
			string requestText = @"
GET http://www.fish-mvc-demo.com/Pages/Demo/Authorize/Everyone.aspx HTTP/1.1
";
			IHttpHandler handler = null;

			using( WebContext context = WebContext.FromRawText(requestText) ) {
				IHttpHandlerFactory factory = new MvcPageHandlerFactory();
				handler = factory.GetHandler(context.HttpContext, "GET", null, null);

				// 下面这个调用没什么意义，只是为了覆盖代码
				factory.ReleaseHandler(handler);
			}
			Assert.IsNotNull(handler);

			object vkInfo = handler.GetValue("InvokeInfo");
			object instance = vkInfo.GetValue("Instance");

			Assert.AreEqual(typeof(Controllers.AuthorizeTestController), instance.GetType());
		}


		[ExpectedException(typeof(HttpException))]
		[TestMethod]
		public void Test2()
		{
			string requestText = @"
GET http://www.fish-mvc-demo.com/Pages/abc.zzz HTTP/1.1
";
			IHttpHandler handler = null;

			using( WebContext context = WebContext.FromRawText(requestText) ) {
				IHttpHandlerFactory factory = new MvcPageHandlerFactory();
				handler = factory.GetHandler(context.HttpContext, "GET", "/Pages/abc.zzz",
							@"D:\ProjectFiles\my-github\ClownFish.net\demo\web\MvcDemoWebSite1\Pages\abc.zzz");
			}
			//Assert.IsNull(handler);
		}


		
	}
}
