using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ClownFish.Web
{
	internal static class ExceptionHelper
	{
		public static void Throw403Exception(HttpContext context)
		{
			// 其实这个判断没什么意义，因为如果连HttpContext的实例也没有，抛出来的异常是无法告知用户的。
			// 但是如果不加，又感觉不严谨。
			if( context == null )
				throw new ArgumentNullException("context");

			throw new HttpException(403,
				"很抱歉，您没有合适的权限访问该资源：" + context.Request.RawUrl);
		}

		public static void Throw404Exception(HttpContext context)
		{
			if( context == null )
				throw new ArgumentNullException("context");

			throw new HttpException(404,
				"不能根据当前URL创建请求处理器，当前URL：" + context.Request.RawUrl);
		}
	}
}
