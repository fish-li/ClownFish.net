using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Log.Configuration;
using ClownFish.Log.Model;

namespace ClownFish.Log.Serializer
{
	/// <summary>
	/// 日志的持久化接口
	/// </summary>
	public interface ILogWriter
	{
		/// <summary>
		/// 第一次触发写日志时的初始化动作，例如：检查数据库连接是否已配置
		/// </summary>
		/// <param name="config"></param>
		void Init(WriterSection config);

		/// <summary>
		/// 写入单条日志信息
		/// </summary>
		/// <typeparam name="T">消息的数据类型</typeparam>
		/// <param name="info">要写入的日志信息</param>
		void Write<T>(T info) where T : BaseInfo;


		/// <summary>
		/// 批量写入日志信息
		/// </summary>
		/// <typeparam name="T">消息的数据类型</typeparam>
		/// <param name="list">要写入的日志信息</param>
		void Write<T>(List<T> list) where T : BaseInfo;

		/// <summary>
		/// 根据日志ID获取单条日志信息
		/// </summary>
		/// <typeparam name="T">消息的数据类型</typeparam>
		/// <param name="guid">MessageGuid</param>
		/// <returns>查询结果</returns>
		T Get<T>(Guid guid) where T : BaseInfo;

		/// <summary>
		/// 根据指定的一段时间获取对应的日志记录
		/// </summary>
		/// <typeparam name="T">消息的数据类型</typeparam>
		/// <param name="t1">开始时间</param>
		/// <param name="t2">结束时间</param>
		/// <returns>查询结果</returns>
		List<T> GetList<T>(DateTime t1, DateTime t2) where T : BaseInfo;


	}
}
