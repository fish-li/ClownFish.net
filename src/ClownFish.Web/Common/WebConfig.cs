using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using ClownFish.Base.Framework;

namespace ClownFish.Web
{
	internal static class WebConfig
	{

		/// <summary>
		/// 是否在web.config的pages配置节点开启了validateRequest参数。
		/// </summary>
		public static readonly bool ValidateRequest;

		static WebConfig()
		{
			if( RunTimeEnvironment.IsAspnetApp ) {

				PagesSection pagesSection =
							ConfigurationManager.GetSection("system.web/pages") as PagesSection;
				if( pagesSection != null )
					ValidateRequest = pagesSection.ValidateRequest;
			}
		}


	}
}
