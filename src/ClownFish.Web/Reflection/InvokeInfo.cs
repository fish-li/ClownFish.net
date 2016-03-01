using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ClownFish.Web.Reflection
{

	internal sealed class InvokeInfo
	{
		public ControllerDescription Controller { get; set; }
		public ActionDescription Action { get; set; }

		/// <summary>
		/// Controller Instance
		/// </summary>
		public object Instance { get; set; }
		/// <summary>
		/// 应用到 PageController Action 的PageRegexUrlAttribute的正则表达式
		/// </summary>
		public Match RegexMatch { get; set; }
		/// <summary>
		/// 解析 ServiceController 时提取的URL信息
		/// </summary>
		public UrlActionInfo UrlActionInfo { get; set; }


		public OutputCacheAttribute GetOutputCacheSetting()
		{
			if( this.Action != null && this.Action.OutputCache != null )
				return this.Action.OutputCache;
			if( this.Controller != null && this.Controller.OutputCache != null )
				return this.Controller.OutputCache;			
			return null;
		}
		public SessionMode GetSessionMode()
		{
			if( this.Action != null && this.Action.SessionMode != null )
				return this.Action.SessionMode.SessionMode;
			if( this.Controller != null && this.Controller.SessionMode != null )
				return this.Controller.SessionMode.SessionMode;			
			return SessionMode.NotSupport;
		}
		public AuthorizeAttribute GetAuthorize()
		{
			if( this.Action != null && this.Action.Authorize != null )
				return this.Action.Authorize;
			if( this.Controller != null && this.Controller.Authorize != null )
				return this.Controller.Authorize;
			return null;
		}
	}


	
}
