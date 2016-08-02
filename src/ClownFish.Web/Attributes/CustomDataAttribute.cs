using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ClownFish.Web
{

	/// <summary>
	/// 允许自定义特定数据类型的参数获取机制，用于在Action调用前获取Action参数值
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple =false)]
	public abstract class CustomDataAttribute : Attribute
	{
		/// <summary>
		/// 根据HttpContext和ParameterInfo获取参数值
		/// </summary>
		/// <param name="context"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		public abstract object GetHttpValue(HttpContext context, ParameterInfo p);
	}
}
