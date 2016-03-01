using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using ClownFish.Base.TypeExtend;
using ClownFish.Web.Debug404;
using ClownFish.Web.Reflection;


namespace ClownFish.Web
{
	/// <summary>
	/// 响应服务请求的HttpHandlerFactory。它要求将所有Action放在一个以Service结尾的类型中。
	/// </summary>
	public class ServiceHandlerFactory : BaseActionHandlerFactory
	{
		//private static readonly ControllerRecognizer s_recognizer = ObjectFactory.New<ControllerRecognizer>();

		private static readonly UrlParser s_UrlParser = ObjectFactory.New<UrlParser>();


		/// <summary>
		/// 解析URL，提取UrlActionInfo对象
		/// </summary>
		/// <param name="context"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public override UrlActionInfo ParseUrl(HttpContext context, string path)
		{
			return s_UrlParser.GetUrlActionInfo(context, path);
		}


		

	}
}
