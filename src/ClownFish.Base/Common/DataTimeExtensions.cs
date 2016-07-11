using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Base
{
	/// <summary>
	/// 包含DataTime类型相关的扩展方法
	/// </summary>
	public static class DataTimeExtensions
	{
		/// <summary>
		/// 返回包含日期时间格式的字符串（"yyyy-MM-dd HH:mm:ss"）
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns></returns>
		public static string ToTimeString(this DateTime datetime)
		{
			return datetime.ToString("yyyy-MM-dd HH:mm:ss");
		}


		/// <summary>
		/// 返回仅仅包含日期格式的字符串（"yyyy-MM-dd"）
		/// </summary>
		/// <param name="datetime"></param>
		/// <returns></returns>
		public static string ToDateString(this DateTime datetime)
		{
			return datetime.ToString("yyyy-MM-dd");
		}
	}
}
