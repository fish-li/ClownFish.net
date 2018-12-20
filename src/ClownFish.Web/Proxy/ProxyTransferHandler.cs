using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ClownFish.Web.Proxy
{
	/// <summary>
	/// 用于服务端代理转发请求的处理器
	/// </summary>
	public sealed class ProxyTransferHandler : HttpTaskAsyncHandler
	{
		/// <summary>
		/// 用于【外部模块】给 ProxyTransferHandler 传递目标网址
		/// </summary>
		public static readonly string TargetUrlKeyName = "x-target-url";



		/// <summary>
		/// 获取需要转发的目标地址
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		private string GetTransferTarget(HttpContext context)
		{
			// 扩展点：允许替换默认实现方式，从其它地方获取目标地址

			return (context.Items[TargetUrlKeyName] as string)
						?? context.Request.Headers[TargetUrlKeyName]
						?? context.Request[TargetUrlKeyName]
						;
		}



		/// <summary>
		/// 以异步方式执行HttpHanlder（基类方法重写）
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public async override Task ProcessRequestAsync(HttpContext context)
		{
			string destUrl = GetTransferTarget(context);
			if( string.IsNullOrEmpty(destUrl) )
				throw new ArgumentNullException("destUrl");


            string srcUrl = context.Request.Url.AbsoluteUri;
            context.Response.Headers.Add("X-ProxyTransferHandler", destUrl);	// 用于调试诊断

            HttpProxyHandler hander = new HttpProxyHandler(srcUrl, destUrl);
            await hander.ProcessRequestAsync(context);
        }


        

	}


}

