using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ClownFish.Log.Configuration
{
	/// <summary>
	/// MSMQ的配置信息
	/// </summary>
	public sealed class MsmqWriterConfig : BaseWriterConfig
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public MsmqWriterConfig()
		{
			this.Name = "Msmq";
		}


		/// <summary>
		/// MSMQ路径
		/// </summary>
		[XmlAttribute]
		public string RootPath { get; set; }

		/// <summary>
		/// 验证属性是否配置正确
		/// </summary>
		public override void Valid()
		{
			if( string.IsNullOrEmpty(RootPath) )
				throw new LogConfigException("日志配置文件中，没有为MsmqWriter指定RootPath属性。");

		}

	}

}
