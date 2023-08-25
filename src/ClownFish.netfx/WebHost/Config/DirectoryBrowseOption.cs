using System;
using System.Xml.Serialization;

namespace ClownFish.WebHost.Config
{
    /// <summary>
    /// 目录浏览相关参数
    /// </summary>
    [Serializable]
    public sealed class DirectoryBrowseOption
    {
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("enabled")]
        public int Enabled { get; set; }

        /// <summary>
        /// 默认文件名，例如：index.html，
        /// 如果有多个文件名，用分号分开。
        /// </summary>
        [XmlElement("defaultFile")]
        public string DefaultFile { get; set; }
    }
}
