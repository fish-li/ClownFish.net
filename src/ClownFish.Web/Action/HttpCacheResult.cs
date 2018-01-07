using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ClownFish.Web
{
	/// <summary>
	/// 表示需要设置HTTP缓存头的执行结果
	/// </summary>
	public sealed class HttpCacheResult : IActionResult
	{
		/// <summary>
		/// 要包装的Action执行结果
		/// </summary>
		public object Result{get; private set;}
		/// <summary>
		/// 缓存持续时间，单位：秒
		/// </summary>
		public int MaxAge { get; private set; }
		/// <summary>
		/// 设置此属性用于调用 SetLastModified 方法。
		/// </summary>
		public DateTime LastModified { get; set; }
		/// <summary>
		/// 设置此属性用于调用 SetETag 方法。
		/// </summary>
		public string ETag { get; set; }

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="result"></param>
		/// <param name="maxAge"></param>
		public HttpCacheResult(object result, int maxAge): 
			this(result, maxAge, DateTime.MinValue)
		{			
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="result"></param>
		/// <param name="maxAge"></param>
		/// <param name="lastModified"></param>
		public HttpCacheResult(object result, int maxAge, DateTime lastModified)
		{
			Result = result;
			MaxAge = maxAge;
			LastModified = lastModified;
		}

		/// <summary>
		/// 输出结果
		/// </summary>
		/// <param name="context"></param>
		public void Ouput(HttpContext context)
		{
			if( context == null )
				throw new ArgumentNullException("context");

			context.Response.Cache.SetCacheability(HttpCacheability.Public);


			if( MaxAge > 0 )
				context.Response.Cache.AppendCacheExtension("max-age=" + MaxAge.ToString());

			// LastModified 可以不设置，那么就是默认值：DateTime.MinValue
			// 判断的意思是：如果设置了LastModified，就调用 SetLastModified
			if( LastModified.Year > 2000 )
				context.Response.Cache.SetLastModified(LastModified);

			if( string.IsNullOrEmpty(ETag) == false )
				context.Response.Cache.SetETag(ETag);


			// 找回与当前请求有关的ActionExecutor实例。
			IActionHandler handler = context.Handler as IActionHandler;
			if( handler == null )
				throw new InvalidOperationException("HttpCacheResult只能与ActionHandler一起配合使用。");

			// 输出内部对象
			handler.ActionExecutor.ExecuteProcessResult(this.Result);
		}


	}
}
