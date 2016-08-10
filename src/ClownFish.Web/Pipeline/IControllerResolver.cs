using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Reflection;
using ClownFish.Base.TypeExtend;

namespace ClownFish.Web
{
	/// <summary>
	/// Controller的构造接口
	/// </summary>
	public interface IControllerResolver
	{
		/// <summary>
		/// 根据指定的Controller类型获取对应的Controller实例
		/// </summary>
		/// <param name="controllerType"></param>
		/// <returns></returns>
		object GetController(Type controllerType);
	}


	internal class DefaultControllerResolver : IControllerResolver
	{
		private DefaultObjectResolver _defaultObjectResolver = new DefaultObjectResolver();

		#region IControllerResolver 成员

		public object GetController(Type controllerType)
		{
			// 调用 ClownFish.Base 中的默认实现
			return _defaultObjectResolver.CreateObject(controllerType);
		}

		#endregion
	}
}
