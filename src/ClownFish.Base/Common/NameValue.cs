using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Base
{
	/// <summary>
	/// 表示一组用于HTTP传输的 【名称/值】 对。
	/// </summary>
	public sealed class NameValue
	{
		/// <summary>
		/// 键名
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 键值
		/// </summary>
		public string Value { get; set; }

		/// <summary>
		/// ToString
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("{0}={1}", this.Name, this.Value);
		}
	}
}
