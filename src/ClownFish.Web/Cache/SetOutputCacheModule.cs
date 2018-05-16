using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using ClownFish.Base;
using ClownFish.Base.Files;
using ClownFish.Base.Xml;
using ClownFish.Web.Config;

namespace ClownFish.Web
{
	/// <summary>
	/// SetOutputCacheModule
	/// </summary>
	public sealed class SetOutputCacheModule : IHttpModule
	{

		private static FileDependencyManager<Dictionary<string, OutputCacheSetting>>
					s_settings = new FileDependencyManager<Dictionary<string, OutputCacheSetting>>(		// 基于文件修改通知的缓存实例
							LoadConfigFile,		// 读取文件的回调委托
							WebRuntime.Instance.GetPhysicalPath("ClownFish.Web.OutputCache.config"));

		private static Dictionary<string, OutputCacheSetting> LoadConfigFile(string[] files)
		{
			string configFilePath = files[0];
			if( RetryFile.Exists(configFilePath) == false )
				throw new FileNotFoundException("未能找到文件：" + configFilePath + " ，如果要启用 SetOutputCacheModule，必须配置这个文件。");

			OutputCacheConfig config = XmlHelper.XmlDeserializeFromFile<OutputCacheConfig>(configFilePath);
			return config.Settings.ToDictionary(x => x.FilePath, StringComparer.OrdinalIgnoreCase);
		}


		/// <summary>
		/// Init
		/// </summary>
		/// <param name="app"></param>
		public void Init(HttpApplication app)
		{
			app.PreRequestHandlerExecute += new EventHandler(app_PreRequestHandlerExecute);
		}

		void app_PreRequestHandlerExecute(object sender, EventArgs e)
		{
			HttpApplication app = (HttpApplication)sender;

			Dictionary<string, OutputCacheSetting> settings = s_settings.Result;
			if( settings == null )
				throw new ConfigurationErrorsException("SetOutputCacheModule加载配置文件失败。");

			// 实现方法：
			// 查找配置参数，如果找到匹配的请求，就设置OutputCache
			OutputCacheSetting setting = null;
			if( settings.TryGetValue(app.Request.FilePath, out setting) ) {
				setting.SetResponseCache(app.Context);
			}
		}

		/// <summary>
		/// Dispose
		/// </summary>
		public void Dispose()
		{
		}
	}
}
