using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Reflection;
using System.Globalization;

namespace ClownFish.MockAspnetRuntime
{
	/// <summary>
	/// 模拟HttpRuntime
	/// </summary>
	public static class MockHttpRuntime
	{
		private static HttpRuntime s_runtime;	// = new HttpRuntime();

		static MockHttpRuntime()
		{
			// 触发HttpRuntime的静态构造函数
			string xx = HttpRuntime.AppDomainAppId;

			// 获取HttpRuntime的实例引用
			s_runtime = (HttpRuntime)typeof(HttpRuntime).GetStaticField("_theRuntime").GetValue(null);
		}

		internal static void EnsureInit()
		{
			// 什么事也不做，只用于确保执行静态构造函数。
		}

		/// <summary>
		/// 设置静态属性：HttpRuntime.AppDomainAppPath
		/// </summary>
		/// <param name="appDomainAppPath"></param>
		public static string AppDomainAppPath
		{
			set
			{
				if( string.IsNullOrEmpty(value) )
					throw new ArgumentNullException("value");


				typeof(HttpRuntime).GetInstanceField("_appDomainAppPath").SetValue(s_runtime, value);
			}
			get
			{
				return HttpRuntime.AppDomainAppPath;
			}
		}
		/// <summary>
		/// 设置静态属性：HttpRuntime.AppDomainAppVirtualPath
		/// </summary>
		public static string AppDomainAppVirtualPath
		{
			set
			{
				if( string.IsNullOrEmpty(value) )
					throw new ArgumentNullException("value");

				object vpath = WebHelper.CreatVirtualPath(value);

				typeof(HttpRuntime).GetInstanceField("_appDomainAppVPath").SetValue(s_runtime, vpath);
			}
			get
			{
				return HttpRuntime.AppDomainAppVirtualPath;
			}
		}


		
	}
}
