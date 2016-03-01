using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Reflection;

namespace ClownFish.Web
{
	/// <summary>
	/// 定义一些用于扩展框架的注入方法
	/// </summary>
	public static class FrameworkExt
	{
		/// <summary>
		/// 注册用于转换HTTP请求数据到特定类型的转换器（不适用于序列化场景）
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="func"></param>
		public static void RegisterHttpDataConvert<T>(Func<HttpContext, ParameterInfo, T> func)
		{
			if( func == null )
				throw new ArgumentNullException("func");

			ClownFish.Web.Serializer.FormDataProvider.RegisterHttpDataConvert<T>(func);
		}
	}
}
