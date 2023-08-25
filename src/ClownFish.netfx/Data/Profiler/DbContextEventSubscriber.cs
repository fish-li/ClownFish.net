using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Http.Pipleline;

namespace ClownFish.Data.Profiler
{
	/// <summary>
	/// 订阅 ClownFish.Data.EventManager 的相关事件
	/// </summary>
	internal static class DbContextEventSubscriber 
	{
		public static void SubscribeEvent()
		{
			DbContextEvent.OnConnectionOpened += DbContextEvent_ConnectionOpened;
			DbContextEvent.OnAfterExecute += DbContextEvent_AfterExecute;
		}

		/// <summary>
		/// 尝试从当前请求中获取DbActionInfo列表，返回值有2上用途：
		/// 1、判断当前请求是不是需要开启数据访问监控，如果返回值为NULL就是不启用。
		/// 2、返回值用于存储请求过程中发生的所有数据访问操作
		/// </summary>
		/// <returns></returns>
		private static List<DbActionInfo> GetDbActionInfoList()
		{
			HttpPipelineContext pipelineContext = HttpPipelineContext.Get();
			if( pipelineContext == null )
				return null;

			// 集合在FiddlerProfilerModule.App_PostResolveRequestCache方法中创建
			return pipelineContext.HttpContext.Items[FiddlerProfilerModule.ContextItemKey] as List<DbActionInfo>;
		}

		private static void DbContextEvent_ConnectionOpened(object sender, OpenConnEventArgs e)
		{
			List<DbActionInfo> list = GetDbActionInfoList();
			if( list == null )
				return;

			DbActionInfo info = new DbActionInfo();
			// 一个特殊的字符串，标记是一个打开连接的操作，后面会在FiddlerPulgin中判断它
			info.SqlText = DbActionInfo.OpenConnectionFlag;	
			list.Add(info);
		}

		private static void DbContextEvent_AfterExecute(object sender, ExecuteCommandEventArgs e)
		{
			List<DbActionInfo> list = GetDbActionInfoList();
			if( list == null )
				return;

			DbActionInfo info = ConvertToDbActionInfo(e);
			if( info != null )
				list.Add(info);
		}

		private static DbActionInfo ConvertToDbActionInfo(ExecuteCommandEventArgs e)
		{
			// 注意：这里会对SQL语句做截断处理，因为有些场景下某些开发人员能拼接出好几M的SQL，
			// 影响网络传输和界面展示。

			DbActionInfo info = new DbActionInfo();
			info.Time = e.EndTime - e.StartTime;    // 计算执行时间

			// 如果是异步执行的命令，以注释形式在前面增加 async 的提示。
			info.SqlText = (e.IsAsync ? "[async] " : string.Empty) + e.DbCommand.CommandText.SubstringN(1024 * 1024 * 2);
			info.InTranscation = e.DbCommand.Transaction != null;	// 判断是否在事务中
			info.Parameters = new List<CommandParameter>();

			// 提取命令参数
			for( int i = 0; i < e.DbCommand.Parameters.Count; i++ ) {
				if( i < 64 ) {	// 只提取64个参数
					DbParameter parameter = e.DbCommand.Parameters[i];

					CommandParameter p = new CommandParameter();
					p.Name = parameter.ParameterName;
					p.DbType = parameter.DbType.ToString();

                    if( parameter.Value == null || parameter.Value == DBNull.Value)
                        p.Value = "NULL";
                    else
                        p.Value = parameter.Value.ToString().SubstringN(128); // 也做截断处理

					info.Parameters.Add(p);
				}
				else {
					// 防止在拼接IN条件，出现上千个参数！
					CommandParameter p = new CommandParameter();
					p.Name = "#####";
					p.DbType = "#####";
					p.Value = "参数太多，已被截断...，参数数量：" + e.DbCommand.Parameters.Count.ToString();
					info.Parameters.Add(p);
					break;
				}
			}


			if(e.Exception != null ) {
				info.ErrorMsg = e.Exception.GetBaseException().Message;
			}

			return info;
		}




	}
}
