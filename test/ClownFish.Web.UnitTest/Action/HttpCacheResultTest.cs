using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ClownFish.AspnetMock;
using ClownFish.Web.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.Action
{
	[TestClass]
	public class HttpCacheResultTest : BaseTest
	{
		[TestMethod]
		public void Test()
		{
			// 正常场景测试

			string requestText = @"
GET http://www.fish-mvc-demo.com/Ajax/test/DataTypeTest/Input_string_ToUpper.aspx?input=fish HTTP/1.1
";
			using( WebContext context = WebContext.FromRawText(requestText) ) {

				TextResult text = new TextResult("abc");

				DateTime lastModified = new DateTime(2015, 5, 3);
				HttpCacheResult result = new HttpCacheResult(text, 100, lastModified);
				result.ETag = "123456789";


				ServiceHandlerFactory factory = new ServiceHandlerFactory();
				IHttpHandler handler = factory.GetHandler(context.HttpContext, null, null, null);

				context.HttpContext.Handler = handler;

				ActionExecutor executor = new ActionExecutor();
				executor.SetValue("HttpContext", context.HttpContext);

				handler.SetValue("ActionExecutor", executor);


				result.Ouput(context.HttpContext);

				Assert.AreEqual("abc", context.Response.GetText());

				var responseCache = context.HttpContext.Response.Cache;
				Assert.AreEqual("Public", responseCache.GetValue("_cacheability").ToString());
				Assert.AreEqual("max-age=100", responseCache.GetValue("_cacheExtension").ToString());
				Assert.AreEqual("123456789", responseCache.GetValue("_etag").ToString());
				Assert.AreEqual(true, (bool)responseCache.GetValue("_isLastModifiedSet"));
				Assert.AreEqual(lastModified.ToUniversalTime(), (DateTime)responseCache.GetValue("_utcLastModified"));
			}
		}


		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void Test_ArgumentNullException()
		{
			string requestText = @"
GET http://www.fish-mvc-demo.com/Ajax/test/DataTypeTest/Input_string_ToUpper.aspx?input=fish HTTP/1.1
";
			using( WebContext context = WebContext.FromRawText(requestText) ) {

				HttpCacheResult result = new HttpCacheResult("abc", 100);
				
				result.Ouput(null);
			}
		}


		[ExpectedException(typeof(InvalidOperationException))]
		[TestMethod]
		public void Test_InvalidOperationException()
		{
			string requestText = @"
GET http://www.fish-mvc-demo.com/Ajax/test/DataTypeTest/Input_string_ToUpper.aspx?input=fish HTTP/1.1
";
			using( WebContext context = WebContext.FromRawText(requestText) ) {

				HttpCacheResult result = new HttpCacheResult("abc", 100);

				result.Ouput(context.HttpContext);
			}
		}

	}
}
