using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using ClownFish.Base.Framework;
using ClownFish.Base.Xml;

namespace ClownFish.Data.Xml
{
	/// <summary>
	/// 用于维护配置文件中数据库访问命令的管理类
	/// </summary>
	public static class XmlCommandManager
	{
		private static readonly string s_CacheKey = Guid.NewGuid().ToString();
		private static Exception s_ExceptionOnLoad = null;

		private static Dictionary<string, XmlCommandItem> s_dict = null;
		private static Dictionary<string, XmlCommandItem> s_dictXml = new Dictionary<string, XmlCommandItem>();


		/// <summary>
		/// 从指定的Xml字符串加载XmlCommand（例如将XmlCommand文件做为嵌入程序集资源）
		/// </summary>
		/// <param name="xml">xml字符串</param>
		public static void LoadFromText(string xml)
		{
			if( string.IsNullOrEmpty(xml) )
				throw new ArgumentNullException("xml");

			List<XmlCommandItem> list = XmlHelper.XmlDeserialize<List<XmlCommandItem>>(xml);
			list.ForEach(x => s_dictXml.AddValue(x.CommandName, x));
		}


		/// <summary>
		/// <para>从指定的目录中加载全部的用于数据访问命令。</para>
		/// <para>说明：1. 这个方法只需要在程序初始化调用一次就够了。</para>
		/// <para>       2. 如果是一个ASP.NET程序，CommandManager还会负责监视此目录，如果文件有更新，会自动重新加载。</para>
		/// </summary>
		/// <param name="directoryPath">包含数据访问命令的目录。不加载子目录，仅加载扩展名为 .config 的文件。</param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public static void LoadFromDirectory(string directoryPath)
		{
			if( s_dict != null && RunTimeEnvironment.IsAspnetApp ) // 不要删除这个判断检查，因为后面会监视这个目录。
				throw new InvalidOperationException("不允许重复调用这个方法。");

			if( Directory.Exists(directoryPath) == false )
				throw new DirectoryNotFoundException(string.Format("目录 {0} 不存在。",
					directoryPath));

			Exception exception = null;
			s_dict = LoadFromDirectoryInternal(directoryPath, out exception);

			if( exception != null )
				s_ExceptionOnLoad = exception;

			if( s_ExceptionOnLoad != null )
				throw s_ExceptionOnLoad;
		}


		
		private static Dictionary<string, XmlCommandItem> LoadFromDirectoryInternal(string directoryPath, out Exception exception)
		{
			exception = null;
			Dictionary<string, XmlCommandItem> dict = null;

			try {
				string[] files = Directory.GetFiles(directoryPath, "*.config", SearchOption.AllDirectories);
				if( files.Length > 0 ) {
					dict = new Dictionary<string, XmlCommandItem>(1024 * 2);

					foreach( string file in files ) {
						List<XmlCommandItem> list = XmlHelper.XmlDeserializeFromFile<List<XmlCommandItem>>(file);
						list.ForEach(x => dict.AddValue(x.CommandName, x));
					}
				}
			}
			catch( Exception ex ) {
				exception = ex;
				dict = null;
			}

			if( RunTimeEnvironment.IsAspnetApp ) {
				// 如果程序运行在ASP.NET环境中，
				// 注册缓存移除通知，以便在用户修改了配置文件后自动重新加载。

				// 参考：细说 ASP.NET Cache 及其高级用法
				//	      http://www.cnblogs.com/fish-li/archive/2011/12/27/2304063.html
				CacheDependency dep = new CacheDependency(directoryPath);
				HttpRuntime.Cache.Insert(s_CacheKey, directoryPath, dep,
					System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration,
					CacheItemPriority.NotRemovable, CacheRemovedCallback);
			}
			return dict;
		}

		private static void CacheRemovedCallback(string key, object value, CacheItemRemovedReason reason)
		{
			Exception exception = null;
			string directoryPath = (string)value;

			for( int i = 0; i < 5; i++ ) {
				// 由于事件发生时，文件可能还没有完全关闭，所以只好让程序稍等。
				System.Threading.Thread.Sleep(3000);


				// 重新加载配置文件
				Dictionary<string, XmlCommandItem> dict = LoadFromDirectoryInternal(directoryPath, out exception);

				if( exception == null ) {
					try {
					}
					finally {
						s_dict = dict;
						s_ExceptionOnLoad = null;
					}
					return;
				}
				//else: 有可能是文件还在更新，此时加载了不完整的文件内容，最终会导致反序列化失败。
			}

			if( exception != null )
				s_ExceptionOnLoad = exception;
		}



		/// <summary>
		/// 根据配置文件中的命令名获取对应的命令对象。
		/// </summary>
		/// <param name="name">命令名称，它应该能对应一个XmlCommand</param>
		/// <returns>如果找到符合名称的XmlCommand，则返回它，否则返回null</returns>
		public static XmlCommandItem GetCommand(string name)
		{
			if( s_ExceptionOnLoad != null )
				throw s_ExceptionOnLoad;

			XmlCommandItem command;

			// 优先加载文件中指定的XmlCommand，这样可以实现文件对程序集资源的覆盖

			if( s_dict != null ) {
				if( s_dict.TryGetValue(name, out command) )
					return command;
			}

			if( s_dictXml != null ) {
				if( s_dictXml.TryGetValue(name, out command) ) {
					return command;
				}
			}

			//throw new ArgumentOutOfRangeException("name", "不能根据指定的名称找到匹配的XmlCommand，name: " + name);
			return null;
		}


	}
}
