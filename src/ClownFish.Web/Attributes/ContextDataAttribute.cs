using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using ClownFish.Base.Http;
using ClownFish.Base.Reflection;
using ClownFish.Web.Reflection;

namespace ClownFish.Web
{
	/// <summary>
	/// 用于根据指定的属性名表达式直接从HttpContext对象中求值。
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple=false)]
	public sealed class ContextDataAttribute : CustomDataAttribute
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


		/// <summary>
		/// 根据HttpContext和ParameterInfo获取参数值
		/// </summary>
		/// <param name="context"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		public override object GetHttpValue(HttpContext context, ParameterInfo p)
		{
			// 直接从HttpRequest对象中获取数据，根据Attribute中指定的表达式求值。
			string expression = this.Expression;
			object requestData = null;

			if( expression.StartsWith("Request.") )
				requestData = System.Web.UI.DataBinder.Eval(context.Request, expression.Substring(8));

			else if( expression.StartsWith("HttpRuntime.") ) {
				PropertyInfo property = typeof(HttpRuntime).GetProperty(expression.Substring(12), BindingFlags.Static | BindingFlags.Public);
				if( property == null )
					throw new ArgumentException(string.Format("参数 {0} 对应的ContextDataAttribute计算表达式 {1} 无效：", p.Name, expression));
				requestData = property.FastGetValue(null);
			}
			else
				requestData = System.Web.UI.DataBinder.Eval(context, expression);


			if( requestData == null )
				return null;
			else {
				if( requestData.GetType().IsCompatible(p.ParameterType) )
					return requestData;
				else
					throw new ArgumentException(string.Format("参数 {0} 的申明的类型与HttpRequest对应属性的类型不一致。", p.Name));
			}
		}
	}
}
