using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace ClownFish.WebHost.Config
{
    /// <summary>
    /// 站点参数
    /// </summary>
    [Serializable]
	public sealed class WebsiteOption
	{
		/// <summary>
		/// 站点的本地路径
		/// </summary>
		[XmlElement("localPath")]
		public string LocalPath { get; set; }

        /// <summary>
        /// 静态文件的默认缓存时间。
        /// 如果配置值小于等于零，则表示缓存时间为一年。
        /// </summary>
        [DefaultValue(0)]
        public int CacheDuration { get; set; }

        /// <summary>
        /// 静态文件参数
        /// </summary>
        [XmlArray("staticFile")]
		[XmlArrayItem("option")]
		public StaticFileOption[] StaticFiles { get; set; }

        /// <summary>
        /// 目录浏览相关参数
        /// </summary>
        [XmlElement("directoryBrowse")]
        public DirectoryBrowseOption DirectoryBrowse { get; set; }

    }
}
