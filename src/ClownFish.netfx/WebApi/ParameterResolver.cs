using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Base.Http;
using ClownFish.Http.Pipleline;

namespace ClownFish.WebApi
{
	/// <summary>
	/// 用于执行Action时，构造方法参数的解析器
	/// </summary>
	internal static class ParameterResolver
	{
		/// <summary>
		/// 从HttpRequest中构造将要调用的方法的所有参数值
		/// </summary>
		/// <param name="method"></param>
		/// <param name="request"></param>
		/// <returns></returns>
		public static object[] GetParameters(MethodInfo method, NHttpRequest request)
		{
			var ps = method.GetParameters();
			if( ps.Length == 0 )
				return null;

			bool onlyOne = ps.Length == 1;
			object[] parameters = new object[ps.Length];

			for( int i = 0; i < ps.Length; i++ ) {
				ParameterInfo p = ps[i];
				parameters[i] = GetParameterValue(p, request, onlyOne);
			}

			return parameters;
		}



		private static object GetParameterValue(ParameterInfo p, NHttpRequest requst, bool onlyOne)
		{
			// 参数的第一种取值方式：通过[FromBody]指定，反序列化请求流
			FromBodyAttribute attr = p.GetMyAttribute<FromBodyAttribute>();
			if( attr != null ) {
				return FromBodyDeserializeObject(p, requst, true);
			}


			// 参数的第三种取值方式：根据参数名，从各种数据集合中获取（Route, QueryString, Form, Headers）
			string value = requst.GetValue(p.Name);

			if( value != null ) {
				// 非序列化的参数，允许可空类型
				return StringConverter.ChangeType(value, p.ParameterType.GetRealType());
			}

			// 如果只有一个参数，就默认从Requestbody中加载参数值
			if( onlyOne )
				return FromBodyDeserializeObject(p, requst, false);


			// 查找失败
			return null;
		}


		private static object FromBodyDeserializeObject(ParameterInfo p, NHttpRequest requst, bool throwOnError)
		{
            // 从请求流中反序列化对象中，要注意三点：
            // 1、忽略参数的名称
            // 2、直接使用参数类型，不做可空类型处理
            // 3、仅支持 JSON, XML 的数据格式

            SerializeFormat format = RequestContentType.GetFormat(requst.ContentType);
            if( format == SerializeFormat.Json ) {
				return JsonExtensions.FromJson(requst.GetBodyText(), p.ParameterType);
			}

			if( format == SerializeFormat.Xml ) {
				return XmlExtensions.FromXml(requst.GetBodyText(), p.ParameterType);
			}

			// 仅仅是需要读取整个请求流字符串，
			// 而且目标类型已经是字符串，就没有必要反序列化了，所以就直接以字符串返回
			if( p.ParameterType == typeof(string) ) {
				return requst.GetBodyText();
			}

			if( throwOnError )
				throw new NotSupportedException("[FromBody]标记只能配合 JSON/XML 数据格式来使用。");
			else
				return null;
		}
	}
}
