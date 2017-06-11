using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Web;

namespace DEMO.Controllers
{
	public class GlobalHttpApplication : System.Web.HttpApplication
	{
		private static Exception s_initException;

		protected void Application_Start(object sender, EventArgs e)
		{
			try {
				DEMO.Controllers.Common.AppHelper.Init();


				InitRouting();
			}
			catch( Exception ex ) {
				// 记下初始化的异常。
				s_initException = ex;
			}
		}


		private void InitRouting()
		{
			// 说明：  MvcRoutingModule 和 UrlRoutingModule 共用 RouteTable，因此不能同时开启。

			#region 这些代码已移到配置文件中：ClownFish.Web.RouteTable.config ，如果要实现复杂的路由配置，请参考下面的代码实现

			//// 带命名空间的路由映射
			//RouteTable.Routes.Add(new Route("mvc-routing/{namespace}/{controller}/{action}", new MvcRouteHandler()));



			//// 也可以这样注册路由以及命名空间
			//Route route1 = new Route("mvc-routing-2/with-datataken-full-namespace/{controller}/{action}", new MvcRouteHandler());
			//route1.DataTokens = new RouteValueDictionary { { "namespace", "DEMO.Controllers.Services" } };
			//RouteTable.Routes.Add(route1);


			//Route route2 = new Route("mvc-routing-3/with-datataken-short-namespace/{controller}/{action}", new MvcRouteHandler());
			//route2.DataTokens = new RouteValueDictionary { { "namespace", "ns" } };
			//RouteTable.Routes.Add(route2);


			//Route route3 = new Route("file-download/demo1/{filename}", new MvcRouteHandler());
			//route3.DataTokens = new RouteValueDictionary { 
			//	{ "namespace", "DEMO.Controllers.Services" },
			//	{ "controller", "TestFile" },
			//	{ "action", "Download2" },
			//};
			//RouteTable.Routes.Add(route3);

			#endregion

		}

		protected void Application_End(object sender, EventArgs e)
		{
		}
		

		protected void Application_Error(object sender, EventArgs e)
		{
			Exception ex = Server.GetLastError();
			DEMO.Controllers.Common.AppHelper.SafeLogException(ex);



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

					Response.StatusCode = 500;  // 继续设置500的响应，供IIS日志记录

					// 这里，我直接显示所有的异常信息，
					// 如果不希望这样显示，可以修改下面方法调用的第二个参数。
					IActionResult page = new PageResult("/Pages/Demo/ApplicationError.aspx", ex.ToString());
					page.Ouput(this.Context);
				}
			}
		}



	}
}
