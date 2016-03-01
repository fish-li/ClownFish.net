using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ClownFish.Log.Configuration
{
	/// <summary>
	/// Windows日志的配置信息
	/// </summary>
	public sealed class WinLogWriterConfig : BaseWriterConfig
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public WinLogWriterConfig()
		{
			this.Name = "WinLog";
		}

		/// <summary>
		/// 日志文件名
		/// </summary>
		[XmlAttribute]
		public string LogName { get; set; }


		/// <summary>
		/// 日志源名称
		/// </summary>
		[XmlAttribute]
		public string SourceName { get; set; }

		/// <summary>
		/// 验证属性是否配置正确
		/// </summary>
		public override void Valid()
		{
			if( string.IsNullOrEmpty(LogName) )
				throw new LogConfigException("在配置文件中没有为WinLogWriter指定LogName属性。");

			if( string.IsNullOrEmpty(SourceName) )
				throw new LogConfigException("在配置文件中没有为WinLogWriter指定SourceName属性。");
		}

	}

}
