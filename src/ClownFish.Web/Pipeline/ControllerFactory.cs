using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Web
{
	/// <summary>
	/// Controller的创建工厂
	/// </summary>
	public sealed class ControllerFactory
	{
		private static IControllerResolver s_controllerResolver = new DefaultControllerResolver();

		/// <summary>
		/// 设置IControllerResolver的实例，允许在框架外部控制Controller的实例化过程。
		/// </summary>
		/// <param name="controllerResolver"></param>
		public static void SetResolver(IControllerResolver controllerResolver)
		{
			if( controllerResolver == null )
				throw new ArgumentNullException("controllerResolver");

			// 这个方法通常会在程序初始化时调用，所以暂不考虑线程安全问题。
			s_controllerResolver = controllerResolver;
		}


		internal static object GetController(Type controllerType)
		{
			return s_controllerResolver.GetController(controllerType);
		}
	}
}
