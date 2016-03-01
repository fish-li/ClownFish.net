using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using ClownFish.Base.Xml;
using ClownFish.Log;

namespace ClownFish.Log.Model
{
	/// <summary>
	/// 包含记录日志的HTTP相关信息
	/// </summary>
	public class HttpInfo
	{
		/// <summary>
		/// 当前登录用户的用户名，可不填。
		/// </summary>
		[XmlElement(Order = 1)]
		public string UserName { get; set; }


		/// <summary>
		/// 请求头信息
		/// </summary>
		[XmlElement(Order = 2)]
		public XmlCdata RequestText { get; set; }


		/// <summary>
		/// 页面地址
		/// </summary>
		[XmlElement(Order = 3)]
		public string Url { get; set; }


		/// <summary>
		/// 浏览器类型。注意：此信息可能不准确。
		/// </summary>
		[XmlElement(Order = 4)]
		public string Browser { get; set; }

			

		/// <summary>
		/// 当前请求的Session信息（可能为 NULL）
		/// </summary>
		[XmlElement(Order = 5)]
		public XmlCdata Session { get; set; }




		/// <summary>
		/// 根据HttpContext实例创建并填充HttpInfo对象
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public static HttpInfo Create(HttpContext context)
		{
			//HttpContext context = HttpContext.Current;
			if( context == null || context.Request == null )
				return null;


			HttpInfo info = new HttpInfo();
			info.SetHttpInfo(context);
			return info;
		}


		/// <summary>
		/// 一个特定的字符串，指示在日志记录时，不记录请求体内容。
		/// 如果不希望记录某个请求体的内容，可以设置 context.Items[IgnoreHttpsRequestBody] = "yes";
		/// </summary>
		public static readonly string IgnoreHttpsRequestBody = "MAP6-Log:IgnoreHttpsRequestBody";


		/// <summary>
		/// 设置请求信息
		/// </summary>
		private void SetHttpInfo(HttpContext context)
		{
			if( context == null )
				return;


			if( context.Request.IsAuthenticated )
				this.UserName = context.User.Identity.Name;


			this.Url = context.Request.Url.ToString();

			if( context.Request.Browser != null )
				this.Browser = context.Request.Browser.Browser + context.Request.Browser.MajorVersion;


			string postData = null;

			// 这里可能会有一个安全漏洞，应该可能会记录一些敏感信息
			if( context.Items[IgnoreHttpsRequestBody] == null ) {
				if( context.Request.RequestType == "POST" ) {
					if( context.Request.Files.Count == 0 ) {
						postData = context.Request.ReadInputStream();
						context.Request.InputStream.Position = 0;
					}
					else {
						StringBuilder sb = new StringBuilder();
						foreach( string name in context.Request.Form.AllKeys ) {
							string[] values = context.Request.Form.GetValues(name);
							if( values != null ) {
								foreach( string value in values )
									sb.AppendFormat("&{0}={1}", HttpUtility.UrlEncode(name), HttpUtility.UrlEncode(value));
							}
						}

						if( sb.Length > 0 ) {
							sb.Remove(0, 1);
							postData = sb.ToString();
						}
					}
				}
			}
			

			if( context.Request.Headers.Count > 0 ) {
				StringBuilder sb = new StringBuilder();
				sb.AppendLine()
					.Append(context.Request.RequestType)
					.Append(" ")
					.Append(context.Request.Url.ToString())
					.AppendLine(" HTTP/1.1");

				foreach( string key in context.Request.Headers.AllKeys ) {
					string value = context.Request.Headers[key];
					sb.Append(key).Append(": ").Append(value).AppendLine();
				}

				if( postData != null )
					sb.AppendLine().AppendLine(postData);

				this.RequestText = sb.ToString();
			}


			

			


			if( context.Session != null ) {
				StringBuilder sb = new StringBuilder();
				sb.AppendLine();

				foreach( string sessionKey in context.Session.Keys ) {
					object sessionValue = context.Session[sessionKey];
					if( sessionValue == null )
						sb.AppendFormat("{0}: NULL\r\n", sessionKey);
					else
						sb.AppendFormat("{0}: ({1}) {2}\r\n", sessionKey, sessionValue.GetType().Name, sessionValue);
				}
				this.Session = sb.ToString();
			}
		}



		

	}
}
