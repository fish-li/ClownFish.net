using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;
using ClownFish.Base;
using ClownFish.Base.Xml;
using ClownFish.Web.Debug404;
using ClownFish.Web.Reflection;


namespace ClownFish.Web
{
	// 路由配置请参考 AppInit.InitRouting 方法（搜索即可找到）。

	/// <summary>
	/// 比 UrlRoutingModule 3.5版本更简单的RoutingModule，而且支持404错误诊断。
	/// 【注意】：MvcRoutingModule 不能和 ASP.NET 的 UrlRoutingModule 同时启用。
	/// </summary>
	public sealed class MvcRoutingModule : IHttpModule
	{
		private static bool s_inited = false;
		private static readonly object s_lock = new object();
		

		private static void Init()
		{
			if( s_inited == false ) {
				lock( s_lock ) {
					if( s_inited == false ) {
						InitRouting();

						s_inited = true;
					}
				}
			}
		}

		/// <summary>
		/// 给 ASP.NET RouteTable 注册路由规则
		/// </summary>
		private static void InitRouting()
		{
			string filePath = WebRuntime.Instance.GetPhysicalPath("ClownFish.Web.RouteTable.config");

			if( RetryFile.Exists(filePath) == false )
				throw new FileNotFoundException("未能找到文件：" + filePath + " ，如果要启用 MvcRoutingModule，必须配置这个文件。");


			ClownFish.Web.Config.RouteTableConfig config 
							= XmlHelper.XmlDeserializeFromFile<ClownFish.Web.Config.RouteTableConfig>(filePath);

			if( config.Routes != null ) {
				foreach( var route in config.Routes ) {
					if( string.IsNullOrEmpty(route.Url) )
						throw new System.Configuration.ConfigurationErrorsException("路由规则配置，必须要指定URL属性。");

					RouteValueDictionary values = new RouteValueDictionary();

					if( string.IsNullOrEmpty(route.Namespace) == false )
						values.Add("namespace", route.Namespace);

					if( string.IsNullOrEmpty(route.Controller) == false )
						values.Add("controller", route.Controller);

					if( string.IsNullOrEmpty(route.Action) == false )
						values.Add("action", route.Action);


					System.Web.Routing.Route routeRule = new System.Web.Routing.Route(route.Url, new MvcRouteHandler());
					if( values.Count > 0 )
						routeRule.DataTokens = values;

					if( string.IsNullOrEmpty(route.Name) )
						RouteTable.Routes.Add(routeRule);
					else
						RouteTable.Routes.Add(route.Name, routeRule);

				}
			}
		}




		/// <summary>
		/// 实现IHttpModule接口
		/// </summary>
		/// <param name="app"></param>
		public void Init(HttpApplication app)
		{
			Init();		// 确保已加载路由表

			app.PostResolveRequestCache += new EventHandler(app_PostResolveRequestCache);
		}

		void app_PostResolveRequestCache(object sender, EventArgs e)
		{
			HttpApplication app = (HttpApplication)sender;
			HttpContext context = app.Context;

			// 如果已经被处理过了，就不再处理。
			if( context.Handler != null  )
				return;
			
			string virtualPath = context.GetRealVirtualPath();



			// 检查有没有匹配的路由规则
			RouteData routeData = GetRoute(context, virtualPath);
			if( routeData == null )
				return;		// 没有就忽略，重新交给ASP.NET管线继续处理。



			MvcRouteHandler routeHandler = routeData.RouteHandler as MvcRouteHandler;
			if( routeHandler == null )
				//return;	// 忽略就会产生404错误了。因此为了排查方便，抛出异常。
				throw new InvalidProgramException("使用ClownFish.Web.MvcRoutingModule时，必须指定MvcRouteHandler");


			UrlActionInfo info = routeHandler.GetUrlActionInfo(routeData, context);
			info.SetHttpcontext(context);


			IHttpHandler handler = routeHandler.GetHttpHandler(context, info);
			if( handler != null ) 
				context.RemapHandler(handler);			
			else
				ExceptionHelper.Throw404Exception(context);
		}



		private RouteData GetRoute(HttpContext context, string virtualPath)
		{
			HttpContextWrapper contextWrapper = new HttpContextWrapper(context);


			// 利用ASP.NET Routing解析URL
			RouteData routeData = RouteTable.Routes.GetRouteData(contextWrapper);
			if( routeData == null ) {
				DiagnoseResult diagnoseResult = Http404DebugModule.TryGetDiagnoseResult(context);
				if( diagnoseResult != null ) {
					diagnoseResult.ErrorMessages.Add("URL不能与任何路由配置(RouteTable)匹配：" + virtualPath);
					diagnoseResult.RouteTestResult = (from x in RouteTable.Routes
													  let route = x as Route
													  where route != null
													  select new TestResult { Text = route.Url, IsPass = false }).ToList();
				}
				return null;
			}

			if( routeData.RouteHandler != null && routeData.RouteHandler is StopRoutingHandler ) {
				DiagnoseResult diagnoseResult = Http404DebugModule.TryGetDiagnoseResult(context);
				if( diagnoseResult != null )
					diagnoseResult.ErrorMessages.Add("路由匹配成功，结果是一个StopRoutingHandler");
				
				return null;
			}

			return routeData;
		}



		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
		}


		
	}
}
