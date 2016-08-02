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
		// 目前有2个调用点：
		// 1、BaseDataProvider，用于获取 Action 参数值
		// 2、ModelBuilder，由于 IHttpDataConvert 不接收ParameterInfo，所以还允许在ModelBuilder中使用


		/// <summary>
		/// 执行转换过程，并返回已加载的对象
		/// </summary>
		/// <param name="context">HttpContext实例</param>
		/// <param name="paraName">参数名称</param>
		/// <returns></returns>
		object Convert(HttpContext context, string paraName);
	}

}
