using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ClownFish.Base;
using ClownFish.Base.Xml;

namespace ClownFish.WebHost.Config
{
	/// <summary>
	/// 表示整个服务实例的运行参数
	/// </summary>
	[Serializable]
	public sealed class ServerOption
	{
		/// <summary>
		/// HTTP监听参数
		/// </summary>
		[XmlArray("httpListener")]
		[XmlArrayItem("option")]
		public HttpListenerOption[] HttpListenerOptions { get; set; }

        /// <summary>
        /// 通用配置参数
        /// </summary>
        [XmlArray("appSettings")]
        [XmlArrayItem("add")]
        public AppSetting[] AppSettings { get; set; }


        /// <summary>
        /// 需要加载的模块（类型字符串）
        /// </summary>
        [XmlArray("modules")]
		[XmlArrayItem("type")]
		public string[] Modules { get; set; }


		/// <summary>
		/// 站点参数
		/// </summary>
		[XmlElement("website")]
		public WebsiteOption Website { get; set; }


        internal string[] GetUrlPrefixes()
        {
            return (from x in this.HttpListenerOptions
                    let url = x.ToString()
                    select url
                    ).ToArray();
        }


        /// <summary>
        /// 字符串："ClownFish.WebHost.config"
        /// </summary>
        public static readonly string ConfigFileName = "ClownFish.WebHost.config";

        private static ServerOption s_instance;
        private static readonly object s_lock = new object();


        /// <summary>
        /// 设置默认的静态单例引用
        /// </summary>
        /// <param name="option"></param>
        public static void SetDefault(ServerOption option)
        {
            if( option == null ) {
                s_instance = null;
                return;
            }

            // 验证参数的设置是否完整
            ServerOptionValidator validator = new ServerOptionValidator();
            validator.Validate(option);

            // 克隆对象副本，给一些节点填充默认取，然后给静态变量赋值
            ServerOption option2 = option.CloneObject();
            validator.SetDefaultValues(option2);

            s_instance = option2;
        }


        /// <summary>
        /// 获取默认的静态单例引用
        /// </summary>
        /// <returns></returns>
        public static ServerOption Get()
        {
            if( s_instance == null ) {
                lock( s_lock ) {
                    if( s_instance == null ) {

                        string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFileName);
                        if( File.Exists(configPath) ) {
                            s_instance = LoadFile(configPath);
                        }
                        else {
                            // 如果配置文件不存在，就创建一个 “空” 实例
                            s_instance = new ServerOption();
                        }
                    }
                }
            }

            return s_instance;
        }


        /// <summary>
        /// 从配置文件中加载ServerOption实例
        /// </summary>
        /// <param name="configPath"></param>
        /// <returns></returns>
        public static ServerOption LoadFile(string configPath)
        {
            ServerOption option;
            try {
                option = XmlHelper.XmlDeserializeFromFile<ServerOption>(configPath);
            }
            catch( Exception ex ) {
                throw new ConfigurationErrorsException($"读取配置文件 {configPath} 发生异常，错误原因：{ex.Message}");
            }

            ServerOptionValidator validator = new ServerOptionValidator();
            validator.Validate(option);
            validator.SetDefaultValues(option);

            return option;
        }



    }


}
