//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Web.Script.Serialization;
//using System.Reflection;
//using System.Web;
//using ClownFish.Web.OptimizeReflection;
//using ClownFish.Web.Reflection;


//namespace ClownFish.Web.Serializer
//{
//	internal class JsonDataProvider : IActionParametersProvider2
//	{
//		#region IActionParametersProvider 成员

//		public object[] GetParameters(HttpContext context, MethodInfo method)
//		{
//			// 不需要实现这个接口，使用另一个重载版本更高效。
//			throw new NotImplementedException();
//		}

//		#endregion

//		// ASP.NET 4.0 为下面二个方法增加了重载版本，所以必须指定更多的匹配条件。
//		private static readonly MethodInfo s_methodDeserialize
//				= typeof(JavaScriptSerializer).GetMethod("Deserialize", 
//							BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(string)}, null);

//		private static readonly MethodInfo s_methodConvertToType
//				= typeof(JavaScriptSerializer).GetMethod("ConvertToType",
//							BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(object) }, null);

//		JavaScriptSerializer jss = new JavaScriptSerializer();

//		public object[] GetParameters(HttpContext context, ActionDescription action)
//		{
//			string input = context.Request.ReadInputStream();
			
//			if( action.Parameters.Length == 1 ) {
//				object value = GetObjectFromString(input, action);
//				return new object[1] { value };
//			}
//			else
//				return GetMultiObjectsFormString(input, action);
//		}


//		private object GetObjectFromString(string input, ActionDescription action)
//		{
//			if( action.Parameters[0].ParameterType == typeof(string) )
//				return input;

//			Type destType = action.Parameters[0].ParameterType.GetRealType();

//			MethodInfo deserialize = s_methodDeserialize.MakeGenericMethod(destType);

//			return deserialize.FastInvoke(jss, new object[] { input });
//		}

//		private object[] GetMultiObjectsFormString(string input, ActionDescription action)
//		{
//			Dictionary<string, object> dict = jss.DeserializeObject(input) as Dictionary<string, object>;

//			//if( dict.Count != action.Parameters.Length )
//			//    throw new ArgumentException("客户端提交的数据项与服务端的参数项的数量不匹配。");

//			object[] parameters = new object[action.Parameters.Length];

//			for( int i = 0; i < parameters.Length; i++ ) {
//				string name = action.Parameters[i].Name;
//				object value = (from kv in dict
//								where string.Compare(kv.Key, name, StringComparison.OrdinalIgnoreCase) == 0
//								select kv.Value).FirstOrDefault();

//				try {
//					if( value != null ) {
//						Type destType = action.Parameters[i].ParameterType.GetRealType();

//						MethodInfo method = s_methodConvertToType.MakeGenericMethod(destType);
//						object parameter = method.FastInvoke(jss, new object[] { value });
//						parameters[i] = parameter;
//					}
//				}
//				catch( Exception ex ) {
//					throw new InvalidCastException("数据转换失败，当前参数名：" + name, ex);
//				}
//			}

//			return parameters;
//		}








		
//	}
//}
