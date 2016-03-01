using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Web;
using ClownFish.Base.TypeExtend;

namespace ClownFish.Web
{
	/// <summary>
	/// 包含流的Action执行结果，通常用于实现文件下载。
	/// </summary>
	public sealed class StreamResult : IActionResult
	{
		private byte[] _buffer;
		private string _contentType;
		private string _filename;

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="buffer">文件内容的字节数组</param>
		public StreamResult(byte[] buffer)
			: this(buffer, null)
		{
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="buffer">文件内容的字节数组</param>
		/// <param name="contentType">文档类型，允许为空</param>
		public StreamResult(byte[] buffer, string contentType)
			: this(buffer, contentType, null)
		{
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="buffer">文件内容的字节数组</param>
		/// <param name="contentType">文档类型，允许为空</param>
		/// <param name="filename">下载对话框显示的文件名</param>
		public StreamResult(byte[] buffer, string contentType, string filename)
		{
			if( buffer == null || buffer.Length == 0 )
				throw new ArgumentNullException("buffer");

			_buffer = buffer;
			_filename = filename;


			if( string.IsNullOrEmpty(contentType) )
				_contentType = "application/octet-stream";
			else
				_contentType = contentType;
		}

		/// <summary>
		/// 实现IActionResult接口，执行输出
		/// </summary>
		/// <param name="context"></param>
		public void Ouput(HttpContext context)
		{
			context.Response.ContentType = _contentType;

			if( string.IsNullOrEmpty(_filename) == false ) {

				// 文件名编码这块不知道未来会不会有问题，
				// 为了便于以后可以快速改进编码问题，且不修改这里的代码，这里定义一个类型和虚方法留着未来去重写。

				DownloadFileNameEncoder encoder = ObjectFactory.New<DownloadFileNameEncoder>();

				string headerValue = encoder.GetFileNameHeader(context, _filename);

				if( string.IsNullOrEmpty(headerValue)  == false )
					context.Response.AddHeader("Content-Disposition", headerValue);
			}

			context.Response.OutputStream.Write(_buffer, 0, _buffer.Length);
		}

	}





	/// <summary>
	/// 计算用于下载文件的编码工具类
	/// </summary>
	public class DownloadFileNameEncoder
	{
		/// <summary>
		/// 根据指定的文件名，按照HTTP相关规范计算用于响应头可以接受的字符串
		/// </summary>
		/// <param name="context"></param>
		/// <param name="filename"></param>
		/// <returns></returns>
		public virtual string GetFileNameHeader(HttpContext context, string filename)
		{
			// 参考：
			// http://www.iefans.net/xiazai-wenjian-http-bianma-content-disposition/

			// http://greenbytes.de/tech/webdav/draft-reschke-rfc2231-in-http-latest.html
			//    3.2 Parameter Value Character Set and Language Information

			// Safari 不识别下面的编码方式
			//string headerValue = "attachment; filename*=UTF-8''" + HttpUtility.UrlEncode(filename);

			// 文件名的编码不能使用 HttpUtility.UrlEncode
			// 因为 HTTP 规范 RFC 2231 中的不转义字符和 HttpUtility.UrlEncode 不转义字符的范围不一样，
			// 可对比 HttpUtility.IsSafe 和 IsSkipChar 方法

			string headerValue = null;

			if( context.Request.Browser.Browser == "IE" )		// 老版本IE
				headerValue = string.Format("attachment; filename=\"{0}\"", EncodeFileName(filename));

			else if( context.Request.Browser.Browser == "Safari" ) {
				headerValue = string.Format("attachment; filename=\"{0}\"", filename);

				// Safari 这货不支持UTF-8编码标准，只能直接使用原文
				// 但是，直接输出原文不是标准的做法，只是这货支持而已，
				// 这样处理后，代理又拿到错误的结果（已被强行转义），所以再增加一个符合规范字符范围的响应头。

				// 也有可能是我没找到更有效的方法，如果看到注释的你有更好的解决方法，请告诉我：liqifeng0503@163.com ，谢谢。

				if( context.Request.Headers[Proxy.ProxyTransferHandler.ProxyFlagHeader] != null )
					// 反向代理发出的请求，这里就直接调用UrlEncode，因为在那边还要调用 UrlDecode来还原
					context.Response.AddHeader("X-Content-Disposition-proxy", HttpUtility.UrlEncode(filename));
			}

			else  // 符合新标准的浏览器（部分特殊字符仍然有问题，汉字没问题）
				headerValue = "attachment; filename*=UTF-8''" + EncodeFileName(filename);


			return headerValue;
		}


		private string EncodeFileName(string filename)
		{
			StringBuilder sb = new StringBuilder();

			byte[] bytes = Encoding.UTF8.GetBytes(filename);

			foreach( byte b in bytes ) {
				if( IsSkipChar((char)b) ) {
					sb.Append((char)b);
				}
				else {
					sb.Append('%');
					sb.Append(IntToHex(b >> 4 & 15));
					sb.Append(IntToHex(b & 15));
				}
			}

			return sb.ToString();
		}

		private char IntToHex(int n)
		{
			if( n <= 9 )
				return (char)(n + 48);

			return (char)(n - 10 + 97);
		}

		private bool IsSkipChar(char ch)
		{
			if( (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9') )
				return true;


			switch( ch ) {
				case '-':
				case '.':
				case '_':
				case '~':
				case ':':
				case '!':
				case '$':
				case '&':
				case '+':
					return true;
			}

			return false;
		}
	}


}
