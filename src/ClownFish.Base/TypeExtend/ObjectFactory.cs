using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ClownFish.Base.TypeExtend
{
	/// <summary>
	/// 创建扩展对象的工厂类型
	/// </summary>
	public static class ObjectFactory
	{
		private static IObjectResolver s_objectResolver = new DefaultObjectResolver();

		/// <summary>
		/// 设置IObjectResolver的实例，允许在框架外部控制对象的实例化过程。
		/// </summary>
		/// <param name="objectResolver"></param>
		public static void SetResolver(IObjectResolver objectResolver)
		{
			if( objectResolver == null )
				throw new ArgumentNullException("objectResolver");

			// 这个方法通常会在程序初始化时调用，所以暂不考虑线程安全问题。
			s_objectResolver = objectResolver;
		}



		/// <summary>
		/// 尝试创建指定类型的扩展类，并尝试加载事件订阅者
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T New<T>() where T : class, new()
		{
			return (T)New(typeof(T));
		}


		/// <summary>
		/// 尝试创建指定类型的扩展类
		/// </summary>
		/// <param name="objectType"></param>
		/// <returns></returns>
		public static object New(Type objectType)
		{
			if( objectType == null )
				throw new ArgumentNullException("objectType");

			return s_objectResolver.CreateObject(objectType);
		}

	}
}
