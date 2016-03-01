using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using ClownFish.Web.Debug404;
using ClownFish.Web.Reflection;


namespace ClownFish.Web
{
	/// <summary>
	/// BaseActionHandlerFactory
	/// </summary>
	public abstract class BaseActionHandlerFactory : IHttpHandlerFactory
	{
		/// <summary>
		/// 解析URL，提取UrlActionInfo对象
		/// </summary>
		/// <param name="context"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public abstract UrlActionInfo ParseUrl(HttpContext context, string path);

		/// <summary>
		/// 实现IHttpHandlerFactory接口，从当前请求获取IHttpHandler
		/// </summary>
		/// <param name="context"></param>
		/// <param name="requestType"></param>
		/// <param name="virtualPath"></param>
		/// <param name="physicalPath"></param>
		/// <returns></returns>
		public IHttpHandler GetHandler(HttpContext context,
							string requestType, string virtualPath, string physicalPath)
		{
			// 说明：这里不使用virtualPath变量，因为不同的配置，这个变量的值会不一样。
			// 例如：/Ajax/*/*.aspx 和 /Ajax/*
			// 为了映射HTTP处理器，下面直接使用context.Request.Path

			string vPath = context.GetRealVirtualPath();

			

			// 根据请求路径，定位到要执行的Action
			UrlActionInfo info = ParseUrl(context, vPath);
			if( info == null ) {
				IHttpHandler handler = Http404DebugModule.TryGetHttp404PageHandler(context);
				if( handler != null )
					return handler;

				ExceptionHelper.Throw404Exception(context);
			}

			info.SetHttpcontext(context);

			

			// 获取内部表示的调用信息
			ControllerResolver controllerResolver = new ControllerResolver(context);
			InvokeInfo vkInfo = controllerResolver.GetActionInvokeInfo(info);
			if( vkInfo == null ) {
				IHttpHandler handler = Http404DebugModule.TryGetHttp404PageHandler(context);
				if( handler != null )
					return handler;

				ExceptionHelper.Throw404Exception(context);
			}

			// 创建能够调用Action的HttpHandler
			return ActionHandlerFactory.CreateHandler(vkInfo);
		}

		/// <summary>
		/// 实现IHttpHandlerFactory接口
		/// </summary>
		/// <param name="handler"></param>
		public void ReleaseHandler(IHttpHandler handler)
		{
		}


	}
}
