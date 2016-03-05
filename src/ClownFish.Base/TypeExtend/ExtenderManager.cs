using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using ClownFish.Base.Common;
using ClownFish.Base.Reflection;

namespace ClownFish.Base.TypeExtend
{
	/// <summary>
	/// 用于管理扩展类型的工具类
	/// </summary>
	public static class ExtenderManager
	{
		#region 类型继承管理

		/// <summary>
		/// 类型与继承类型的映射字典
		/// </summary>
		private static readonly Hashtable s_typeMapDict = Hashtable.Synchronized(new Hashtable(256));

		/// <summary>
		/// 获取指定类型的扩展类（继承类），
		/// 如果没有找到匹配的类型，就返回NULL
		/// </summary>
		/// <param name="srcType"></param>
		/// <returns></returns>
		internal static Type GetExtendType(Type srcType)
		{
			return s_typeMapDict[srcType] as Type;
		}


		/// <summary>
		/// 注册扩展类型（继承类），表示指定的类型要对当前类型的基类做扩展（实例化时将会代替基类），
		/// 为了便于识别，建议扩展类型的名称以“Ext”结尾
		/// </summary>
		/// <param name="extType">继承类，表示将要对基类扩展（替代）</param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public static void RegisterExtendType(Type extType)
		{
			if( extType == null )
				throw new ArgumentNullException("extType");

			if( extType.IsAbstract )
				throw new ArgumentException(string.Format(
						"扩展类型 [{0}] 不能是抽象类。", extType.FullName));

			Type srcType = extType.BaseType;
			s_typeMapDict[srcType] = extType;
		}


		/// <summary>
		/// 移除扩展类型的注册（应该仅用于单元测试）
		/// </summary>
		/// <param name="extType"></param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public static void RemoveExtendType(Type extType)
		{
			if( extType == null )
				return;

			Type srcType = extType.BaseType;
			s_typeMapDict.Remove(srcType);
		}


		/// <summary>
		/// 批量注册一个程序集中所有的扩展类型
		/// </summary>
		/// <param name="asm">包含扩展类型的程序集</param>
		/// <param name="extendTypeChecker">用于判断是不是扩展类型的检查委托</param>
		public static void RegisterExtendTypes(Assembly asm, Func<Type, bool> extendTypeChecker)
		{
			if( asm == null )
				throw new ArgumentNullException("asm");
			if( extendTypeChecker == null )
				throw new ArgumentNullException("extendTypeChecker");

			foreach( Type t in asm.GetExportedTypes() ) {
				if( extendTypeChecker(t) )
					RegisterExtendType(t);
			}
		}
	

		#endregion


		#region 事件扩展管理


		/// <summary>
		/// 保存事件源与订阅类型的映射关系： Type / Type[]
		/// </summary>
		private static readonly Hashtable s_eventDict = Hashtable.Synchronized(new Hashtable(128));


		/// <summary>
		/// 获取指定事件源的所有订阅者类型
		/// 如果没有找到匹配的类型，就返回NULL
		/// </summary>
		/// <param name="eventSrcType"></param>
		/// <returns></returns>
		internal static List<Type> GetSubscribers(Type eventSrcType)
		{
			return s_eventDict[eventSrcType] as List<Type>;
		}

		/// <summary>
		/// 注册事件订阅者
		/// </summary>
		/// <param name="subscriberType">事件订阅者类型，要求从EventSubscriber&lt;T&gt;继承</param>
		/// <param name="eventSrcType">事件源类型，要求从BaseEventObject继承</param>
		/// <param name="isAdd">是：注册，否：移除注册</param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		private static void RegisterSubscriber(Type subscriberType, Type eventSrcType, bool isAdd)
		{
			if( subscriberType.IsAbstract )
				throw new ArgumentException(string.Format(
						"事件订阅者类型 [{0}] 不能是抽象类。", subscriberType.FullName));

			ConstructorInfo ctor2 = subscriberType.GetConstructor(Type.EmptyTypes);
			if( ctor2 == null )
				throw new ArgumentException(string.Format(
						"事件订阅者类型 [{0}] 没有无参的构造方法。", subscriberType.FullName));

			// 这里不再检查 subscriberType 是不是从 EventSubscriber<> 继承，由调用这个方法的代码负责检查。
			

			if( eventSrcType.IsSubclassOf(typeof(BaseEventObject)) == false )
				throw new ArgumentException(string.Format(
						"事件源类型 [{0}] 不是 BaseEventObject 的继承类。", eventSrcType.FullName));


			// 检查无参构造函数，因为不可能传入其它参数
			ConstructorInfo ctor1 = eventSrcType.GetConstructor(Type.EmptyTypes);
			if( ctor1 == null )
				throw new ArgumentException(string.Format(
						"事件源类型 [{0}] 没有无参的构造方法。", eventSrcType.FullName));


			// 将参数类型添加到事件源的订阅列表中
			List<Type> list = s_eventDict[eventSrcType] as List<Type>;

			if( isAdd ) {
				if( list == null ) {
					list = new List<Type>(4);
					s_eventDict[eventSrcType] = list;
				}

				list.Add(subscriberType);
			}
			else {
				if( list == null ) 
					return;				
				else 
					list.Remove(subscriberType);
			}
		}



		/// <summary>
		/// 注册事件订阅者，
		/// 为了便于识别，建议事件订阅者类型的名称以“EventSubscriber”结尾
		/// </summary>
		/// <param name="subscriberType">事件订阅者类型，要求从EventSubscriber&lt;T&gt;继承</param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public static void RegisterSubscriber(Type subscriberType)
		{
			if( subscriberType == null )
				throw new ArgumentNullException("subscriberType");

			// 检查是不是从 EventSubscriber<> 继承（这里不考虑多次继承）
			Type argumentType = subscriberType.BaseType.GetArgumentType(typeof(EventSubscriber<>));

			if( argumentType == null )
				throw new ArgumentException(string.Format(
						"事件订阅者类型 [{0}] 不是 EventSubscriber<T> 的继承类。", subscriberType.FullName));

			// EventSubscriber<> 的类型参数就是事件源类型
			RegisterSubscriber(subscriberType, argumentType, true);
		}

		/// <summary>
		/// 移除事件订阅者
		/// </summary>
		/// <param name="subscriberType"></param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public static void RemoveSubscriber(Type subscriberType)
		{
			if( subscriberType == null )
				return;

			// 检查是不是从 EventSubscriber<> 继承（这里不考虑多次继承）
			Type argumentType = subscriberType.BaseType.GetArgumentType(typeof(EventSubscriber<>));

			if( argumentType == null )
				return;

			RegisterSubscriber(subscriberType, argumentType, false);
		}

		/// <summary>
		/// 批量注册一个程序集中所有的事件订阅者
		/// </summary>
		/// <param name="asm"></param>
		public static void RegisterSubscribers(Assembly asm)
		{
			if( asm == null )
				throw new ArgumentNullException("asm");

			foreach( Type t in asm.GetExportedTypes() ) {
				Type argumentType = t.BaseType.GetArgumentType(typeof(EventSubscriber<>));
				if( argumentType != null)
					RegisterSubscriber(t, argumentType, true);
			}
		}


		#endregion


	}
}
