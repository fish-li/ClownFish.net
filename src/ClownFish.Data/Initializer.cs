using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Internals;
using ClownFish.Data.CodeDom;
using ClownFish.Data.Xml;

namespace ClownFish.Data
{
	/// <summary>
	/// 初始化接口封装类
	/// </summary>
	public sealed class Initializer
	{
        private Initializer()
        {
            // 供 PerformanceModule 调用
            InternalBridge.RegisterEventManagerEventSubscriber = EventManagerEventSubscriber.Register;
        }

        /// <summary>
        /// Initializer的实例引用
        /// </summary>
        public static readonly Initializer Instance = new Initializer();

		/// <summary>
		/// 默认的实例列表长度
		/// </summary>
		internal int DefaultEntityListLen { get; private set; } = 50;

		/// <summary>
		/// 是否允许创建一次性的DbContext实例，默认值：false
		/// 如果启用，那么可以不必创建ConnectionScope范围块，则直接操作数据库
		/// </summary>
		internal bool IsAutoCreateOneOffDbContext { get; private set; }


		/// <summary>
		/// 是否允许创建一次性的DbContext实例，默认值：false
		/// 如果启用，那么可以不必创建ConnectionScope范围块，则直接操作数据库
		/// </summary>
		/// <returns></returns>
		public Initializer AllowCreateOneOffDbContext()
		{
			IsAutoCreateOneOffDbContext = true;
			return this;
		}

		/// <summary>
		/// 设置默认的实例列表长度
		/// </summary>
		/// <param name="len"></param>
		/// <returns></returns>
		public Initializer SetDefaultEntityListLen(int len)
		{
			if( len < 1 )
				throw new ArgumentOutOfRangeException("len");

			DefaultEntityListLen = len;
			return this;
		}

		/// <summary>
		/// 初始化连接配置
		/// </summary>
		/// <param name="configFilePath"></param>
		/// <returns></returns>
		public Initializer InitConnection(string configFilePath = null)
		{
			if( string.IsNullOrEmpty(configFilePath) )
				ConnectionManager.Init();
			else
				ConnectionManager.Init(configFilePath);

			return this;
		}

		/// <summary>
		/// 从指定的目录中加载所有 XmlCommand 配置
		/// </summary>
		/// <param name="directoryPath">包含XmlCommand配置文件的目录，如果不指定就表示接受XmlCommand规范的默认目录</param>
		/// <returns></returns>
		public Initializer LoadXmlCommandFromDirectory(string directoryPath = null)
		{
			// 如果不指定目录，就采用XmlCommand规范的默认目录（建议不指定！）
			if( string.IsNullOrEmpty(directoryPath) ) {
				if( ClownFish.Base.Framework.RunTimeEnvironment.IsAspnetApp)
					// 如果是ASP.NET程序，默认的XmlCommand存放目录就是 App_Data\\XmlCommand
					directoryPath = System.IO.Path.Combine(System.Web.HttpRuntime.AppDomainAppPath, "App_Data\\XmlCommand");
				else
					// 非ASP.NET程序，就是当前执行程序下的 XmlCommand 目录
					directoryPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "XmlCommand");
			}
			XmlCommandManager.LoadFromDirectory(directoryPath);
			return this;
		}

		/// <summary>
		/// 加载XML字符串中包含的 XmlCommand
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public Initializer LoadXmlCommandFromText(string xml)
		{
			XmlCommandManager.LoadFromText(xml);
			return this;
		}

		/// <summary>
		/// 自动搜索当前程序加载的所有实体类型，并为它们编译生成代理类型及注册。
		/// </summary>
		/// <param name="useAttrFilter"></param>
		/// <returns></returns>
		public Initializer CompileAllEntityProxy(bool useAttrFilter = false)
		{
			ProxyBuilder.CompileAllEntityProxy(useAttrFilter);
			return this;
		}


	}
}
