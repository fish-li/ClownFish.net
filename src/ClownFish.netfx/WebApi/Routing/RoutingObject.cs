using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using ClownFish.Base;

namespace ClownFish.WebApi.Routing
{
	/// <summary>
	/// 表示每个Action方法的路由描述信息
	/// </summary>
	internal class RoutingObject
	{
		public string Url { get; set; }

		public MethodInfo MethodInfo { get; set; }

		public TypeInfo ControllerType { get; set; }

		public HttpMethodAttribute[] Methods { get; set; }

		public Regex UrlRegex { get; set; }


		internal bool IsMatchMethod(string httpMethod)
		{
			if( this.Methods.IsNullOrEmpty() )
				return true;

			if( this.Methods.Any(x2 => x2.Name == httpMethod) )
				return true;

			return false;
		}
	}





}
