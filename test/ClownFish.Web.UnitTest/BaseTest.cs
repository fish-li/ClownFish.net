using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using ClownFish.Base.TypeExtend;
using ClownFish.MockAspnetRuntime;
using ClownFish.Web;
using ClownFish.Web.UnitTest.Ext;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace ClownFish.Web.UnitTest
{
	[TestClass]
	public abstract class BaseTest
	{
		private static readonly HttpRuntime HttpRuntimeInstance = new HttpRuntime();


		[AssemblyInitialize]
		public static void InitRuntime(TestContext context)
		{
			ExtenderManager.RegisterExtendType(typeof(MvcRuntimeExt));

			MockHttpRuntime.AppDomainAppPath = AppDomain.CurrentDomain.BaseDirectory;
			MockHttpRuntime.AppDomainAppVirtualPath = "/";
		}

		
	

		public string ExecuteService(string requestText)
		{
			using( WebContext context = WebContext.FromRawText(requestText) ) {
				return ExecuteService(context);
			}
		}

		public string ExecuteService(WebContext context)
		{
			ServiceHandlerFactory factory = new ServiceHandlerFactory();
			IHttpHandler handler = factory.GetHandler(context.HttpContext, null, null, null);
			handler.ProcessRequest(context.HttpContext);

			return context.Response.GetText();
		}


		public string ExecutePage(WebContext context)
		{
			IHttpHandlerFactory factory = new MvcPageHandlerFactory();
			IHttpHandler handler = factory.GetHandler(context.HttpContext, null, null, null);
			handler.ProcessRequest(context.HttpContext);

			return context.Response.GetText();
		}


		public async Task<string> AsyncExecuteService(string requestText)
		{
			using( WebContext context = WebContext.FromRawText(requestText) ) {
				ServiceHandlerFactory factory = new ServiceHandlerFactory();
				IHttpHandler handler = factory.GetHandler(context.HttpContext, null, null, null);

				HttpTaskAsyncHandler taskHandler = handler as HttpTaskAsyncHandler;
				if( taskHandler == null )
					throw new InvalidOperationException();

				await taskHandler.ProcessRequestAsync(context.HttpContext);

				return context.Response.GetText();
			}
		}
	}
}
