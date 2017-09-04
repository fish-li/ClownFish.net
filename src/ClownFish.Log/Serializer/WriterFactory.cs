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
		internal static readonly string ExceptionWriterKey = "09ce3042-4dca-492c-9ac5-27d243b1903e";


		private static bool s_inited = false;
		private static readonly object s_lock = new object();

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


        private static void InternalInit(LogConfig config)
        {
            Config = config;    // 注意：这里不克隆参数对应，直接引用它

            ConfigLoader loader = new ConfigLoader();
            loader.Load(config, s_writerTable);
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

			DataTypeCacheItem cacheItem = (DataTypeCacheItem)s_writerTable[t.FullName];
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

			DataTypeCacheItem cacheItem = (DataTypeCacheItem)s_writerTable[t.FullName];
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
			Type t = (Type)s_writerTable[ExceptionWriterKey];
			if( t == null )
				return null;

			return (ILogWriter)Activator.CreateInstance(t);
		}
	}
}
