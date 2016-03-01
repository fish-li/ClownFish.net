using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ClownFish.Base.Reflection;

namespace ClownFish.Base.TypeExtend
{
	/// <summary>
	/// 可用于外部事件订阅的基类，继承这个类型后，就可以从其它类型中订阅当前类型的实例事件。
	/// </summary>
	public abstract class BaseEventObject
	{
		/// <summary>
		/// 在实例化的时候绑定事件订阅者
		/// </summary>
		/// <param name="srcType"></param>
		internal void BindSubscribes(Type srcType)
		{
			// 事件源类型可能会被继承，但是扩展是基于事件源的，所以通过参数来传递类型（下面的代码不使用，也不要删除！）
			//Type srcType = this.GetType();

			List<Type> types = ExtenderManager.GetSubscribers(srcType);
			if( types == null )
				return;

			// 这里假设：所有的订阅者类型注册都是在程序启动时执行的，
			// 反之，如果假设不成立，真是在运行过程中再注册订阅者，最终的执行结果也是不是预知的，
			// 所以，为了简化实现过程，也为了减少不必要的性能损耗，这里就不考虑线程同步问题的判断问题（假设集合不会在读取时修改）。


			// 说明：这里不使用 foreach 循环，是因为：从头到尾对一个集合进行枚举在本质上不是一个线程安全的过程。
			//      而使用 for ，对集合仅仅只是读取操作，List<T> 可以同时支持多个阅读器。
			for( int i = 0; i < types.Count; i++ ) {
				Type t = types[i];

				// 这里就不采用 ObjectFactory 来创建了，因为不太可能对扩展类型再扩展。
				object subscriber = t.FastNew();
				MethodInfo method = t.GetMethod("SubscribeEvent",
											BindingFlags.Public | BindingFlags.Instance,
											null, new Type[] { srcType }, null);
				method.FastInvoke(subscriber, this);
			}
		}
	}


	/// <summary>
	/// 用于订阅BaseEventObject派生类型事件的基类，
	/// 如果要订阅BaseEventObject派生类型事件，必须继承此类型，
	/// 为了便于识别，建议继承类型的名称以“EventSubscriber”结尾
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class EventSubscriber<T> where T : BaseEventObject
	{
		/// <summary>
		/// 订阅事件
		/// </summary>
		/// <param name="instance"></param>
		public abstract void SubscribeEvent(T instance);
	}


}
