using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ClownFish.Base;
using ClownFish.Base.Http;
using ClownFish.Base.Reflection;
using ClownFish.Base.TypeExtend;
using ClownFish.Web.Reflection;

namespace ClownFish.Web.Serializer
{
	/// <summary>
	/// 所有Action参数提供者的基类，提供允许从HTTP请求中构造参数的功能
	/// </summary>
	public abstract class BaseDataProvider
	{
		/// <summary>
		/// 根据指定的参数信息，从HTTP请求中构造参数
		/// </summary>
		/// <param name="context"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		protected object GetParameterFromHttp(HttpContext context, ParameterInfo p)
		{
			if( p.IsOut )
				throw new NotImplementedException();

			if( p.ParameterType == typeof(VoidType) )
				return null;        // 忽略用于方法重载识别的【空参数】


			object result = GetAspnetObject(context, p)
							?? GetByCustomDataAttribute(context, p)
							?? GetByLoadFromHttpMethod(context, p)
							?? GetByIHttpDataConvert(context, p)
							?? GetSupportableValue(context, p)
							?? GetObjectFromHttp(context, p);

			// 检查是否是特殊的结果，在这里只用于跳过合并操作符
			if( object.ReferenceEquals(result, VoidType.Value) )
				return null;
			else
				return result;
		}


		/// <summary>
		/// 根据指定的参数信息，尝试从HTTP上下文中获取参数值
		/// </summary>
		/// <param name="context">HttpContext实例</param>
		/// <param name="p">ParameterInfo实例</param>
		/// <returns></returns>
		private object GetAspnetObject(HttpContext context, ParameterInfo p)
		{
			if( p.ParameterType == typeof(HttpContext) ) 
				return context;


			if( p.ParameterType == typeof(NameValueCollection) ) {
				if( p.Name.EqualsIgnoreCase("Form") )
					return context.Request.Form;

				else if( p.Name.EqualsIgnoreCase("QueryString") )
					return context.Request.QueryString;

				else if( p.Name.EqualsIgnoreCase("Headers") )
					return context.Request.Headers;

				else if( p.Name.EqualsIgnoreCase("ServerVariables") )
					return context.Request.ServerVariables;
			}

			return null;
		}


		
		private object GetByCustomDataAttribute(HttpContext context, ParameterInfo p)
		{
			CustomDataAttribute attr = p.GetMyAttribute<CustomDataAttribute>(false);
			if( attr == null )
				return null;

			return attr.GetHttpValue(context, p);
		}


		private object GetByIHttpDataConvert(HttpContext context, ParameterInfo p)
		{
			IHttpDataConvert convert = HttpDataConvertFactory.GetConvert(p.ParameterType);
			if( convert == null )
				return null;

			return convert.Convert(context, p.Name);
		}
		

		private object GetByLoadFromHttpMethod(HttpContext context, ParameterInfo p)
		{
			// 如果参数类型实现了下面这样一个工厂方法：
			// public static object LoadFromHttp(HttpContext context, ParameterInfo p)
			// 这里就调用它来创建参数对象

			// 这里只判断方法名称和参数个数及参数类型，不检查返回值类型

			MethodInfo m = p.ParameterType.GetMethod("LoadFromHttp",
							BindingFlags.Public | BindingFlags.Static |  BindingFlags.FlattenHierarchy,
							null, new Type[] { typeof(HttpContext), typeof(ParameterInfo) }, null);

			if( m == null )
				return null;

			return m.FastInvoke(null, new object[] { context, p });
		}

		private object GetSupportableValue(HttpContext context, ParameterInfo p)
		{
			Type paramterType = p.ParameterType.GetRealType();

			// 如果参数是可支持的类型，则直接从HttpRequest中读取并赋值
			if( paramterType.IsSupportableType() ) {
				ModelBuilder builder = ObjectFactory.New<ModelBuilder>();
				object val = builder.GetValueFromHttp(context, p.Name, paramterType);
				if( val != null )
					return val;

				else {
					// C#5 支持参数默认值。
					if( p.HasDefaultValue )
						return p.DefaultValue;

					if( p.ParameterType.IsValueType && p.ParameterType.IsNullableType() == false )
						throw new ArgumentException("未能找到指定的参数值：" + p.Name);
					else
						return VoidType.Value;	// 返回一个特殊的对象，表示 null，以便于路过后面的合并操作符
				}
			}

			return null;
		}

		/// <summary>
		/// 根据指定的参数信息，从HTTP请求中构造【对象类型】参数
		/// </summary>
		/// <param name="context"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		private object GetObjectFromHttp(HttpContext context, ParameterInfo p)
		{
			ModelBuilder builder = ObjectFactory.New<ModelBuilder>();

			// 自定义的类型。首先创建实例，然后给所有成员赋值。
			return builder.CreateObjectFromHttp(context, p);
		}

		


	}
}
