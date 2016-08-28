using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

		#region GetAttributes

		/// <summary>
		/// 获取类型成员的Attribute（单个定义）
		/// </summary>
		/// <typeparam name="T">要查找的修饰属性类型</typeparam>
		/// <param name="m">包含attribute的类成员对象</param>
		/// <param name="inherit">搜索此成员的继承链以查找这些属性，则为 true；否则为 false。</param>
		/// <returns></returns>
		public static T GetMyAttribute<T>(this MemberInfo m, bool inherit = false) where T : Attribute
		{
			return AttributeCache.GetOne<T>(m, inherit);
		}



		/// <summary>
		/// 获取类型成员的Attribute（多个定义）
		/// </summary>
		/// <typeparam name="T">要查找的修饰属性类型</typeparam>
		/// <param name="m">包含attribute的类成员对象</param>
		/// <param name="inherit">搜索此成员的继承链以查找这些属性，则为 true；否则为 false。</param>
		/// <returns></returns>
		public static T[] GetMyAttributes<T>(this MemberInfo m, bool inherit = false) where T : Attribute
		{
			return AttributeCache.GetArray<T>(m, inherit);
		}


		/// <summary>
		/// 获取参数对象的Attribute（单个定义）
		/// </summary>
		/// <typeparam name="T">要查找的修饰属性类型</typeparam>
		/// <param name="p">包含attribute的参数对象</param>
		/// <param name="inherit">搜索此成员的继承链以查找这些属性，则为 true；否则为 false。</param>
		/// <returns></returns>
		public static T GetMyAttribute<T>(this ParameterInfo p, bool inherit = false) where T : Attribute
		{
			return AttributeCache.GetOne<T>(p, inherit);
		}



		/// <summary>
		/// 获取参数对象的Attribute（多个定义）
		/// </summary>
		/// <typeparam name="T">要查找的修饰属性类型</typeparam>
		/// <param name="p">包含attribute的参数对象</param>
		/// <param name="inherit">搜索此成员的继承链以查找这些属性，则为 true；否则为 false。</param>
		/// <returns></returns>
		public static T[] GetMyAttributes<T>(this ParameterInfo p, bool inherit = false) where T : Attribute
		{
			return AttributeCache.GetArray<T>(p, inherit);
		}


		/// <summary>
		/// 获取类型对象的Attribute（单个定义）
		/// </summary>
		/// <typeparam name="T">要查找的修饰属性类型</typeparam>
		/// <param name="t">包含attribute的类型</param>
		/// <param name="inherit">搜索此成员的继承链以查找这些属性，则为 true；否则为 false。</param>
		/// <returns></returns>
		public static T GetMyAttribute<T>(this Type t, bool inherit = false) where T : Attribute
		{
			return AttributeCache.GetOne<T>(t, inherit);
		}



		/// <summary>
		/// 获取类型对象的Attribute（多个定义）
		/// </summary>
		/// <typeparam name="T">要查找的修饰属性类型</typeparam>
		/// <param name="t">包含attribute的类型</param>
		/// <param name="inherit">搜索此成员的继承链以查找这些属性，则为 true；否则为 false。</param>
		/// <returns></returns>
		public static T[] GetMyAttributes<T>(this Type t, bool inherit = false) where T : Attribute
		{
			return AttributeCache.GetArray<T>(t, inherit);
		}


		/// <summary>
		/// 等同于调用 Assembly实例的GetCustomAttributes()，只是在缺少依赖程序集时能指出当前程序集的名称。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="assembly"></param>
		/// <returns></returns>
		public static T[] GetAttributes<T>(this Assembly assembly) where T : Attribute
		{
			// 获取程序集的Attribute使用场景较少，就不使用缓存版本了

			if( assembly == null )
				throw new ArgumentNullException("assembly");

			try {
				return (T[])assembly.GetCustomAttributes(typeof(T), true);
			}
			catch( FileNotFoundException ex ) {
				throw new InvalidOperationException(
							"反射程序集时无法加载依赖项，当前程序集名称：" + assembly.FullName, ex);
			}
			catch( FileLoadException ex ) {
				throw new InvalidOperationException(
							"反射程序集时无法加载依赖项，当前程序集名称：" + assembly.FullName, ex);
			}
			catch( TypeLoadException ex ) {
				throw new InvalidOperationException(
							"反射程序集时无法加载依赖项，当前程序集名称：" + assembly.FullName, ex);
			}
		}

		#endregion

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
		/// 等同于调用 Assembly实例的GetExportedTypes()，只是在缺少依赖程序集时能指出当前程序集的名称。
		/// </summary>
		/// <param name="assembly"></param>
		/// <returns></returns>
		public static Type[] GetPublicTypes(this Assembly assembly)
		{
			if( assembly == null )
				throw new ArgumentNullException("assembly");

			try {
				return assembly.GetExportedTypes();
			}
			catch( FileNotFoundException ex ) {
				throw new InvalidOperationException(
							"反射程序集时无法加载依赖项，当前程序集名称：" + assembly.FullName, ex);
			}
			catch( FileLoadException ex ) {
				throw new InvalidOperationException(
							"反射程序集时无法加载依赖项，当前程序集名称：" + assembly.FullName, ex);
			}
			catch( TypeLoadException ex ) {
				throw new InvalidOperationException(
							"反射程序集时无法加载依赖项，当前程序集名称：" + assembly.FullName, ex);
			}
		}

		
		

		/// <summary>
		/// 获取带个指定修饰属性的程序集列表
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static List<Assembly> GetAssemblyList<T>() where T : Attribute
		{
			List<Assembly> list = new List<Assembly>(128);

			ICollection assemblies = RunTimeEnvironment.GetLoadAssemblies(true);
			foreach( Assembly assembly in assemblies ) {
				
				if( assembly.GetAttributes<T>().Length == 0 )
					continue;
				
				list.Add(assembly);
			}

			return list;
		}


	}
}
