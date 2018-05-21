using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ClownFish.Base
{
    /// <summary>
    /// 包含一些处理ASP.NET对象的扩展方法
    /// </summary>
    public static class HttpExtensions
    {
        /// <summary>
		/// 读取输入的流
		/// </summary>
		/// <param name="request">HttpRequest</param>
		/// <returns>输入的流</returns>
		public static string ReadInputStream(this HttpRequest request)
        {
            if( request == null )
                throw new ArgumentNullException(nameof(request));

            request.InputStream.Position = 0;
            using( StreamReader sr = new StreamReader(request.InputStream, request.ContentEncoding, true, 1024, true) ) {
                return sr.ReadToEnd();
            }
        }


        /// <summary>
        /// 一个标记字符串，用于访问 HttpContext.Items 集合
        /// </summary>
        private static readonly string RequestIdFlag = "RequestIdFlag-9faafca090a2496ab41e5972b82dcf77";

        /// <summary>
        /// 为每个请求设置 RequestID，此方法允许重复调用，仅当没有指定RequestID时有效。
        /// </summary>
        /// <param name="context"></param>
        public static void SetRequestId(this HttpContext context)
        {
            if( context == null )
                throw new ArgumentNullException(nameof(context));

            if( context.Items[RequestIdFlag] == null )
                context.Items[RequestIdFlag] = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// 获取当前请求的 RequestID，如果请求没有指定RequestID，则返回 null
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetRequestId(this HttpContext context)
        {
            if( context == null )
                throw new ArgumentNullException(nameof(context));

            return context.Items[RequestIdFlag] as string;
        }
    }
}
