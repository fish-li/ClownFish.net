using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Base
{
	/// <summary>
	/// 包含一些与类型相关的扩展方法
	/// </summary>
	public static class TypeExtensions
	{
		/// <summary>
		/// 判断指定的类型是不是常见的值类型，范围：DateTime，Guid，decimal， Enum
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public static bool IsCommonValueType(this Type t)
		{
			return t == typeof(DateTime)
				|| t == typeof(Guid)
				|| t == typeof(decimal)
				|| t.IsEnum;
		}


		/// <summary>
		/// 得到一个实际的类型（排除Nullable类型的影响）。比如：int? 最后将得到int
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Type GetRealType(this Type type)
		{
			if( type.IsGenericType )
				return Nullable.GetUnderlyingType(type) ?? type;
			else
				return type;
		}

		/// <summary>
		/// 判断指定的类型是不是可空类型
		/// </summary>
		/// <param name="nullableType"></param>
		/// <returns></returns>
		public static bool IsNullableType(this Type nullableType)
		{
			if( nullableType.IsGenericType
				&& nullableType.IsGenericTypeDefinition == false
				&& nullableType.GetGenericTypeDefinition() == typeof(Nullable<>) )
				return true;

			return false;
		}

		/// <summary>
		/// 判断二个类型是不是兼容（可转换）的
		/// </summary>
		/// <param name="t">要测试的类型</param>
		/// <param name="convertToType">需要转换的类型（基类或者接口类型）</param>
		/// <returns></returns>
		public static bool IsCompatible(this Type t, Type convertToType)
		{
			if( t == convertToType )
				return true;

			if( convertToType.IsInterface )
				return convertToType.IsAssignableFrom(t);
			else
				return t.IsSubclassOf(convertToType);
		}

		/// <summary>
		/// 判断方法是不是有返回值
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		public static bool HasReturn(this MethodInfo m)
		{
			return m.ReturnType != typeof(void);
		}

		/// <summary>
		/// 判断是不是一个 Task 方法
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		public static bool IsTaskMethod(this MethodInfo method)
		{
			if( method.ReturnType == typeof(Task) )
				return true;

			if( method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>) )
				return true;


			return false;
		}


		/// <summary>
		/// 检查是不是Task&lt;T&gt;方法，如果是，则返回类型参数T，否则返回 null
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		public static Type GetTaskMethodResultType(this MethodInfo method)
		{
			Type type = method.ReturnType;

			if( type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>) )
				return type.GetGenericArguments()[0];


			return null;
		}

	}
}
