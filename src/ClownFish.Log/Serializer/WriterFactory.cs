using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Xml;
using ClownFish.Log.Configuration;

namespace ClownFish.Log.Serializer
{

	internal static class WriterFactory
	{
		private static bool s_inited = false;
		private static readonly object s_lock = new object();

		/// <summary>
		/// 日志的配置信息
		/// </summary>
		public static LogConfig Config { get; private set; }

		/// <summary>
		/// 数据类型和Writer的映射表
		/// </summary>
		private static Hashtable s_writerTable = Hashtable.Synchronized(new Hashtable(16));

		/// <summary>
		/// 表示日志持久化时出现异常后的Writer键名，用于查找s_writerTable字典
		/// </summary>
		private static readonly string s_ExceptionWriterKey = "09ce3042-4dca-492c-9ac5-27d243b1903e";


		private class CacheItem
		{
			public Type[] WriteTypes { get; set; }

			public ILogWriter[] Instances { get; set; }
		}


		public static void Init()
		{
			if( s_inited )
				return;

			LogConfig config = LogConfig.ReadConfigFile();

			Init(config);
		}


		public static void Init(LogConfig config)
		{
			if( config == null )
				throw new ArgumentNullException("config");


			if( s_inited == false ) {
				lock( s_lock ) {
					if( s_inited == false ) {
						InternalInit(config);

						// 标记初始化已成功
						s_inited = true;
					}
				}
			}
		}


		internal static void InternalInit(LogConfig config)
		{
			// 检查配置参数是否正确
			CheckConfig(config);


			// 保存配置信息
			Config = config;		// 要不要克隆对象？？


			// 初始化各个 Writer
			foreach( BaseWriterConfig writerConfig in config.Writers.GetWriters() ) {
				Type t = Type.GetType(writerConfig.WriteType, true);
				ILogWriter writer = Activator.CreateInstance(t) as ILogWriter;

				writer.Init();
			}
		}
		


		internal static void CheckConfig(LogConfig config)
		{
			if( config.Types == null || config.Types.Count == 0 || config.Writers == null )
				throw new LogConfigException("日志配置文件中，没有配置Writers和Types节点。");


			// 检查无效的数据类型定义
			foreach( var item in config.Types ) {
				if( string.IsNullOrEmpty(item.DataType) )
					throw new LogConfigException("日志配置文件中，不允许DataType属性为空。");

				if( item.Writers == null || item.Writers.Trim(';', ',').Length == 0 )
					throw new LogConfigException("日志配置文件中，不允许Writers属性为空。");

				// 如果指定的类型不正确，下面代码会抛出异常
				item.Type = Type.GetType(item.DataType, true);
			}


			Dictionary<string, string> writerNameTypeDict = new Dictionary<string, string>();

			// 构建各个 Writer 的名称和类型的映射表
			foreach( BaseWriterConfig writerConfig in config.Writers.GetWriters() ) {
				// 如果不启用某个Writer，可以不指定WriteType属性
				if( writerConfig == null || string.IsNullOrEmpty(writerConfig.WriteType) )
					continue;

				writerConfig.Valid();

				Type t = Type.GetType(writerConfig.WriteType, true);
				ILogWriter writer = Activator.CreateInstance(t) as ILogWriter;
				if( writer == null )
					throw new LogConfigException("日志配置文件中，指定的WriteType没有实现接口ILogWriter：" + writerConfig.WriteType);

				writerNameTypeDict[writerConfig.Name] = writerConfig.WriteType;
			}


			// 如果没有正确配置
			if( writerNameTypeDict.Count == 0 )
				throw new LogConfigException("日志配置文件中，没有配置有效的Writers");


			// 增加一个内置的，可用于开发环境中不写日志
			writerNameTypeDict["NULL"] = "ClownFish.Log.Serializer.NullWriter, ClownFish.Log";



			// 只有当配置是有效的时候，才做进一步操作
			if( config.Enable ) {

				if( string.IsNullOrEmpty(config.ExceptionWriter) == false ) {
					string writerTypeName = null;
					if( writerNameTypeDict.TryGetValue(config.ExceptionWriter, out writerTypeName) )
						// 保存写日志发生异常的Writer，单个实例
						try {
							Type t = Type.GetType(writerTypeName, true);
							ILogWriter instance = (ILogWriter)Activator.CreateInstance(t);	// 测试类型是否符合要求
							s_writerTable[s_ExceptionWriterKey] = t;
						}
						catch( Exception ex ) {
							throw new LogConfigException("日志配置文件中，指定的Writer无效：" + writerTypeName, ex);
						}
					else
						throw new LogConfigException("日志配置文件中，指定的Writer无效：" + config.ExceptionWriter);
				}

				foreach( var item in config.Types ) {
					string[] writers = item.Writers.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
					List<Type> typeList = new List<Type>(writers.Length);

					for( int i = 0; i < writers.Length; i++ ) {
						string value = null;
						if( writerNameTypeDict.TryGetValue(writers[i].Trim(), out value) ) {
							try {
								typeList.Add(Type.GetType(value, true));		// 如果类型名称不正确，这里会出现异常
							}
							catch( Exception ex ) {
								throw new LogConfigException("日志配置文件中，指定的Writer无效：" + writers[i], ex);
							}
						}
						else
							throw new LogConfigException("日志配置文件中，指定的Writer无效：" + writers[i]);
					}

					// 保存各种数据类型对应的Writer数组
					try {
						CacheItem cacheItem = new CacheItem();
						cacheItem.WriteTypes = typeList.ToArray();
						cacheItem.Instances = (from x in typeList
											   select (ILogWriter)Activator.CreateInstance(x)	// 如果配置有错，会出现异常！
												   ).ToArray();

						Type t = Type.GetType(item.DataType, true);
						s_writerTable[t.FullName] = cacheItem;
					}
					catch( Exception ex ) {
						throw new LogConfigException("日志配置文件中，指定的Writer无效：" + item.DataType, ex);
					}
				}
			}

		}
		
		
		/// <summary>
		/// 判断指定的数据类型是否已配置到支持列表
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public static bool IsSupport(Type t)
		{
			if( Config.Enable == false )
				return false;

			return s_writerTable[t.FullName] != null;			
		}

		/// <summary>
		/// 创建指定类型的日志序列化实例
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public static ILogWriter[] CreateWriters(Type t)
		{
			if( Config.Enable == false )
				return null;

			CacheItem cacheItem = (CacheItem)s_writerTable[t.FullName];
			if( cacheItem == null )
				return null;


			return (from x in cacheItem.WriteTypes
					select (ILogWriter)Activator.CreateInstance(x)
												   ).ToArray();
		}

		/// <summary>
		/// 获取指定类型的日志序列化实例（从缓存中获取）
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public static ILogWriter[] GetWriters(Type t)
		{
			if( Config.Enable == false )
				return null;

			CacheItem cacheItem = (CacheItem)s_writerTable[t.FullName];
			if( cacheItem == null )
				return null;

			return cacheItem.Instances;
		}


		/// <summary>
		/// 获取写日志失败时的【重试】序列化实例
		/// 例如：正常情况下写数据库，失败就写入Windows日志
		/// 如果重试失败，就忽略记录操作
		/// </summary>
		/// <returns></returns>
		public static ILogWriter GetRetryWriter()
		{
			Type t = (Type)s_writerTable[s_ExceptionWriterKey];
			if( t == null )
				return null;

			return (ILogWriter)Activator.CreateInstance(t);
		}
	}
}
