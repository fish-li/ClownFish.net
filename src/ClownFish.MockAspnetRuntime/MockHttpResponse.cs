using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Reflection;
using System.Collections;

namespace ClownFish.MockAspnetRuntime
{
	/// <summary>
	/// 模拟HttpResponse
	/// </summary>
	public sealed class MockHttpResponse
	{
		private MockTextWriter _writer;

		public HttpResponse Response { get; private set; }

		internal MockHttpResponse()
		{
			_writer = new MockTextWriter();
			Response = new HttpResponse(_writer);			
		}

		public void EnableOutputStream()
		{
			// 为了支持在代码中使用 Response.OutputStream
			var ctor = typeof(HttpWriter).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
				null, new Type[] { typeof(HttpResponse) }, null);

			HttpWriter httpWriter = ctor.Invoke(new object[] { Response }) as HttpWriter;
			Response.SetValue("_httpWriter", httpWriter);
			Response.SetValue("_writer", httpWriter);
		}

		/// <summary>
		/// 设置或者获取响应的ContentEncoding
		/// </summary>
		public Encoding ContentEncoding
		{
			get { return _writer.StreamEncoding; }
			set { _writer.StreamEncoding = value; }
		}

		/// <summary>
		/// 获取输出流
		/// </summary>
		public Stream OutputStream
		{
			get { return _writer.Stream; }
		}
		/// <summary>
		/// 获取输出文本
		/// </summary>
		/// <returns></returns>
		public string GetText()
		{
			return _writer.GetText();
		}


		public string GetCustomHeader(string name)
		{
			ArrayList headers = Response.GetValue("_customHeaders") as ArrayList;
			foreach( var h in headers ) {
				string headerName = h.GetValue("Name") as string;
				if( name.Equals(headerName, StringComparison.OrdinalIgnoreCase) )
					return h.GetValue("Value") as string;
			}
			return null;
		}
	}
}
