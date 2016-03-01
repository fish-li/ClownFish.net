using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClownFish.Web
{
	/// <summary>
	/// 用于指示不要用Http请求中的内容给一些实体成员赋值。
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	public sealed class HttpValueIgnoreAttribute : System.Attribute
	{
	}
}
