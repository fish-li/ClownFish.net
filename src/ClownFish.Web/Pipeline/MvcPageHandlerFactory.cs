using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using ClownFish.Web.Debug404;
using ClownFish.Web.Reflection;


namespace ClownFish.Web
{

	internal sealed class AspnetPageHandlerFactory : PageHandlerFactory { }

	/// <summary>
	/// MvcPageHandlerFactory
	/// </summary>
	public sealed class MvcPageHandlerFactory : IHttpHandlerFactory
	{
		private readonly AspnetPageHandlerFactory _msPageHandlerFactory = new AspnetPageHandlerFactory();

		IHttpHandler IHttpHandlerFactory.GetHandler(HttpContext context, 
							string requestType, string virtualPath, string physicalPath)
		{
			// 说明：这里不使用virtualPath变量，因为不同的配置，这个变量的值会不一样。
			// 例如：/mvc/*/*.aspx 和 /mvc/*
			// 为了映射HTTP处理器，下面直接使用context.Request.Path

			string requestPath = context.Request.Path;
			string vPath = context.GetRealVirtualPath();

			// 尝试根据请求路径获取Action
			ControllerResolver controllerResolver = new ControllerResolver(context);
			InvokeInfo vkInfo = controllerResolver.GetActionInvokeInfo(vPath);
			
			// 如果没有找到合适的Action，并且请求的是一个ASPX页面，则按ASP.NET默认的方式来继续处理
			if( vkInfo == null  ) {
				if( requestPath.EndsWithIgnoreCase(".aspx")
					&& System.IO.File.Exists(context.Request.PhysicalPath) ) {
					// 调用ASP.NET默认的Page处理器工厂来处理
					return _msPageHandlerFactory.GetHandler(context, requestType, requestPath, physicalPath);
				}
				else
					ExceptionHelper.Throw404Exception(context);
			}

			return ActionHandlerFactory.CreateHandler(vkInfo);
		}

		void IHttpHandlerFactory.ReleaseHandler(IHttpHandler handler)
		{			
		}


	}

	
}
