using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ClownFish.Base;

namespace ClownFish.WebApi.Routing
{
	internal static class RouteHelper
	{
		private static readonly Regex s_regex = new Regex(@"{(\w+)}", RegexOptions.Compiled);

		/// <summary>
		/// 将包含了占位符模式的字符串翻译成等效的正则表达式
		/// </summary>
		/// <param name="pattern"></param>
		/// <returns></returns>
		public static Regex CreateRegex(string pattern)
		{
			string newString = s_regex.Replace(pattern, @"(?<$1>\w+)");
			return new Regex(newString, RegexOptions.Compiled | RegexOptions.IgnoreCase);


			// input:   /page/{id}/{year}-{month}-{day}.aspx
			// output:  /page/(?<id>\w+)/(?<year>\w+)-(?<month>\w+)-(?<day>\w+).aspx
		}


		public static RoutingObject CreateRoutingObject(Type t, MethodInfo m, RouteAttribute a1, RouteAttribute a2)
		{
			RoutingObject routing = new RoutingObject {
				Url = a2.Url.StartsWith("/", StringComparison.Ordinal) ? a2.Url : a1.Url + a2.Url,
				ControllerType = t.GetTypeInfo(),
				MethodInfo = m,
				Methods = m.GetMyAttributes<HttpMethodAttribute>()
			};

			if( routing.Url.IndexOfIgnoreCase("[controller]") > 0 ) {

				string name = t.Name;
				if( name.EndsWithIgnoreCase("Controller") ) {

					name = name.Substring(0, name.Length - 10);
					routing.Url = routing.Url.Replace("[controller]", name);
				}
			}

			if( routing.Url.IndexOf('{') >= 0 )
				routing.UrlRegex = RouteHelper.CreateRegex(routing.Url);

			return routing;
		}


	}
}
