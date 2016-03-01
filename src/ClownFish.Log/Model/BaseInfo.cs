using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ClownFish.Base.Xml;
using MongoDB.Bson.Serialization.Attributes;

namespace ClownFish.Log.Model
{
	/// <summary>
	/// 日志信息的数据结构基类
	/// </summary>
	[Serializable]
	public abstract class BaseInfo
	{
		/// <summary>
		/// 日志信息GUID，用于从数据库查询单条消息
		/// </summary>
#if _MongoDB_
		[BsonId]
#endif		
		[XmlElement(Order = 1)]
		public Guid InfoGuid { get; set; }

		/// <summary>
		/// 消息的创建时间
		/// </summary>
#if _MongoDB_
		[BsonDateTimeOptions(Kind = DateTimeKind.Local)]
#endif
		[XmlElement(Order = 2)]
		public DateTime Time { get; set; }

		/// <summary>
		/// 消息文本
		/// </summary>
		[XmlElement(Order = 3)]
		public XmlCdata Message { get; set; }

		/// <summary>
		/// 服务器名
		/// </summary>
		[XmlElement(Order = 4)]
		public string HostName { get; set; }


		/// <summary>
		/// 填充一些基础信息：InfoGuid，Time，HostName
		/// </summary>
		public void FillBaseInfo()
		{
			this.InfoGuid = Guid.NewGuid();
			this.Time = DateTime.Now;

			try {
				this.HostName = System.Environment.MachineName;
			}
			catch { /* 这里出异常，只能忽略了  */
				this.HostName = "#######";
			}
		}

		
	}
}
