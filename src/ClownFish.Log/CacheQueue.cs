using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Log.Model;
using ClownFish.Log.Serializer;

namespace ClownFish.Log
{

	/// <summary>
	/// 为了方便而定义的一个弱类型接口
	/// </summary>
	internal interface ICacheQueue
	{
		void Add(object info);

		void Flush();
	}


	/// <summary>
	/// 缓存日志信息的写入队列
	/// </summary>
	/// <typeparam name="T"></typeparam>
	internal class CacheQueue<T> : ICacheQueue where T : BaseInfo
	{
		/// <summary>
		/// 队列锁
		/// </summary>
		private readonly object _lock = new object();

		/// <summary>
		/// 静态缓冲队列
		/// </summary>
		private readonly List<T> _list = new List<T>(1024);


		#region ICacehQueue 成员

		public void Add(object info)
		{
			// 将弱类型变成强类型
			T tInfo = (T)info;
			Add(tInfo);
		}

		#endregion

		/// <summary>
		/// 写入一条日志信息到缓冲队列
		/// </summary>
		/// <param name="info"></param>
		public void Add(T info)
		{
			// 外部调用写日志时，其实只是将日志信息写入静态队列，用于缓冲写入压力
			lock( _lock ) {
				_list.Add(info);
			}
		}


		/// <summary>
		/// 供外部定时器调用，一次性写入所有等待消息
		/// 此方法由定时器线程调用。
		/// </summary>
		public void Flush()
		{
			List<T> tempList = null;

			lock( _lock ) {
				if( _list.Count > 0 ) {
					// 将静态队列的数据转移到临时队列，避免在后面写操作时长时间占用锁
					tempList = (from x in _list select x).ToList();

					// 清空静态队列
					_list.Clear();
				}
			}

			if( tempList == null )
				return;			// 没有需要写入的日志信息


			// 获取写日志的实例，注意：允许一个类型配置多个写入方式
			ILogWriter[] writers = WriterFactory.GetWriters(typeof(T));

			// 如果类型没有配置日志序列化器，就忽略
			if( writers == null || writers.Length == 0 )
				return;


			List<Exception> exceptions = new List<Exception>();		// 临时保存写日志过程的异常

			foreach( var writer in writers ) {
				try {
					writer.Write(tempList);
				}
				catch( Exception ex ) {
					exceptions.Add(ex);
				}
			}

			// 为了便于单元测试，所以提炼了一个内部方法
			ProcessFlushException(exceptions);
		}


		private void ProcessFlushException(List<Exception> exceptions)
		{
			if( exceptions.Count > 0 ) {
				ILogWriter retryLogger = WriterFactory.GetRetryWriter();		// 获取重试日志序列化器
				if( retryLogger != null ) {
					try {
						List<ExceptionInfo> list = (from x in exceptions select ExceptionInfo.Create(x)).ToList();
						retryLogger.Write(list);
					}
					catch( Exception ex ) {
						LogHelper.RaiseErrorEvent(ex);
					}
				}
				else {
					foreach( Exception ex in exceptions )
						LogHelper.RaiseErrorEvent(ex);
				}


			}
		}

		
	}
}
