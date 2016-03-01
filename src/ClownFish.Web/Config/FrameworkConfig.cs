using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using ClownFish.Base.Files;

namespace ClownFish.Web.Config
{
	/// <summary>
	/// 表示ClownFish.Web的配置信息类型
	/// </summary>
	public sealed class FrameworkConfig : ConfigurationSection
	{
		/// <summary>
		/// Pipeline相关的配置节
		/// </summary>
		[ConfigurationProperty("pipeline", IsRequired = false)]
		public PipelineSectionElement Pipeline
		{
			get { return (PipelineSectionElement)this["pipeline"]; }
		}

		/// <summary>
		/// Action相关的配置节
		/// </summary>
		[ConfigurationProperty("action", IsRequired = false)]
		public ActionSectionElement Action
		{
			get { return (ActionSectionElement)this["action"]; }
		}


		/// <summary>
		/// staticFileHandler相关的配置节
		/// </summary>
		[ConfigurationProperty("staticFileHandler", IsRequired = false)]
		public StaticFileHandlerSectionElement StaticFileHandler
		{
			get { return (StaticFileHandlerSectionElement)this["staticFileHandler"]; }
		}
		

		private static FileDependencyManager<FrameworkConfig>
					s_instance = new FileDependencyManager<FrameworkConfig>(		// 基于文件修改通知的缓存实例
							LoadFromConfigFile,		// 读取文件的回调委托
							WebRuntime.Instance.GetPhysicalPath("ClownFish.Web.config"));	// 获取配置文件的路径


		/// <summary>
		/// ClownFish.WebConfiguration实例的引用（已缓存对象，具有文件更新后自动刷新功能）
		/// </summary>
		/// <returns></returns>
		public static FrameworkConfig Instance
		{
			get { return s_instance.Result; }
		}

		private static FrameworkConfig LoadFromConfigFile(string[] files)
		{
			ExeConfigurationFileMap filemap = new ExeConfigurationFileMap();
			filemap.ExeConfigFilename = files[0];

			Configuration config = ConfigurationManager.OpenMappedExeConfiguration(filemap, ConfigurationUserLevel.None);

			return (FrameworkConfig)(config.GetSection("FrameworkConfig")) ?? new FrameworkConfig();
		}
	}


	

	
}
