using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Web.Config
{
	/// <summary>
	/// StaticFileHandler的配置节点
	/// </summary>
	public sealed class StaticFileHandlerSectionElement : ConfigurationElement
	{
		/// <summary>
		/// 各种静态文件的缓存时间，默认值：js:31536000;css:3600;*:31536000，单位：秒
		/// </summary>
		[ConfigurationProperty("cacheDuration", IsRequired = false, DefaultValue = "js:31536000;css:3600;*:31536000")]
		public string CacheDuration
		{
			get { return this["cacheDuration"].ToString(); }
		}

	}
}
