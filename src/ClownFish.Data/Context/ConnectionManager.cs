using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Data
{
	/// <summary>
	/// 管理数据库的连接信息。
	/// 即使不调用，也会自动调用Init()方法一次。
	/// </summary>
	public static class ConnectionManager
	{
		private static bool s_inited = false;
		private static readonly object s_lock = new object();

		/// <summary>
		/// 默认的连接（第一个注册的连接）
		/// </summary>
		private static ConnectionStringSettings s_defaultConnection;

		private static ConcurrentDictionary<string, ConnectionStringSettings> s_dict 
							= new ConcurrentDictionary<string, ConnectionStringSettings>();


		/// <summary>
		/// 用web.config或者app.config中的数据库连接信息初始化，
		/// 框架会默认使用这种方式完成初始化过程。
		/// </summary>
		public static void Init()
		{
			if( s_inited )
				return;

			Init(ConfigurationManager.ConnectionStrings);
		}

		/// <summary>
		/// 使用指定的配置文件初始化连接信息，配置文件要求包含ConnectionStringSettings配置节点
		/// </summary>
		/// <param name="configFilePath"></param>
		public static void Init(string configFilePath)
		{
			if( s_inited )
				throw new InvalidOperationException("已经调用过Init方法，不允许重复调用。");

			ExeConfigurationFileMap filemap = new ExeConfigurationFileMap();
			filemap.ExeConfigFilename = configFilePath;

			Configuration config = ConfigurationManager.OpenMappedExeConfiguration(
													filemap, ConfigurationUserLevel.None);

			Init(config.ConnectionStrings.ConnectionStrings);
		}

		private static void Init(ConnectionStringSettingsCollection collection)
		{
			if( collection == null )
				return;

			if( s_inited == false ) {
				lock( s_lock ) {
					if( s_inited == false ) {
						// 注册每个连接配置
						foreach( ConnectionStringSettings setting in collection )
							RegisterConnection(setting);

						// 标记初始化完成
						s_inited = true;
					}
				}
			}			
		}

		/// <summary>
		/// 直接注册数据库连接
		/// </summary>
		/// <param name="setting"></param>
		public static void RegisterConnection(ConnectionStringSettings setting)
		{
			if( setting == null )
				throw new ArgumentNullException("setting");

			if( string.IsNullOrEmpty(setting.Name) )
				throw new ArgumentNullException("setting.Name");

			if( string.IsNullOrEmpty(setting.ConnectionString) )
				throw new ArgumentNullException("setting.ConnectionString");


			// 克隆一个对象副本，供内部使用
			ConnectionStringSettings connection = new ConnectionStringSettings(
						setting.Name, setting.ConnectionString, setting.ProviderName);

			if( s_dict.TryAdd(setting.Name, connection) == false )
				throw new InvalidOperationException("不允许注册同名的数据库连接信息：" + setting.Name);
		}

		/// <summary>
		/// 根据名称获取对应的连接信息
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static ConnectionInfo GetConnection(string name = null)
		{
			if( name == null ) {	// 获取默认连接
				ConnectionStringSettings setting = GetDefaultConnection();
				if( setting == null )
					throw new InvalidOperationException("没有注册任何数据库连接，因此不能执行数据库操作");

				return new ConnectionInfo(setting);
			}
			else {
				ConnectionStringSettings setting = null;
				if( s_dict.TryGetValue(name, out setting) == false )
					throw new ArgumentOutOfRangeException("指定的连接名称没有注册：" + name);
				else
					return new ConnectionInfo(setting);
			}
		}


		private static ConnectionStringSettings GetDefaultConnection()
		{
			if( s_defaultConnection == null ) {
				foreach( KeyValuePair<string, ConnectionStringSettings> kvp in s_dict ) {

					// 只取第一个枚举值
					s_defaultConnection = kvp.Value;
					return kvp.Value;
				}
			}
			return s_defaultConnection;
		}
	}
}
