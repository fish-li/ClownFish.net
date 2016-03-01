using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClownFish.Web.Reflection
{
	/// <summary>
	/// Controller分类识别器（如果希望修改判断规则，可以继承并重写相关方法。
	/// 注意：这个类型的实例会被静态变量引用，因此要求是线程安全的。
	/// </summary>
	public class ControllerRecognizer
	{
		/// <summary>
		/// 判断指定的类型是否为页面控制器
		/// （使用PageUrlAttribute，PageRegexUrlAttribute定位Action）
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public virtual bool IsPageController(Type t)
		{
			// 扩展点：允许自定义判断PageController的实现方式

			return t.Name.EndsWith("Controller", StringComparison.Ordinal);
		}

		/// <summary>
		/// 判断指定的类型是否为服务控制器
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public virtual bool IsServiceController(Type t)
		{
			// 扩展点：允许自定义判断ServiceController的实现方式

			return t.Name.EndsWith("Service", StringComparison.Ordinal);
		}

		/// <summary>
		/// 确保指定的类型名称是一个Service类型，
		/// 处理逻辑：如果不是以Service结尾（区分大小写），就添加Service，反之则不处理。
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public virtual string EnsureServicePostfix(string typeName)
		{
			// 扩展点：允许自定义判断ServiceController的实现方式

			return typeName.EndsWith("Service", StringComparison.Ordinal) ? typeName : typeName + "Service";
		}


		/// <summary>
		/// 根据UrlActionInfo收集到的命名空间及类名计算ServiceController的全名
		/// 如果希望在URL中使用简短或者缩写的命名空间，可以重写这个方法
		/// </summary>
		/// <param name="info">UrlActionInfo的实例</param>
		/// <returns></returns>
		public virtual string GetServiceFullName(UrlActionInfo info)
		{
			// 扩展点：允许自定义判断ServiceController的实现方式

			return info.Namesapce
				+ (string.IsNullOrEmpty(info.Namesapce) ? string.Empty : ".")
				+ EnsureServicePostfix(info.ClassName);
		}

	}
}
