using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Log.Model;
using ClownFish.Log.Serializer;

namespace ClownFish.Log
{

	/// <summary>
	/// 日志时出现异常不能被处理时引用的事件参数
	/// </summary>
	public sealed class LogExceptionEventArgs : System.EventArgs
	{
		/// <summary>
		/// 新产生的异常实例
		/// </summary>
		public Exception Exception { get; internal set; }
	}



	/// <summary>
	/// 日志记录的工具类
	/// </summary>
	public static class LogHelper
	{
		/// <summary>
		/// 初始化锁
		/// </summary>
		private static readonly object s_lock = new object();

		/// <summary>
		/// 保存 【Type / ICacehQueue】的映射字典
		/// </summary>
		private static Hashtable s_queueDict = new Hashtable(128);

		/// <summary>
		/// 定时器，用于定时刷新所有的写入队列
		/// </summary>
		private static System.Threading.Timer s_timer;
		
		/// <summary>
		/// 写日志时出现异常不能被处理时引用的事件
		/// </summary>
		public static event EventHandler<LogExceptionEventArgs> OnError;
		
		/// <summary>
		/// 是否启用异常写入，默认就是启动，在测试时可以根据需要禁用。
		/// </summary>
		private static bool s_enableAsyncWrite = true;



		/// <summary>
		/// 以同步方式把消息写入日志
		/// 如果需要写入到指定的持久化方式，可以直接调用相应的 Writter ，就不需要调用这个方法。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="info"></param>
		public static void SyncWrite<T>(T info) where T : BaseInfo
		{
			// 先不对外开放这个方法！

			// 触发日志的配置检查
			WriterFactory.Init();


			// 获取写日志的实例，注意：允许一个类型配置多个写入方式
			ILogWriter[] writers = WriterFactory.CreateWriters(typeof(T));

			// 如果类型没有配置日志序列化器，就忽略
			if( writers == null || writers.Length == 0 )
				return;


			foreach( var writer in writers )
				writer.Write(info);
		}


		/// <summary>
		/// 记录指定的日志信息
		/// 说明：此方法是一个异步版本，内部维护一个缓冲队列，每5秒钟执行一次写入动作
		/// </summary>
		/// <typeparam name="T">日志信息的类型参数</typeparam>
		/// <param name="info">要写入的日志信息</param>
		public static void Write<T>(T info) where T: BaseInfo
		{
			// 触发日志的配置检查
			WriterFactory.Init();

			// 所有需要记录到日志的数据类型必须配置，否则不记录（因为不知道以什么方式记录）！
			if( WriterFactory.IsSupport(typeof(T)) == false )
				throw new NotSupportedException("不支持未配置的数据类型。");


			// 为第一次调用，创建定时器
			if( s_timer == null ) {
				lock( s_lock ) {
					if( s_timer == null )
						StartTimer();
				}
			}

			// 获取指定消息类型的写入队列，并将日志信息放入队列
			ICacheQueue queue = GetCacheQueue<T>();
			queue.Add(info);	
		}


		private static ICacheQueue GetCacheQueue<T>() where T: BaseInfo
		{
			Type t = typeof(T);

			// 这里使用了 Hashtable 的并发特有机制：允许 多个读，一个写

			ICacheQueue queue = s_queueDict[t] as ICacheQueue;
			if( queue == null ) {
				lock( (s_queueDict as ICollection).SyncRoot ) {

					queue = s_queueDict[t] as ICacheQueue;

					if( queue == null ) {
						queue = new CacheQueue<T>();
						s_queueDict[t] = queue;
					}
				}
			}

			return queue;
		}


		private static void Flush()
		{
			if( s_enableAsyncWrite == false )
				return;

			List<ICacheQueue> list = new List<ICacheQueue>();

			// 集中获取所有的写入队列，避免长时间占用锁
			// 后面创建的写入队列将在下次调度时获取到。

			lock( s_lock ) {
				foreach( DictionaryEntry e in s_queueDict )
					list.Add(e.Value as ICacheQueue);
			}


			foreach( ICacheQueue queue in list )
				queue.Flush();
		}


		private static void StartTimer()
		{
			int period = WriterFactory.Config.TimerPeriod;
			if( period <= 0 )
				return;

			s_timer = new System.Threading.Timer(TimerWorker, null, period, period);
		}


		private static void TimerWorker(object xxxx)
		{
			int period = WriterFactory.Config.TimerPeriod;

			// 在执行任务前，禁用定时器
			s_timer.Change(System.Threading.Timeout.Infinite, 0);


			try {
				Flush();
			}
			catch(Exception ex) {
				//定时器线程只能吃掉异常，否则程序就崩溃了，
				//不过，理论上不应该出现异常，这里只是一种预防措施
				RaiseErrorEvent(ex);
			}
			
			// 重新启动定时器
			s_timer.Change(period, period);
		}


		internal static void RaiseErrorEvent(Exception ex)
		{
			EventHandler<LogExceptionEventArgs> handler = OnError;

			try {
				if( handler != null ) {
					LogExceptionEventArgs e = new LogExceptionEventArgs{
						Exception = ex
					};
					handler(null, e);
				}					
			}
			catch { /* 当事件订阅时，再出异常就不能处理了，否则会形成循环调用。 */ }
		}


	}
}
