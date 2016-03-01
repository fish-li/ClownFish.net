using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using ClownFish.Web;

namespace MvcDemoWebSite1 { 
/// <summary>
/// 应用程序初始化
/// </summary>
	public class AppInit
	{
		public static void InitRouting()
		{
			// 说明：  MvcRoutingModule 和 UrlRoutingModule 共用 RouteTable，因此不能同时开启。

			#region 这些代码已移到配置文件中：ClownFish.Web.RouteTable.config ，如果要实现复杂的路由配置，请参考下面的代码实现

			//// 带命名空间的路由映射
			//RouteTable.Routes.Add(new Route("mvc-routing/{namespace}/{controller}/{action}", new MvcRouteHandler()));



			//// 也可以这样注册路由以及命名空间
			//Route route1 = new Route("mvc-routing-2/with-datataken-full-namespace/{controller}/{action}", new MvcRouteHandler());
			//route1.DataTokens = new RouteValueDictionary { { "namespace", "DEMO.Controllers.Ajax" } };
			//RouteTable.Routes.Add(route1);


			//Route route2 = new Route("mvc-routing-3/with-datataken-short-namespace/{controller}/{action}", new MvcRouteHandler());
			//route2.DataTokens = new RouteValueDictionary { { "namespace", "ns" } };
			//RouteTable.Routes.Add(route2);


			//Route route3 = new Route("file-download/demo1/{filename}", new MvcRouteHandler());
			//route3.DataTokens = new RouteValueDictionary { 
			//	{ "namespace", "DEMO.Controllers.Ajax" },
			//	{ "controller", "TestFile" },
			//	{ "action", "Download2" },
			//};
			//RouteTable.Routes.Add(route3);

			#endregion

		}


	}
}
