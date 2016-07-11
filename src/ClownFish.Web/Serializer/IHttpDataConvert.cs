using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ClownFish.Web.Serializer
{
	/// <summary>
	/// HTTP数据转换器接口，用于实现从HttpContext中加载特定的类型数据
	/// </summary>
	public interface IHttpDataConvert
	{
		/// <summary>
		/// 执行转换过程，并返回已加载的对象
		/// </summary>
		/// <param name="context">HttpContext实例</param>
		/// <param name="paraName">参数名称</param>
		/// <returns></returns>
		object Convert(HttpContext context, string paraName);
	}

}
