using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ClownFish.Log.Model;
using ClownFish.Log.Serializer;

namespace ClownFish.Log
{
	/// <summary>
	/// 性能日志Moudle，可直接在 web.config 中配置
	/// </summary>
	public sealed class PerformanceModule : IHttpModule
	{
		private static readonly string s_ItemKey = "289e8920-b291-4167-80b0-793cd46cad22";

		/// <summary>
		/// 实现IHttpModule的Init接口
		/// </summary>
		/// <param name="app"></param>
		public void Init(HttpApplication app)
		{
			// 确保配置文件已读取
			WriterFactory.Init();

			// 这里只记录 Handler 的执行时间，排除管线过程中HttpModule的执行时间
			app.PreRequestHandlerExecute += app_PreRequestHandlerExecute;
			app.PostRequestHandlerExecute += app_PostRequestHandlerExecute;
		}

		void app_PreRequestHandlerExecute(object sender, EventArgs e)
		{
			HttpApplication app = (HttpApplication)sender;
			app.Context.Items[s_ItemKey] = DateTime.Now;
		}

		void app_PostRequestHandlerExecute(object sender, EventArgs e)
		{
			DateTime currentTime = DateTime.Now;

			HttpApplication app = (HttpApplication)sender;
			object beforeValue = app.Context.Items[s_ItemKey];
			if( beforeValue == null )		// 防止其它代码清除了集合
				return;

			DateTime beforeTime = (DateTime)beforeValue;
			TimeSpan timeSpan = currentTime - (DateTime)beforeValue;

			if( timeSpan.TotalMilliseconds >= WriterFactory.Config.Performance.HttpExecuteTimeout ) {
				PerformanceInfo info = PerformanceInfo.CreateByHttp(
					app.Context,
					"HTP请求执行时间超出性能指标，已执行：" + timeSpan.ToString(),
					timeSpan);

				LogHelper.Write(info);
			}
		}

		internal static void CheckExecuteTime(DbCommand command, TimeSpan timeSpan)
		{
			if( command == null )
				return;

			if( timeSpan.TotalMilliseconds >= WriterFactory.Config.Performance.DbExecuteTimeout ) {
				PerformanceInfo info = PerformanceInfo.CreateBySql(
					command,
					"SQL请求执行时间超出性能指标，已执行：" + timeSpan.ToString(),
					timeSpan);

				LogHelper.Write(info);
			}
		}


		/// <summary>
		/// 实现IHttpModule的Init接口
		/// </summary>
		public void Dispose()
		{
		}

	}
}
