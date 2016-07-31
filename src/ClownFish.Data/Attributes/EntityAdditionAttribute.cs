using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Data
{
	/// <summary>
	/// 实体的附加描述标记，仅供框架内部使用。
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple =false)]
	public sealed class EntityAdditionAttribute : Attribute
	{
		/// <summary>
		/// 相关联的实体代理类型
		/// </summary>
		public Type ProxyType { get; set; }

	}
}
