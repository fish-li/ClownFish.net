using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ClownFish.Web.Serializer
{
	/// <summary>
	/// 一种从HTTP上下文中加载Action参数的接口
	/// </summary>
	public interface ILoadActionParameter
	{
		/// <summary>
		/// 根据HttpContext和参数信息，加载参数值
		/// </summary>
		/// <param name="context"></param>
		/// <param name="p"></param>
		void GetParameterValue(HttpContext context, ParameterInfo p);
	}
}
