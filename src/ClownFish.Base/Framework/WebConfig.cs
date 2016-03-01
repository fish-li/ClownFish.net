using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.Configuration;

namespace ClownFish.Base.Framework
{
	internal static class WebConfig
	{
		/// <summary>
		/// 当前运行的程序是不是ASP.NET程序
		/// </summary>
		public static readonly bool IsAspnetApp;

		/// <summary>
		/// 相当于HttpContext.IsDebuggingEnabled，不过那个属性是实例的，因此使用不方便，所以就重新实现了一个静态的版本。
		/// </summary>
		public static readonly bool IsDebugMode;

		/// <summary>
		/// 是否在web.config的pages配置节点开启了validateRequest参数。
		/// </summary>
		public static readonly bool ValidateRequest;

		static WebConfig()
		{
			IsAspnetApp = string.IsNullOrEmpty(System.Web.HttpRuntime.AppDomainAppId) == false;

			if( IsAspnetApp ) {
				CompilationSection compilationSection =
							ConfigurationManager.GetSection("system.web/compilation") as CompilationSection;
				if( compilationSection != null )
					IsDebugMode = compilationSection.Debug;

				PagesSection pagesSection =
							ConfigurationManager.GetSection("system.web/pages") as PagesSection;
				if( pagesSection != null )
					ValidateRequest = pagesSection.ValidateRequest;
			}
		}


	}
}
