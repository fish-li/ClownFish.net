using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using ClownFish.Base;
using ClownFish.Base.Json;
using ClownFish.Base.Reflection;
using ClownFish.Web.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace ClownFish.Web.Serializer
{
	internal class JsonnetDataProvider : BaseDataProvider, IActionParametersProvider2
	{
		#region IActionParametersProvider 成员

		public object[] GetParameters(HttpContext context, MethodInfo method)
		{
			// 不需要实现这个接口，使用另一个重载版本更高效。
			throw new NotImplementedException();
		}

		#endregion

				


		public object[] GetParameters(HttpContext context, ActionDescription action)
		{
			string input = context.Request.ReadInputStream();

			if( action.Parameters.Length == 1 ) {
				object value = GetObjectFromString(input, action);
				return new object[1] { value };
			}
			else
				return GetMultiObjectsFormString(input, action, context);
		}


		private object GetObjectFromString(string input, ActionDescription action)
		{
			if( action.Parameters[0].ParameterType == typeof(string) )		// 参数要求以 string 形式接收
				return input;

			return JsonExtensions.FromJson(input, action.Parameters[0].ParameterType);
		}
		

		private object[] GetMultiObjectsFormString(string input, ActionDescription action, HttpContext context)
		{
			JObject jsonObj = JObject.Parse(input);

			DefaultJsonSerializer jsonnet = new DefaultJsonSerializer();
			JsonSerializerSettings settings = jsonnet.GetJsonSerializerSettings(false);

			Newtonsoft.Json.JsonSerializer jsonSerializer
									= Newtonsoft.Json.JsonSerializer.CreateDefault(settings);


			object[] parameters = new object[action.Parameters.Length];

			for( int i = 0; i < parameters.Length; i++ ) {

				FromBodyAttribute bodyAttr = action.Parameters[i].GetMyAttribute<FromBodyAttribute>(false);
				if( bodyAttr != null ) {
					// 当前参数需要从整体请求体中反序列化得到参数值
					parameters[i] = GetObjectFromString(input, action);
				}
				else {
					//尝试从JSON中获取一个片段，用这个片段来构造参数值
					JToken childObj = jsonObj.GetValue(action.Parameters[i].Name, StringComparison.OrdinalIgnoreCase);
					if( childObj != null ) {
						Type pType = action.Parameters[i].ParameterType;
						parameters[i] = childObj.ToObject(pType, jsonSerializer);
					}
					else {
						// 再次尝试从HTTP上下文中获取
						parameters[i] = GetParameterFromHttp(context, action.Parameters[i]);
					}
				}
			}

			return parameters;
		}



		


		





		
	}
}
