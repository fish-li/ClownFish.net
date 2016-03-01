using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ClownFish.Log.Configuration
{
	/// <summary>
	/// 表示需要写入日志的数据类型配置
	/// </summary>
	public sealed class TypeItemConfig
	{
		/// <summary>
		/// 要记录到日志的数据类型名称
		/// </summary>
		[XmlAttribute]
		public string DataType { get; set; }


		/// <summary>
		/// 数据类型对应的写入器，允许指定多个名称，用【分号】分开
		/// </summary>
		[XmlAttribute]
		public string Writers { get; set; }


		/// <summary>
		/// DataType 对应的 Type 实例
		/// </summary>
		[XmlIgnore]
		internal Type Type { get; set; }
		
	}
}
