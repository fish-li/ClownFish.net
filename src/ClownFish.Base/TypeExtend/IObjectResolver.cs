using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClownFish.Base.Reflection;

namespace ClownFish.Base.TypeExtend
{
	/// <summary>
	/// 对象的构造接口
	/// </summary>
	public interface IObjectResolver
	{
		/// <summary>
		/// 根据指定的类型获取对应的实例
		/// </summary>
		/// <param name="objectType"></param>
		/// <returns></returns>
		object CreateObject(Type objectType);
	}


	/// <summary>
	/// 框架内部默认的IObjectResolver实现类，用于根据指定类型创建对象实例。
	/// 这个实现类中，主要处理了二个扩展特性：类型扩展和事件订阅。
	/// </summary>
	public sealed class DefaultObjectResolver : IObjectResolver
	{
		/// <summary>
		/// 根据类型创建实例，
		/// 注意：将要实例化的类型必须有【无参的构造函数】。
		/// </summary>
		/// <param name="objectType"></param>
		/// <returns></returns>
		public object CreateObject(Type objectType)
		{
			if( objectType == null )
				throw new ArgumentNullException("objectType");

			object instance = null;

			// 封闭类型无法扩展，直接使用当前类型（不支持有参的构造函数）
			if( objectType.IsSealed ) {
				instance = objectType.FastNew();
			}
			else {
				// 尝试获取扩展类型
				Type extType = ExtenderManager.GetExtendType(objectType);

				instance = (extType == null)
								? objectType.FastNew()  // 没有扩展类型，就直接使用原类型（不支持有参的构造函数）
								: extType.FastNew();    // 有扩展类型就创建扩展类型的实例
			}

			// 尝试加载事件订阅者
			BaseEventObject baseEventObject = instance as BaseEventObject;
			if( baseEventObject != null )
				baseEventObject.BindSubscribes(objectType);

			return instance;
		}
	}
}
