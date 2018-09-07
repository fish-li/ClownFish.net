using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ClownFish.Base.Http;
using ClownFish.Base.Xml;


namespace ClownFish.Base.WebClient
{
	internal class ResponseReader : IDisposable
	{
		/// <summary>
		/// HTTP响应对象
		/// </summary>
		private HttpWebResponse _response;
		/// <summary>
		/// HTTP响应头中指定的编码
		/// </summary>
		private Encoding _httpHeaderEncoding;
		/// <summary>
		/// HTTP响应流
		/// </summary>
		private Stream _responseStream;
		/// <summary>
		/// 响应文本流（可能被复制为内存流）
		/// </summary>
		private Stream _textStream;
		/// <summary>
		/// 是否是HTML页面的响应
		/// </summary>
		private bool _isHtml;


		public ResponseReader(HttpWebResponse response)
		{
			if( response == null )
				throw new ArgumentNullException("response");

			_response = response;
		}

		public T Read<T>()
		{
			if( _response.Headers["Content-Encoding"] == "gzip" )
				_responseStream = new GZipStream(_response.GetResponseStream(), CompressionMode.Decompress);
			else
				_responseStream = _response.GetResponseStream();
			

			if( typeof(T) == typeof(byte[]) ) {
				// 二进制，就直接读取，忽略字符编码
				return (T)(object)GetResponseBytes();
			}
			else {
				// 其它类型的结果，先得到字符串，再做反序列化处理
				InitTextStream();

				string responseText = GetResponseText();

				// 转换结果
				return ConvertResult<T>(responseText);
			}
		}


		private MemoryStream ConvertToMemoryStream(Stream stream)
		{
			if( stream.CanSeek )
				stream.Position = 0;

			MemoryStream ms = new MemoryStream();
			byte[] buffer = new byte[1024];
			int lenght = 0;

			while( (lenght = stream.Read(buffer, 0, 1024)) > 0 )
				ms.Write(buffer, 0, lenght);


			ms.Position = 0;
			return ms;
		}
		
		private byte[] GetResponseBytes()
		{
			using( MemoryStream ms = ConvertToMemoryStream(_responseStream) ) {	
				return ms.ToArray();
			}
		}
		
		private T ConvertResult<T>(string responseText)
		{
			if( typeof(T) == typeof(string) )
				return (T)(object)responseText;


			if( _response.ContentType.IndexOfIgnoreCase(ResponseContentType.Json) >= 0 )
				return JsonExtensions.FromJson<T>(responseText);

			else if( _response.ContentType.IndexOfIgnoreCase(ResponseContentType.Xml) >= 0 )
				return XmlHelper.XmlDeserialize<T>(responseText);

			else
				return (T)Convert.ChangeType(responseText, typeof(T));
		}
		
		private void InitTextStream()
		{
			string contentType = _response.Headers["Content-Type"];
			if( string.IsNullOrEmpty(contentType) == false ) {
				_httpHeaderEncoding = GetEncodingFromHttpHeader(contentType);
				_isHtml = contentType.StartsWithIgnoreCase(ResponseContentType.Html);
			}


			if( _httpHeaderEncoding == null )
				// 增加一个【非标准的响应头】，用于特定场景下指定编码（解决未知的扩展问题）
				// 如果你认为没有必要，可以删除这二行代码！
				_httpHeaderEncoding = GetEncodingFromString(_response.Headers["Content-Charset"]);


			if( _httpHeaderEncoding == null &&		 // 如果响应头没有指定字符编码
				_isHtml &&							 // 并且是HTML的响应
				_responseStream.CanSeek == false ) { // 并且流不支持定位读

				// 这种情况下，需要根据HTML头的元数据来确定是什么编码，所以可能需要读取二次，
				// 因此就在内存中创建流的副本
				_textStream = ConvertToMemoryStream(_responseStream);
			}
			else {
				_textStream = _responseStream;
			}
		}

		private string GetResponseText()
		{
			// 如果响应头已指定编码，就按响应头的编码来读取流
			if( _httpHeaderEncoding != null )
				return ReadText(_httpHeaderEncoding);

			// HTML页面可以采用二种方式指定编码方式：HTTP响应头，HTML元数据头，所以会有二种解析方式
			if( _isHtml ) {

				// 如果响应头没有指定编码，就用 UTF-8 尝试读取（过程中会检查是否正确）
				string responseText = TryReadText(Encoding.UTF8);
				if( responseText == null )
					// 重新用HTML头中指定的编码再次读取。
					responseText = ReadText(_httpHeaderEncoding);

				return responseText;
			}


			// 非HTML的响应，就默认以 UTF-8 方式读取
			return ReadText(Encoding.UTF8);
		}


		private string TryReadText(Encoding encoding)
		{
			bool isChecked = false;
			string line = null;
			StringBuilder html = new StringBuilder();

			if( _textStream.CanSeek )
				_textStream.Position = 0;

			using( StreamReader reader = new StreamReader(_textStream, encoding, true, 1024, true) ) {
				while( (line = reader.ReadLine()) != null ) {

					html.AppendLine(line);

					if( isChecked == false && line.IndexOfIgnoreCase("</head>") >= 0 ) {
						isChecked = true;

						// 检查HTML头的元数据值
						Encoding headerEncoding = GetEncodingFromHtmlHeader(html.ToString());
						if( headerEncoding != null && headerEncoding != encoding ) {

							_httpHeaderEncoding = headerEncoding;

							// 如果HTML头指定的编码不是预期编码，就不再读取，此时需要按新的编码重新读取
							return null;							
							// 否则，继续读取
						}
					}
					// 如果HTML头中没有指定编码，那就一直读下去。
				}
			}

			return html.ToString();
		}

		private string ReadText(Encoding encoding)
		{
			if( _textStream.CanSeek )
				_textStream.Position = 0;

			using( StreamReader reader = new StreamReader(_textStream, encoding, true, 1024, true) ) {
				return reader.ReadToEnd();
			}
		}



		// <meta http-equiv="charset"  content="iso-8859-1">
		private static readonly Regex s_htmlCharsetRegex = new Regex(
					@"<meta\s+http-equiv=[\'\#]charset[\'\#]\s+content=[\'\#](?<chartset>[\w-]+)[\'\#]\s*\/?>".Replace('#', '\"'),
					RegexOptions.IgnoreCase | RegexOptions.Compiled);

		// <meta charset="utf-8">
		private static readonly Regex s_htmlCharsetRegex2 = new Regex(
					@"<meta\s+charset=[\'\#](?<chartset>[\w-]+)[\'\#]\s*\/?>".Replace('#', '\"'),
					RegexOptions.IgnoreCase | RegexOptions.Compiled);

		// <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
		private static readonly Regex s_htmlContentTypeRegex = new Regex(
					@"<meta\s+http-equiv=[\'\#]content-Type[\'\#]\s+content=[\'\#][\w\/]+;\s+charset=(?<chartset>[\w-]+)[\'\#]\s*\/?>".Replace('#', '\"'),
					RegexOptions.IgnoreCase | RegexOptions.Compiled);

		// text/html; charset=utf-8"
		private static readonly Regex s_httpHeaderContentTypeRegex = new Regex(
					@"^[\w\/]+;\s+charset=(?<chartset>[\w-]+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);



		private Encoding GetEncodingFromHtmlHeader(string text)
		{
			Match m = s_htmlCharsetRegex.Match(text);
			if( m.Success == false )
				// 再匹配一次
				m = s_htmlCharsetRegex2.Match(text);
			
			if( m.Success == false )
				// 再匹配一次
				m = s_htmlContentTypeRegex.Match(text);

			if( m.Success ) {
				string charset = m.Groups["chartset"].Value;
				return GetEncodingFromString(charset);
			}
			return null;
		}


		private Encoding GetEncodingFromHttpHeader(string contentType)
		{
			// 说明：直接使用 response.CharacterSet 不靠谱！
			//      因为如果响应头不指定编码，它就默认返回 "ISO-8859-1"，最后也不知道是不是真的是"ISO-8859-1"编码，所以干脆不用这个属性。

			Match m = s_httpHeaderContentTypeRegex.Match(contentType);
			if( m.Success ) {
				string charset = m.Groups["chartset"].Value;
				return GetEncodingFromString(charset);
			}
			return null;
		}


		private Encoding GetEncodingFromString(string encodingName)
		{
			if( string.IsNullOrEmpty(encodingName) )
				return null;

			try {
				return Encoding.GetEncoding(encodingName);
			}
			catch {
				/* 忽略无效的 charset 值 */
				return null;
			}
		}



		#region IDisposable 成员

		public void Dispose()
		{
			if( object.ReferenceEquals(_responseStream, _textStream) ) {
				if( _responseStream != null )	// 异常日志居然记录这里会有NULL引用！
					_responseStream.Dispose();

				_responseStream = null;
				_textStream = null;
			}
			else {

				if( _responseStream != null ) {
					_responseStream.Dispose();
					_responseStream = null;
				}

				if( _textStream != null ) {
					_textStream.Dispose();
					_textStream = null;
				}
			}
		}

		#endregion
	}
}
