using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ClownFish.Log.Serializer;

namespace ClownFish.Log.Configuration
{
	/// <summary>
	/// 所有Writer的配置基类
	/// </summary>
	public class BaseWriterConfig
	{
		/// <summary>
		/// 对应Writer的名称
		/// </summary>
		[XmlIgnore]
		public string Name { get; set; }


		/// <summary>
		/// 对应Writer的类型，如果不使用，可以不指定
		/// </summary>
		public string WriteType { get; set; }


		/// <summary>
		/// 验证属性是否配置正确
		/// </summary>
		public virtual void Valid()
		{
			
		}

	}
}
