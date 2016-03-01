using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using ClownFish.Base.Common;
using ClownFish.Base.TypeExtend;
using ClownFish.Web.Reflection;

namespace ClownFish.Web.Serializer
{
	internal class XmlDataProvider : BaseDataProvider, IActionParametersProvider2
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
			if( action.Parameters.Length == 1 ) {
				object value = GetObjectFromRequest(context, action);
				return new object[1] { value };
			}
			else
				return GetMultiObjectsFormRequest(context, action);
		}


		private object GetObjectFromRequest(HttpContext context, ActionDescription action)
		{
			HttpRequest request = context.Request;

			if( action.Parameters[0].ParameterType == typeof(string) )		// 参数要求以 string 形式接收
				return request.ReadInputStream();


			if( action.Parameters[0].ParameterType == typeof(XmlDocument) ) {		// 参数要求以 XmlDocument 形式接收
				string xml = request.ReadInputStream();
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(xml);
				return doc;
			}

			Type destType = action.Parameters[0].ParameterType.GetRealType();

			XmlSerializer mySerializer = new XmlSerializer(destType);

			request.InputStream.Position = 0;
			StreamReader sr = new StreamReader(request.InputStream, request.ContentEncoding);
			return mySerializer.Deserialize(sr);
		}

		private object[] GetMultiObjectsFormRequest(HttpContext context, ActionDescription action)
		{
			HttpRequest request = context.Request;
			string xml = request.ReadInputStream();

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xml);

			XmlNode root = doc.LastChild;

			//if( root.ChildNodes.Count != action.Parameters.Length )
			//    throw new ArgumentException("客户端提交的数据项与服务端的参数项的数量不匹配。");

			string name = null;
			object value = null;
			object[] parameters = new object[action.Parameters.Length];
			LazyObject<ModelBuilder> builder = new LazyObject<ModelBuilder>();

			for( int i = 0; i < parameters.Length; i++ ) {				

				FromBodyAttribute bodyAttr = action.Parameters[i].GetCustomAttribute<FromBodyAttribute>(false);
				if( bodyAttr != null ) {
					// 当前参数需要从整体请求体中反序列化得到参数值
					parameters[i] = GetObjectFromRequest(context, action);
				}
				else {					
					value = null;

					if( TryGetSpecialParameter(context, action.Parameters[i], out value) ) {
						parameters[i] = value;		// 特殊参数，直接赋值，不需要从XML中读取
					}
					else {
						name = action.Parameters[i].Name;
						XmlNode node = (from n in root.ChildNodes.Cast<XmlNode>()
										where string.Compare(n.Name, name, StringComparison.OrdinalIgnoreCase) == 0
										select n).FirstOrDefault();

						if( node != null ) {
							try {
								object parameter = null;
								Type destType = action.Parameters[i].ParameterType.GetRealType();

								if( destType.IsSupportableType() ) {	// 如果是简单类型，就不需要反序列化
									parameter = builder.Instance.StringToObject(node.InnerText, destType);
								}
								else
									// 复杂类型的参数，就使用反序列化
									parameter = XmlDeserialize(node.OuterXml, destType, request.ContentEncoding);

								parameters[i] = parameter;

							}
							catch( Exception ex ) {
								throw new InvalidCastException("数据转换失败，当前参数名：" + name, ex);
							}
						}
						else {
							// 再次尝试从HTTP上下文中获取
							parameters[i] = GetObjectFromHttp(context, action.Parameters[i]);
						}
					}
				}
			}

			return parameters;
		}


		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202")]
		private static object XmlDeserialize(string xml, Type destType, Encoding encoding)
		{
			if( string.IsNullOrEmpty(xml) )
				throw new ArgumentNullException("xml");
			if( encoding == null )
				throw new ArgumentNullException("encoding");

			XmlSerializer mySerializer = new XmlSerializer(destType);
			using( MemoryStream ms = new MemoryStream(encoding.GetBytes(xml)) ) {
				using( StreamReader sr = new StreamReader(ms, encoding) ) {
					return mySerializer.Deserialize(sr);
				}
			}
		}


		
	}
}
