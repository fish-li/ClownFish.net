using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Xml;
using ClownFish.Log.Configuration;

namespace ClownFish.Log.Serializer
{
	/// <summary>
	/// 将日志记录到Windows日志的写入器
	/// </summary>
	public sealed class WinLogWriter : ILogWriter
	{
		private static bool s_initOK = false;

		private static string s_logName;
		private static string s_sourceName;

		#region ILogWriter 成员

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="config"></param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Init(WriterSection config)
		{
			string logName = config.GetOptionValue("LogName");
			if( string.IsNullOrEmpty(logName) )
				throw new LogConfigException("在配置文件中没有为WinLogWriter指定LogName属性。");

			string sourceName = config.GetOptionValue("SourceName");
			if( string.IsNullOrEmpty(sourceName) )
				throw new LogConfigException("在配置文件中没有为WinLogWriter指定SourceName属性。");

			try {
				// 尽量尝试为日志消息创建一个目录来存放

				// 下面这二个API都需要管理员权限才能调用，在ASP.NET程序中会出现异常
				// 当事件源存在时，调用SourceExists()不会出现异常，不存在时才会有异常

				if( EventLog.SourceExists(sourceName) == false )
					EventLog.CreateEventSource(sourceName, logName);

				s_logName = logName;
				s_sourceName = sourceName;
				s_initOK = true;
			}
			catch {
				// 如果权限不够，就直接存在到Application目录中。
				// 所以，这里不做异常处理
			}

			// ########### DEBUG INFO
			//EventLog.WriteEntry("Application Error ", "WinLogWriter.s_initOK: " + s_initOK.ToString());
			// ########### DEBUG INFO
		}

		/// <summary>
		/// 写入单条日志信息
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="info"></param>
		public void Write<T>(T info) where T : Model.BaseInfo
		{
			if( info == null )
				return;

			string xml = XmlHelper.XmlSerialize(info, Encoding.UTF8);

			if( s_initOK ) {
				using( EventLog myLog = new EventLog(s_logName, ".", s_sourceName) ) {
					myLog.WriteEntry(xml, EventLogEntryType.Information);
				}
			}
			else {
				// 如果不能注册日志和事件源，就写入应用程序日志中
				EventLog.WriteEntry("Application Error ", xml);
			}
		}

		/// <summary>
		/// 批量写入日志信息
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		public void Write<T>(List<T> list) where T : Model.BaseInfo
		{
			if( list == null || list.Count == 0 )
				return;

			if( s_initOK ) {
				using( EventLog myLog = new EventLog(s_logName, ".", s_sourceName) ) {
					foreach( T info in list ) {
						string xml = XmlHelper.XmlSerialize(info, Encoding.UTF8);
						myLog.WriteEntry(xml, EventLogEntryType.Information);
					}
				}
			}
			else {
				// 如果不能注册日志和事件源，就写入应用程序日志中
				foreach( T info in list ) {
					string xml = XmlHelper.XmlSerialize(info, Encoding.UTF8);
					EventLog.WriteEntry("Application Error", xml);
				}
			}
		}

		/// <summary>
		/// 根据日志ID获取单条日志信息
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="guid"></param>
		/// <returns></returns>
		public T Get<T>(Guid guid) where T : Model.BaseInfo
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// 根据指定的一段时间获取对应的日志记录
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t1"></param>
		/// <param name="t2"></param>
		/// <returns></returns>
		public List<T> GetList<T>(DateTime t1, DateTime t2) where T : Model.BaseInfo
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
