using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ClownFish.Base.Http;

namespace ClownFish.Data
{
	/// <summary>
	/// 用于指示ACTION参数是一个实体代理类型
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
	public sealed class EntityProxyAttribute : CustomDataAttribute
	{
		/// <summary>
		/// 根据HttpContext和ParameterInfo获取参数值
		/// </summary>
		/// <param name="context"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		public override object GetHttpValue(HttpContext context, ParameterInfo p)
		{
			return EntityHttpLoader.LoadFromHttp(context, p);
		}
	}
}
