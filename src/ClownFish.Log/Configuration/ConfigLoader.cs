using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Log.Serializer;

namespace ClownFish.Log.Configuration
{

	internal class ConfigLoader
	{
		private LogConfig _config;
		private Hashtable _writersTable;
		private Dictionary<string, string> _writerNameTypeDict = new Dictionary<string, string>();


		internal void Load(LogConfig config, Hashtable writersTable)
		{
			_config = config;
			_writersTable = writersTable;


			// 检查配置参数是否正确
			ChceckDataTypeConfig();

			// 获取所有的写入器类型，并填充 _writerNameTypeDict
			LoadWritersType();


			// 只有当配置是有效的时候，才做进一步操作
			if( _config.Enable ) {
				// 设置写入异常时的【重试】写入器
				SetExceptionWriter();

				// 设置每种数据类型对应的缓存项，填充到 writersTable
				SetDataTypeCache();
			}
		}


		private void ChceckDataTypeConfig()
		{
			if( _config.Types == null || _config.Types.Length == 0 )
				throw new LogConfigException("日志配置文件中，没有配置Types节点。");


			// 检查无效的数据类型定义
			foreach( var item in _config.Types ) {
				if( string.IsNullOrEmpty(item.DataType) )
					throw new LogConfigException("日志配置文件中，不允许DataType属性为空。");

				if( item.Writers == null || item.Writers.Trim(';', ',').Length == 0 )
					throw new LogConfigException("日志配置文件中，不允许Writers属性为空。");

				// 如果指定的类型不正确，下面代码会抛出异常
				item.Type = Type.GetType(item.DataType, true);
			}
		}


		private void LoadWritersType()
		{
			if( _config.Writers == null || _config.Writers.Length == 0 )
				throw new LogConfigException("日志配置文件中，没有配置Writers节点。");


			foreach( var w in _config.Writers ) {
				if( string.IsNullOrEmpty(w.Name) )
					throw new LogConfigException("日志配置文件中，写入器的 Name 属性不能为空。");

				if( string.IsNullOrEmpty(w.Type) )
					throw new LogConfigException("日志配置文件中，写入器的 Type 属性不能为空。");

				// 如果指定的类型不正确，下面代码会抛出异常
				Type t = Type.GetType(w.Type, true);

				if( typeof(ILogWriter).IsAssignableFrom(t) == false )
					throw new LogConfigException("日志配置文件中，指定的WriteType没有实现接口ILogWriter：" + w.Type);

				ILogWriter instance = Activator.CreateInstance(t) as ILogWriter;
				instance.Init(w);

				_writerNameTypeDict[w.Name] = w.Type;
			}

			// 增加一个内置的，可用于开发环境中不写日志
			_writerNameTypeDict["NULL"] = "ClownFish.Log.Serializer.NullWriter, ClownFish.Log";
		}


		private void SetExceptionWriter()
		{
			if( string.IsNullOrEmpty(_config.ExceptionWriter) == false ) {
				string writerTypeName = null;
				if( _writerNameTypeDict.TryGetValue(_config.ExceptionWriter, out writerTypeName) )
					// 保存写日志发生异常的Writer，单个实例
					try {
						Type t = Type.GetType(writerTypeName, true);
						ILogWriter instance = (ILogWriter)Activator.CreateInstance(t);  // 测试类型是否符合要求
						_writersTable[WriterFactory.ExceptionWriterKey] = t;
					}
					catch( Exception ex ) {
						throw new LogConfigException("日志配置文件中，指定的Writer无效：" + writerTypeName, ex);
					}
				else
					throw new LogConfigException("日志配置文件中，指定的Writer无效：" + _config.ExceptionWriter);
			}
		}


		private void SetDataTypeCache()
		{
			foreach( var item in _config.Types ) {
				string[] writers = item.Writers.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
				List<Type> typeList = new List<Type>(writers.Length);

				for( int i = 0; i < writers.Length; i++ ) {
					string value = null;
					if( _writerNameTypeDict.TryGetValue(writers[i].Trim(), out value) ) {
						try {
							typeList.Add(Type.GetType(value, true));        // 如果类型名称不正确，这里会出现异常
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
					DataTypeCacheItem cacheItem = new DataTypeCacheItem();
					cacheItem.WriteTypes = typeList.ToArray();
					cacheItem.Instances = (from x in typeList
										   select (ILogWriter)Activator.CreateInstance(x)   // 如果配置有错，会出现异常！
											   ).ToArray();

					Type t = Type.GetType(item.DataType, true);
					_writersTable[t.FullName] = cacheItem;
				}
				catch( Exception ex ) {
					throw new LogConfigException("日志配置文件中，指定的Writer无效：" + item.DataType, ex);
				}
			}
		}


	}
}
