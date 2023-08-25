using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Http.Pipleline;

namespace ClownFish.Data.Profiler
{
	/// <summary>
	/// FiddlerProfilerModule
	/// </summary>
	public sealed class FiddlerProfilerModule : NHttpModule
	{
		internal static readonly string ContextItemKey = "FiddlerProfilerModulef8c754d498af4c50a04eb767e8c48368";

		private static readonly object s_lock = new object();
		private static bool s_inited = false;

		/// <summary>
		/// Init
		/// </summary>
		public override void Init()
		{
			if( s_inited == false ) {
				lock( s_lock ) {
					if( s_inited == false ) {
						// 订阅 ClownFish.Data.DbContextEvent 的相关事件
						DbContextEventSubscriber.SubscribeEvent();
						s_inited = true;
					}
				}
			}
		}

		/// <summary>
		/// PreRequestExecute
		/// </summary>
		/// <param name="httpContext"></param>
		public override void PreRequestExecute(NHttpContext httpContext)
		{
			string headerValue = httpContext.Request.Header("X-Fiddler-Profiler");
			if( string.IsNullOrEmpty(headerValue) )
				return;


			// 如果Fiddler插件中的【数据库访问】 选项卡已启用
			if( headerValue.IndexOf("db", StringComparison.Ordinal) >= 0 ) {
				// 创建一个列表，用于存储当前请求过程中发生的数据访问操作
				httpContext.Items[ContextItemKey] = new List<DbActionInfo>(32);
			}
		}

		/// <summary>
		/// PostRequestExecute
		/// </summary>
		/// <param name="httpContext"></param>
		public override void PostRequestExecute(NHttpContext httpContext)
		{
			if( httpContext.Response.HasStarted )
				return;

			string headerValue = httpContext.Request.Header("X-Fiddler-Profiler");
			if( string.IsNullOrEmpty(headerValue) )
				return;

			if( headerValue.IndexOf("ar", StringComparison.Ordinal) >= 0 )
				// 输入一个响应头，回应Fiddler插件，可用于分析不规范请求的响应头
				httpContext.Response.SetHeader("X-Fiddler-AnalyzeRequest", "OK");
			

			List<DbActionInfo> list = httpContext.Items[ContextItemKey] as List<DbActionInfo>;
			if( list == null || list.Count == 0 )
				return;

			// 由于浏览器的响应头数量的限制，所以这里就只保留500条执行记录，因为500条已经够多了
			//if( list.Count > 500 )
			//	Keep500SqlAction(list);

			// 将SQL的执行过程输出到响应头
			WriteSqlAction(httpContext, list);
		}



		private void WriteSqlAction(NHttpContext httpContext, List<DbActionInfo> list)
		{
			// 计算数据库连接打开次数
			int connectionCount = 0;
			foreach( var info in list )
				if( info.SqlText == DbActionInfo.OpenConnectionFlag )
					connectionCount++;

			// 打开数据库的连接次数
			httpContext.Response.SetHeader("X-SQL-ConnectionCount", connectionCount.ToString());


			// 数据访问监控的响应头
			int index = 1;
			foreach( DbActionInfo info in list ) {
				string base64 = DbActionInfo.Serialize(info);
				string headerName = "X-SQL-Action-" + (index++).ToString();
				httpContext.Response.SetHeader(headerName, base64);
			}
		}

	}
}
