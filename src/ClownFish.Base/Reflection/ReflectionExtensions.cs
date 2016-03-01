using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ClownFish.Base.Framework;

namespace ClownFish.Base.Reflection
{

	/// <summary>
	/// 一些扩展方法，用于反射操作，它们都可以优化反射性能。
	/// </summary>
	public static class ReflectionExtensions
	{
		
		/// <summary>
		/// 获取一个封闭泛型的类型参数
		/// </summary>
		/// <param name="type">一个具体的封装泛型类型</param>
		/// <param name="baseTypeDefinition">泛型定义</param>
		/// <returns>泛型的类型参数</returns>
		public static Type GetArgumentType(this Type type, Type baseTypeDefinition)
		{
			if( type == null )
				throw new ArgumentNullException("type");

			if( baseTypeDefinition == null )
				throw new ArgumentNullException("baseTypeDefinition");

			if( baseTypeDefinition.IsGenericTypeDefinition == false )
				throw new ArgumentException("参数必须是一个泛型定义。", "baseTypeDefinition");


			if( type.IsGenericType && type.GetGenericTypeDefinition() == baseTypeDefinition )
				return type.GetGenericArguments()[0];


			return null;
		}


		/// <summary>
		/// 获取某个反射成员的自定义修饰属性定义（单个定义）
		/// </summary>
		/// <typeparam name="T">要查找的修饰属性类型</typeparam>
		/// <param name="m">反射成员对象，例如：方法信息</param>
		/// <param name="inherit">搜索此成员的继承链以查找这些属性，则为 true；否则为 false。</param>
		/// <returns></returns>
		public static T GetMyAttribute<T>(this MemberInfo m, bool inherit = false) where T : Attribute
		{
			T[] array = m.GetCustomAttributes(typeof(T), inherit) as T[];

			if( array.Length == 1 )
				return array[0];

			if( array.Length > 1 )
				throw new InvalidProgramException(string.Format("方法 {0} 不能同时指定多次 [{1}]。", m.Name, typeof(T)));

			return default(T);
		}



		/// <summary>
		/// 获取某个反射成员的自定义修饰属性定义（多个定义）
		/// </summary>
		/// <typeparam name="T">要查找的修饰属性类型</typeparam>
		/// <param name="m">反射成员对象，例如：方法信息</param>
		/// <param name="inherit">搜索此成员的继承链以查找这些属性，则为 true；否则为 false。</param>
		/// <returns></returns>
		public static T[] GetMyAttributes<T>(this MemberInfo m, bool inherit = false) where T : Attribute
		{
			return m.GetCustomAttributes(typeof(T), inherit) as T[];
		}


		/// <summary>
		/// 获取当前程序加载的所有程序集
		/// </summary>
		/// <returns></returns>
		public static ICollection GetReferencedAssemblies()
		{
			if( WebConfig.IsAspnetApp )
				return System.Web.Compilation.BuildManager.GetReferencedAssemblies();
			else
				return AppDomain.CurrentDomain.GetAssemblies();
		}

		/// <summary>
		/// 获取带个指定修饰属性的程序集列表
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static List<Assembly> GetAssemblyList<T>() where T : Attribute
		{
			List<Assembly> list = new List<Assembly>(128);

			ICollection assemblies = GetReferencedAssemblies();
			foreach( Assembly assembly in assemblies ) {
				// 过滤以【System】开头的程序集，加快速度
				if( assembly.FullName.StartsWith("System", StringComparison.OrdinalIgnoreCase) )
					continue;

				if( assembly.GetCustomAttributes(typeof(T), false).Length == 0 )
					continue;

				list.Add(assembly);
			}

			return list;
		}


	}
}
