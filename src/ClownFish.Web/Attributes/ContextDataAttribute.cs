using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClownFish.Web
{
	/// <summary>
	/// 用于根据指定的属性名表达式直接从HttpContext对象中求值。
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter,AllowMultiple=false, Inherited=false)]
	public sealed class ContextDataAttribute : System.Attribute
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="expression"></param>
		public ContextDataAttribute(string expression)
		{
			if( string.IsNullOrEmpty(expression) )
				throw new ArgumentNullException("expression");

			this.Expression = expression;
		}

		/// <summary>
		/// 用于求值的属性名，也可以是一个表达式。
		/// </summary>
		public string Expression { get; private set; }
	}
}
