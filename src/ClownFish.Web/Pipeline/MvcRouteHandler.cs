using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;
using ClownFish.Web;
using ClownFish.Base.TypeExtend;
using ClownFish.Web.Debug404;
using ClownFish.Web.Reflection;

namespace ClownFish.Web
{
	// 路由配置请参考 AppInit.InitRouting 方法（搜索即可找到）。


	/// <summary>
	/// 实现 ASP.NET Routing IRouteHandler的路由处理器，可供UrlRoutingModule使用
	/// </summary>
	/// <example>
	/// RouteTable.Routes.Add(new Route("routing/{namespace}/{controller}/{action}", new MvcRouteHandler()));
	/// </example>
	public sealed class MvcRouteHandler : IRouteHandler
	{
		private static readonly ControllerRecognizer s_recognizer = ObjectFactory.New<ControllerRecognizer>();


		/// <summary>
		/// 实现IRouteHandler接口
		/// </summary>
		/// <param name="requestContext"></param>
		/// <returns></returns>
		public IHttpHandler GetHttpHandler(RequestContext requestContext)
		{
			HttpContext context = requestContext.HttpContext.ApplicationInstance.Context;

			UrlActionInfo info = GetUrlActionInfo(requestContext.RouteData, context);
			if( info == null ) {
				IHttpHandler handler = Http404DebugModule.TryGetHttp404PageHandler(context);
				if( handler != null )
					return handler;

				ExceptionHelper.Throw404Exception(context);
			}

			info.SetHttpcontext(context);

			return GetHttpHandler(context, info);
		}

		internal IHttpHandler GetHttpHandler(HttpContext context, UrlActionInfo info)
		{
			if( context == null )
				throw new ArgumentNullException("context");
			if( info == null )
				throw new ArgumentNullException("info");


			// 获取内部表示的调用信息
			ControllerResolver controllerResolver = new ControllerResolver(context);
			InvokeInfo vkInfo = controllerResolver.GetActionInvokeInfo(info);
			if( vkInfo == null ) {
				IHttpHandler handler = Http404DebugModule.TryGetHttp404PageHandler(context);
				if( handler != null )
					return handler;
				else
					return null;
			}


			// 创建能够调用Action的HttpHandler
			return ActionHandlerFactory.CreateHandler(vkInfo);
		}

		internal UrlActionInfo GetUrlActionInfo(RouteData routeData, HttpContext context)
		{
			if( routeData == null )
				throw new ArgumentNullException("routeData");
			if( context == null )
				throw new ArgumentNullException("context");

			// 采用ASP.NET Routing后，这三个参数都应该可以直接获取到，
			// 如果URL没有指定，可以通过默认值，或者DataToken指定，
			// 所以不需要像RestServiceModule那样重新计算
			string nspace = GetRouteString(routeData, "namespace");
			string className = GetRouteString(routeData, "controller");
			string action = GetRouteString(routeData, "action");

			if( string.IsNullOrEmpty(className) || string.IsNullOrEmpty(action) ) {
				DiagnoseResult diagnoseResult = Http404DebugModule.TryGetDiagnoseResult(context);
				if( diagnoseResult != null )
					diagnoseResult.ErrorMessages.Add("不能从URL中提取到controller和action信息");

				return null;
			}

			if( action == "{HttpMethod}" )		// 允许定义这个特殊变量
				action = context.Request.HttpMethod;


			ControllerResolver controllerResolver = new ControllerResolver(context);

			UrlActionInfo info = new UrlActionInfo();
			info.RoutePattern = (routeData.Route as Route).Url;		// 转换失败？？
			info.Namesapce =  controllerResolver.GetNamespaceMap(nspace);
			info.ClassName = className;
			info.MethodName = action;

			info.Action = action;
			info.Controller = s_recognizer.GetServiceFullName(info);


			// 将路由提取到的其它URL参数，保存到UrlActionInfo实例中。
			foreach( KeyValuePair<string, object> kvp in routeData.Values ) {

				// 排除3个特定名字。
				if( kvp.Key.EqualsIgnoreCase("namespace") || kvp.Key.EqualsIgnoreCase("controller") || kvp.Key.EqualsIgnoreCase("action") )
					continue;

				string value = kvp.Value as string;
				if( string.IsNullOrEmpty(value) == false )
					info.AddParam(kvp.Key, value);
			}

			return info;
		}


		private string GetRouteString(RouteData routeData, string name)
		{
			// 说明：这里不使用 RouteData.GetRequiredString 实例方法，因为它在找不到数据时会抛出异常。

			object obj;
			if( routeData.Values.TryGetValue(name, out obj) ) {
				string text = obj as string;
				if( string.IsNullOrEmpty(text) == false )
					return text;
			}

			if( routeData.DataTokens != null ) {
				if( routeData.DataTokens.TryGetValue(name, out obj) ) {
					string text = obj as string;
					if( string.IsNullOrEmpty(text) == false )
						return text;
				}
			}

			return null;
		}

	}
}
