using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Log.Serializer;

namespace ClownFish.Log.Configuration
{
	/// <summary>
	/// 每种数据类型的缓存项
	/// </summary>
	internal class DataTypeCacheItem
	{
		/// <summary>
		/// 数据类型对应的写入器类型
		/// </summary>
		public Type[] WriteTypes { get; set; }

		/// <summary>
		/// 数据类型对应的写入器实例
		/// </summary>
		public ILogWriter[] Instances { get; set; }
	}
}
