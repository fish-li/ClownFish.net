using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Log
{
	/// <summary>
	/// 指定在记录日志时使用的类型过滤器
	/// </summary>
	public class LogFilter
	{
		/// <summary>
		/// 是否不记录某个对象
		/// 默认行为：记录任何对象（不忽略），
		/// 可重写行为：检查是否为特定类型对象，返回true表示不记录。
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public virtual bool IgnoreWrite(object obj)
		{
			return false;
		}
	}
}
