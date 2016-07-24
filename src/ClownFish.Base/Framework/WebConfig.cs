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
		/// 相当于HttpContext.IsDebuggingEnabled，不过那个属性是实例的，因此使用不方便，所以就重新实现了一个静态的版本。
		/// </summary>
		public static readonly bool IsDebugMode;


		static WebConfig()
		{

			if( RunTimeEnvironment.IsAspnetApp ) {
				CompilationSection compilationSection =
							ConfigurationManager.GetSection("system.web/compilation") as CompilationSection;
				if( compilationSection != null )
					IsDebugMode = compilationSection.Debug;
			}
		}


	}
}
