using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using ClownFish.Web.Reflection;

namespace ClownFish.Web.Serializer
{
	/// <summary>
	/// 定义用于构造Action传入参数的提供者接口
	/// </summary>
	public interface IActionParametersProvider
	{
		/// <summary>
		/// 根据指定的Action方法实例，从HTTP请求中获取调用参数。
		/// </summary>
		/// <param name="context">HttpContext实例</param>
		/// <param name="method">Action方法实例</param>
		/// <returns></returns>
		object[] GetParameters(HttpContext context, System.Reflection.MethodInfo method);

	}


	// 内部版本，避免公开过多的数据类型，增加学习成本。
	internal interface IActionParametersProvider2 : IActionParametersProvider
	{		
		object[] GetParameters(HttpContext context, ActionDescription action);
	}
}
