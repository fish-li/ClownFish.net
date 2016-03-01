using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ClownFish.Web
{

	/// <summary>
	/// 提供DEBUG及辅助测试的工具类
	/// </summary>
	public static class DebugHelper
	{
		/// <summary>
		/// 将DEBUG信息写到响应头，格式："ThreadId, text"
		/// </summary>
		/// <param name="context"></param>
		/// <param name="text"></param>
		public static void WriteHeader(this HttpContext context, string text)
		{
			if( context == null )
				throw new ArgumentNullException("context");

			// 正式的发布模式，将不执行实现调用
			if( WebRuntime.Instance.IsDebugMode == false )
				return;

			// 获取计数器，为了对齐显示，从100开始
			object currentIndex = context.Items["DebugHelper-WriteHeader-Index"];
			if( currentIndex == null)
				currentIndex = 100;
			

			int index = (int)currentIndex + 1;
			context.Items["DebugHelper-WriteHeader-Index"] = index;

			string name = "debug-info-" + index.ToString();

			string value = string.Format("ThreadId: {0}, {1}", Thread.CurrentThread.ManagedThreadId, text);

			WebRuntime.Instance.WriteResponseHeader(context.Response, name, value);
		}


	}

}
