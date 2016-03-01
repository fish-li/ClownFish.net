using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Web
{
	/// <summary>
	/// 指示Action结果的序列化方式
	/// </summary>
	public enum SerializeFormat
	{
		/// <summary>
		/// 默认值，没有指定。注意：有些场景下不指定将会出现异常。
		/// </summary>
		None,
		/// <summary>
		/// 直接调用 ToString() 方法
		/// 匹配标头："text/plain"
		/// </summary>
		Text,
		/// <summary>
		/// 采用 JSON.NET 序列化为 JSON 字符串
		/// 匹配标头："application/json"
		/// </summary>
		Json,
		/// <summary>
		/// 采用 JSON.NET 序列化为 JSON 字符串，并尽量输出类型信息，可用于服务端之间或者客户端是C#的反序列化。
		/// 匹配标头："application/json"
		/// </summary>
		Json2,		// 原枚举名：JsonWithType
		/// <summary>
		/// 序列化成 XML 字符串
		/// 匹配标头："application/xml"
		/// </summary>
		Xml,
		/// <summary>
		/// 采用 "application/x-www-form-urlencoded" 方式序列化
		/// 匹配标头："application/x-www-form-urlencoded"
		/// </summary>
		Form,
		/// <summary>
		/// 自动根据请求头去响应，用于服务端响应。
		/// 客户端发起请求时，需要指定 Request.Headers["X-Result-Format"]，否则按文本输出
		/// </summary>
		Auto
	}



}
