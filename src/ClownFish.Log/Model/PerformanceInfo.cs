using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace ClownFish.Log.Model
{

	/// <summary>
	/// 性能日志相关信息
	/// </summary>
#if _MongoDB_
	[BsonIgnoreExtraElements]
#endif		
	public class PerformanceInfo : BaseInfo
	{
		/// <summary>
		/// HTTP请求相关信息
		/// </summary>
		[XmlElement(Order = 1)]
		public HttpInfo HttpInfo { get; set; }


		/// <summary>
		/// 业务相关信息
		/// </summary>
		[XmlElement(Order = 2)]
		public BusinessInfo BusinessInfo { get; set; }


		/// <summary>
		/// 性能日志的记录类型：HTTP， SQL
		/// </summary>
		[XmlElement(Order = 3)]
		public string PerformanceType { get; set; }


		/// <summary>
		/// 执行时间
		/// </summary>
		[XmlElement(Order = 4)]
		public TimeSpan ExecuteTime { get; set; }


		/// <summary>
		/// SQL相关的执行信息
		/// </summary>
		[XmlElement(Order = 5)]
		public SqlInfo SqlInfo { get; set; }



		/// <summary>
		/// 根据HttpContext创建并填充PerformanceInfo对象
		/// </summary>
		/// <param name="context"></param>
		/// <param name="message"></param>
		/// <param name="executeTime"></param>
		/// <returns></returns>
		public static PerformanceInfo CreateByHttp(HttpContext context, string message, TimeSpan executeTime)
		{
			PerformanceInfo info = new PerformanceInfo();
			info.FillBaseInfo();
			info.HttpInfo = HttpInfo.Create(context);

			info.PerformanceType = "HTTP";
			info.ExecuteTime = executeTime;
			info.Message = message;
			return info;
		}


		/// <summary>
		/// 根据DbCommand创建并填充PerformanceInfo对象
		/// </summary>
		/// <param name="command"></param>
		/// <param name="message"></param>
		/// <param name="executeTime"></param>
		/// <returns></returns>
		public static PerformanceInfo CreateBySql(DbCommand command, string message, TimeSpan executeTime)
		{
			PerformanceInfo info = new PerformanceInfo();
			info.FillBaseInfo();
			info.SqlInfo = SqlInfo.Create(command);

			info.PerformanceType = "SQL";
			info.ExecuteTime = executeTime;
			info.Message = message;
			return info;
		}

	}
}
