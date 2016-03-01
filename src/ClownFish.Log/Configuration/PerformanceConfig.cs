using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ClownFish.Log.Configuration
{
	/// <summary>
	/// 配置日志的配置信息
	/// </summary>
	public sealed class PerformanceConfig
	{
		/// <summary>
		/// 数据库执行的阀值时间，单位：毫秒
		/// </summary>
		[XmlAttribute]
		public int DbExecuteTimeout { get; set; }


		/// <summary>
		/// HTTP请求执行的阀值时间，单位：毫秒
		/// </summary>
		[XmlAttribute]
		public int HttpExecuteTimeout { get; set; }
	}
}
