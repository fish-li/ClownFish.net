using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Reflection;

namespace ClownFish.Base
{
	internal static class HttpInternalExtensions
    {
		private static readonly ActionWrapper<WebHeaderCollection, string, string> s_AddWithoutValidateInvoker;

		static HttpInternalExtensions()
		{
			// 使用这个内部方法写HTTP头会比较方便，
			// 因为有些头不允许直接添加，需要通过属性来设置，那样就需要一大堆的判断，写起来很麻烦。
			MethodInfo method = typeof(WebHeaderCollection).GetMethod(
				"AddWithoutValidate",
				BindingFlags.Instance | BindingFlags.NonPublic, null,
				new Type[] { typeof(string), typeof(string) }, null);

			s_AddWithoutValidateInvoker = new ActionWrapper<WebHeaderCollection, string, string>();
			s_AddWithoutValidateInvoker.BindMethod(method);
		}

		internal static void InternalAdd(this WebHeaderCollection headers, string name, string value)
		{
			s_AddWithoutValidateInvoker.Call(headers, name, value);
		}
	}
}
