using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ClownFish.MockAspnetRuntime;

namespace ClownFish.Web.UnitTest
{
	public class TestPage : System.Web.UI.Page
	{
		public string ReadQuerySting(string name)
		{
			return Request.QueryString[name];
		}

		public string ReadForm(string name)
		{
			return Request.Form[name];
		}

		public string ReadParams(string name)
		{
			return Request.Params[name];
		}

		public string ReadItem(string name)
		{
			return Request[name];
		}

		public string ReadHeader(string name)
		{
			// 读【请求】头
			return Request.Headers[name];
		}

		public void WriteHeader(string name, string value)
		{
			// 写【响应】头
			Response.AppendHeader(name, value);
		}

		public object ReadSession(string name)
		{
			return Session[name];
		}

		public void WriteSession(string name, object value)
		{
			Session[name] = value;
		}

		public object ReadApplication(string name)
		{
			return Application[name];
		}

		public void WriteApplication(string name, object value)
		{
			Application.Lock();
			Application[name] = value;
			Application.UnLock();
		}

		public string ReadCookie(string name)
		{
			HttpCookie cookie = Request.Cookies[name];
			if( cookie == null )
				return null;

			return cookie.Value;
		}

		public void WriteCookie(string name, string value)
		{
			HttpCookie cookie = new HttpCookie(name, value);
			cookie.Expires = new DateTime(2016, 1, 2);
			Response.Cookies.Add(cookie);
		}

		public string GetAppDomainPath()
		{
			return HttpRuntime.AppDomainAppPath;
		}

		public void SetResponseContentType(string contentType)
		{
			Response.ContentType = contentType;
		}


		public string GetServerMappingPath(string path)
		{
			return Server.GetMappingPath(path);
		}

		public string GetRequestMappingPath(string path)
		{
			return Request.GetMappingPath(path);
		}

		public void WriteToResponse(string text)
		{
			Response.Write(text);
		}


		public string GetInputStreamString()
		{
			StreamReader reader = new StreamReader(Request.InputStream, Request.ContentEncoding);
			Request.InputStream.Position = 0;
			return reader.ReadToEnd();
		}
	}
}
