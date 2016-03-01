using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ClownFish.Log.Configuration
{
	/// <summary>
	/// 日志的配置数据结构
	/// </summary>
	public sealed class LogConfig
	{
		/// <summary>
		/// 是否启用
		/// </summary>
		public bool Enable { get; set; }

		/// <summary>
		/// 定时刷新间隔
		/// </summary>
		public int TimerPeriod { get; set; }

		/// <summary>
		/// 所有Writer的配置集合
		/// </summary>
		public WritersConfig Writers { get; set; }

		/// <summary>
		/// 所有要写入的数据类型集合
		/// </summary>
		[XmlArrayItem("Type")]
		public List<TypeItemConfig> Types { get; set; }

		/// <summary>
		/// 性能日志的配置信息
		/// </summary>
		public PerformanceConfig Performance { get; set; }


		/// <summary>
		/// 写日志失败时，异常消息的写入器
		/// </summary>
		public string ExceptionWriter { get; set; }

	}


	


	

	


	

	



}
