using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ClownFish.Web.Config
{
	/// <summary>
	/// OutputCacheConfig
	/// </summary>
	[XmlRoot("OutputCache")]
	public class OutputCacheConfig
	{
		/// <summary>
		/// OutputCacheSetting节点列表
		/// </summary>
		[XmlArrayItem("Setting")]
		public List<OutputCacheSetting> Settings = new List<OutputCacheSetting>();
	}

	/// <summary>
	/// OutputCacheSetting
	/// </summary>
	public class OutputCacheSetting : OutputCacheAttribute
	{
		/// <summary>
		/// 需要缓存的文件路径
		/// </summary>
		[XmlAttribute]
		public string FilePath { get; set; }
	}

}
