using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Base.Framework
{
	/// <summary>
	/// 提供对应用程序启动的扩展支持。
	/// </summary>
	/// <remarks>
	///  说明：
	///  ASP.NET也有一个同名的类型，即：System.Web.PreApplicationStartMethodAttribute
	/// 
	///  如果使用那个类型，在初始化时不能调用（会抛异常） BuildManager.GetReferencedAssemblies(); 
	/// 
	///  这个类型的执行时间也会晚一点，它是由Global.asax的Application_Start调用触发的。
	///  而且它允许执行一个内部方法。
	/// </remarks>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class PreApplicationStartMethodAttribute : Attribute
	{

		/// <summary>
		/// 初始化 PreApplicationStartMethodAttribute 类的新实例。
		/// </summary>
		/// <param name="type">一个描述启动方法的类型的对象。</param><param name="methodName">没有返回值的空参数签名。</param>
		public PreApplicationStartMethodAttribute(Type type, string methodName)
		{
			if( type == null )
				throw new ArgumentNullException("type");

			if( string.IsNullOrEmpty(methodName) )
				throw new ArgumentNullException("methodName");

			Type = type;
			MethodName = methodName;
		}

		/// <summary>
		/// 获取关联启动方法所返回的类型。
		/// </summary>
		/// <returns>
		/// 一个描述启动方法的类型的对象。
		/// </returns>
		public Type Type { get; private set; }

		/// <summary>
		/// 获取关联的启动方法。
		/// </summary>
		/// <returns>
		/// 一个字符串，其中包含关联启动方法的名称。
		/// </returns>
		public string MethodName { get; private set; }
	}
}
