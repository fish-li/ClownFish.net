using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using ClownFish.Web;

namespace MvcDemoWebSite1
{
	public class Global : System.Web.HttpApplication
	{
		private static Exception s_initException;

		protected void Application_Start(object sender, EventArgs e)
		{
			try {
				DEMO.Common.AppHelper.Init();

				DEMO.BLL.BllFactory.GetDatabase().AppStart();

				MvcDemoWebSite1.AppInit.InitRouting();



				// 为了兼容以前使用的 System.Web.Script.Serialization.JavaScriptSerializer 的使用，
				// 在这个示例网站的使用中，它与Newtonsoft.Json的主要差别在于日期的处理方式不同。
				Newtonsoft.Json.JsonConvert.DefaultSettings = () => new Newtonsoft.Json.JsonSerializerSettings {
					DateFormatHandling = Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat
				};
			}
			catch( Exception ex ) {
				// 记下初始化的异常。
				s_initException = ex;
			}
		}

		protected void Application_End(object sender, EventArgs e)
		{
			DEMO.BLL.BllFactory.GetDatabase().AppEnd();
		}


		protected void Application_BeginRequest(object sender, EventArgs e)
		{
			if( s_initException != null )
				throw s_initException;


			// 不允许直接访问特定样式的页面。
			// 因为那样会绕过ClownFish.Web框架，导致Page的Model成员没有机会赋值。

			HttpApplication app = (HttpApplication)sender;
			if( app.Request.FilePath.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase)
				&& app.Request.FilePath.StartsWith("/Pages/Style", StringComparison.OrdinalIgnoreCase) )
				app.Response.Redirect("/Pages/" + app.Request.FilePath.Substring(14), true);
		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{

		}

		protected void Application_Error(object sender, EventArgs e)
		{
			Exception ex = Server.GetLastError();
			DEMO.Common.AppHelper.SafeLogException(ex);



			// 判断是否为AJAX请求。
			// 如果是AJAX请求，我们可以不用做任何处理，
			// 因为前端已经有统一的全局处理逻辑。
			bool isAjaxRequest = string.Compare(
				Request.Headers["X-Requested-With"],
				"XMLHttpRequest", StringComparison.OrdinalIgnoreCase) == 0;

			if( isAjaxRequest == false ) {
				// 是一个页面请求，此时我们可以这样处理：
				// 1. 本机请求（调试），那就出现黄页。
				// 2. 来自其他用户的访问，显示自定义的错误显示页面

				if( Request.IsLocal == false ) {
					// 不是本机请求
					// 首先要清除异常，防止产生黄页。
					Server.ClearError();

					Response.StatusCode = 500;	// 继续设置500的响应，供IIS日志记录

					// 这里，我直接显示所有的异常信息，
					// 如果不希望这样显示，可以修改下面方法调用的第二个参数。
					IActionResult page = new PageResult("/Pages/Demo/ApplicationError.aspx", ex.ToString());
					page.Ouput(this.Context);
				}
			}	
		}


		
	}
}