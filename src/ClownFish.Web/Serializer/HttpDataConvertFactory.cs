using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ClownFish.Base.Http;
using ClownFish.Base.Reflection;

namespace ClownFish.Web.Serializer
{
	/// <summary>
	/// IHttpDataConvert接口与实现的管理工厂类
	/// </summary>
	public static class HttpDataConvertFactory
	{
		static HttpDataConvertFactory()
		{
			// 注册二个内置的转换器，用于处理HttpFile类型
			Register<HttpFile>(new HttpFileDataConvert());
			Register<HttpFile[]>(new HttpFileArrayDataConvert());
		}

		private static readonly Hashtable s_convertTable = Hashtable.Synchronized(new Hashtable(256));


		/// <summary>
		/// 注册一个IHttpDataConvert的实现类型的可重用【实例】
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance"></param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public static void Register<T>(IHttpDataConvert instance)
		{
			Type dataType = typeof(T);

			// 采用覆盖的方式，如果类型注册多次，以最后一次为准
			s_convertTable[dataType] = instance;
		}

		/// <summary>
		/// 注册一个IHttpDataConvert的【实现类型】
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="convertType"></param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public static void Register<T>(Type convertType)
		{
			Type dataType = typeof(T);

			// 采用覆盖的方式，如果类型注册多次，以最后一次为准
			s_convertTable[dataType] = convertType;
		}

		/// <summary>
		/// 获取一个IHttpDataConvert的实例
		/// </summary>
		/// <param name="dataType"></param>
		/// <returns></returns>
		public static IHttpDataConvert GetConvert(Type dataType)
		{
			object value = s_convertTable[dataType];
			if( value != null ) {
				if( value is Type )
					return (value as Type).FastNew() as IHttpDataConvert;
				else
					return value as IHttpDataConvert;
			}

			return null;
		}
	}
}
