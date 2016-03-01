using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Base.Reflection
{
	/// <summary>
	/// 包含用于优化反射调用性能的扩展类
	/// </summary>
	public static class ReflectionOptimizeExtensions
	{
		private static readonly Hashtable s_getterDict = Hashtable.Synchronized(new Hashtable(10240));
		private static readonly Hashtable s_setterDict = Hashtable.Synchronized(new Hashtable(10240));
		private static readonly Hashtable s_methodDict = Hashtable.Synchronized(new Hashtable(10240));

		/// <summary>
		/// 用优化的方式快速读取FieldInfo
		/// </summary>
		/// <param name="fieldInfo"></param>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static object FastGetValue(this FieldInfo fieldInfo, object obj)
		{
			if( fieldInfo == null )
				throw new ArgumentNullException("fieldInfo");

			GetValueDelegate getter = (GetValueDelegate)s_getterDict[fieldInfo];
			if( getter == null ) {
				getter = DynamicMethodFactory.CreateFieldGetter(fieldInfo);
				s_getterDict[fieldInfo] = getter;
			}

			return getter(obj);
		}

		/// <summary>
		/// 用优化的方式快速写FieldInfo
		/// </summary>
		/// <param name="fieldInfo"></param>
		/// <param name="obj"></param>
		/// <param name="value"></param>
		public static void FastSetField(this FieldInfo fieldInfo, object obj, object value)
		{
			if( fieldInfo == null )
				throw new ArgumentNullException("fieldInfo");

			SetValueDelegate setter = (SetValueDelegate)s_setterDict[fieldInfo];
			if( setter == null ) {
				setter = DynamicMethodFactory.CreateFieldSetter(fieldInfo);
				s_setterDict[fieldInfo] = setter;
			}

			setter(obj, value);
		}

		/// <summary>
		/// 根据指定的Type，用优化的方式快速创建实例
		/// </summary>
		/// <param name="instanceType"></param>
		/// <returns></returns>
		public static object FastNew(this Type instanceType)
		{
			if( instanceType == null )
				throw new ArgumentNullException("instanceType");

			CtorDelegate ctor = (CtorDelegate)s_methodDict[instanceType];
			if( ctor == null ) {
				ConstructorInfo ctorInfo = instanceType.GetConstructor(Type.EmptyTypes);

				if( ctorInfo == null )
					throw new NotSupportedException(string.Format("类型\"{0}\"没有无参的构造方法。", instanceType.ToString()));

				ctor = DynamicMethodFactory.CreateConstructor(ctorInfo);
				s_methodDict[instanceType] = ctor;
			}

			return ctor();
		}



		/// <summary>
		/// 用优化的方式快速读取PropertyInfo
		/// </summary>
		/// <param name="propertyInfo"></param>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static object FastGetValue2(this PropertyInfo propertyInfo, object obj)
		{
			if( propertyInfo == null )
				throw new ArgumentNullException("propertyInfo");

			GetValueDelegate getter = (GetValueDelegate)s_getterDict[propertyInfo];
			if( getter == null ) {
				getter = DynamicMethodFactory.CreatePropertyGetter(propertyInfo);
				s_getterDict[propertyInfo] = getter;
			}

			return getter(obj);
		}

		/// <summary>
		/// 用优化的方式快速写PropertyInfo
		/// </summary>
		/// <param name="propertyInfo"></param>
		/// <param name="obj"></param>
		/// <param name="value"></param>
		public static void FastSetValue2(this PropertyInfo propertyInfo, object obj, object value)
		{
			if( propertyInfo == null )
				throw new ArgumentNullException("propertyInfo");

			SetValueDelegate setter = (SetValueDelegate)s_setterDict[propertyInfo];
			if( setter == null ) {
				setter = DynamicMethodFactory.CreatePropertySetter(propertyInfo);
				s_setterDict[propertyInfo] = setter;
			}

			setter(obj, value);
		}


		/// <summary>
		/// 用优化的方式快速调用一个方法
		/// </summary>
		/// <param name="methodInfo"></param>
		/// <param name="obj"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public static object FastInvoke2(this MethodInfo methodInfo, object obj, params object[] parameters)
		{
			if( methodInfo == null )
				throw new ArgumentNullException("methodInfo");

			MethodDelegate invoker = (MethodDelegate)s_methodDict[methodInfo];
			if( invoker == null ) {
				invoker = DynamicMethodFactory.CreateMethod(methodInfo);
				s_methodDict[methodInfo] = invoker;
			}

			return invoker(obj, parameters);
		}



	}
}
