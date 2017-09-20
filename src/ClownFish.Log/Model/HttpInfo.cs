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
using MongoDB.Bson.Serialization.Attributes;

namespace ClownFish.Log.Model
{
	/// <summary>
	/// 包含记录日志的HTTP相关信息
	/// </summary>
#if _MongoDB_
	[BsonIgnoreExtraElements]
#endif
	public class HttpInfo
	{
		/// <summary>
		/// 当前登录用户的用户名，可不填。
		/// </summary>
		public string UserName { get; set; }


		/// <summary>
		/// 请求头信息
		/// </summary>
		public XmlCdata RequestText { get; set; }


		/// <summary>
		/// 页面地址
		/// </summary>
		public string Url { get; set; }

		/// <summary>
		/// 页面原始URL
		/// </summary>
		public string RawUrl { get; set; }


		/// <summary>
		/// 浏览器类型。注意：此信息可能不准确。
		/// </summary>
		public string Browser { get; set; }

			

		/// <summary>
		/// 当前请求的Session信息（可能为 NULL）
		/// </summary>
		public XmlCdata Session { get; set; }




		/// <summary>
		/// 根据HttpContext实例创建并填充HttpInfo对象
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public static HttpInfo Create(HttpContext context)
		{
			try {
				//HttpContext context = HttpContext.Current;
				if( context == null 
					|| context.Request == null	// 当程序在初始化时，访问Request属性可能会出现异常
					)
					return null;
			}
			catch {
				return null;
			}

			HttpInfo info = new HttpInfo();
			info.SetHttpInfo(context);
			return info;
		}


		/// <summary>
		/// 一个特定的字符串，指示在日志记录时，不记录请求体内容。
		/// 如果不希望记录某个请求体的内容，可以设置 context.Items[IgnoreHttpsRequestBody] = "yes";
		/// </summary>
		public static readonly string IgnoreHttpsRequestBody = "ClownFish.Log:IgnoreHttpsRequestBody";


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
            this.RawUrl = context.Request.RawUrl;

            if( context.Request.Browser != null )
                this.Browser = context.Request.Browser.Browser + context.Request.Browser.MajorVersion;


            GetRequestText(context);
            GetSession(context);
        }


        private void GetRequestText(HttpContext context)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine()
                .Append(context.Request.RequestType)
                .Append(" ")
                .Append(context.Request.Url.ToString())
                .AppendLine(" HTTP/1.1");

            if( context.Request.Headers.Count > 0 ) {
                foreach( string key in context.Request.Headers.AllKeys ) {
                    string value = context.Request.Headers[key];
                    sb.Append(key).Append(": ").Append(value).AppendLine();
                }
            }


            string postData = GetPostText(context);
            if( postData != null )
                sb.AppendLine().AppendLine(postData);

            this.RequestText = sb.ToString();
        }

        private string GetPostText(HttpContext context)
        {
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

            return postData;
        }


        private void GetSession(HttpContext context)
        {
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
