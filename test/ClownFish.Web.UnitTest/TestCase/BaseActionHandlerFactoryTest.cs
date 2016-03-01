using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ClownFish.MockAspnetRuntime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.TestCase
{
	[TestClass]
	public class BaseActionHandlerFactoryTest  : BaseTest
	{
		[TestMethod]
		public void Test1()
		{
			string requestText = @"
POST http://www.fish-mvc-demo.com/Ajax/test/DataTypeTest/Input_int_Add.aspx HTTP/1.1
";
			IHttpHandler handler = null;

			using( WebContext context = WebContext.FromRawText(requestText) ) {
				var factory = new ServiceHandlerFactory();
				handler = factory.GetHandler(context.HttpContext, "POST", null, null);

				// 下面这个调用没什么意义，只是为了覆盖代码
				factory.ReleaseHandler(handler);
			}
			Assert.IsNotNull(handler);
		}


		[ExpectedException(typeof(HttpException))]
		[TestMethod]
		public void Test2()
		{
			string requestText = @"
POST http://www.fish-mvc-demo.com/Ajax/test/DataTypeTest/xxxxxxxxxxx.aspx HTTP/1.1
";
			// 上面URL指定了一个不存在的Action名称，会引发404错误

			IHttpHandler handler = null;

			using( WebContext context = WebContext.FromRawText(requestText) ) {
				var factory = new ServiceHandlerFactory();
				handler = factory.GetHandler(context.HttpContext, "POST", null, null);				
			}
		}


		[ExpectedException(typeof(HttpException))]
		[TestMethod]
		public void Test3()
		{
			string requestText = @"
POST http://www.fish-mvc-demo.com/DataTypeTest/xxxxxxxxxxx HTTP/1.1
";
			// 上面URL指定了一个ServiceHandlerFactory不能接受的格式，会引发404错误

			IHttpHandler handler = null;

			using( WebContext context = WebContext.FromRawText(requestText) ) {
				var factory = new ServiceHandlerFactory();
				handler = factory.GetHandler(context.HttpContext, "POST", null, null);
			}
		}
	}
}
