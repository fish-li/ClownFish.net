using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace ClownFish.Base.WebClient
{
	/// <summary>
	/// 在HTTP调用时发生的远程服务端异常。
	/// 这个异常类型解决了二个问题：
	/// 1、WebException异常消息Message太笼统，很不友好。
	/// 2、Response属性的页面编码不一致（ASP.NET 采用UTF-8，IIS采用GB2312），导致获取异常页面时乱码问题。
	/// </summary>
	[Serializable]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2240:ImplementISerializableCorrectly")]
	public sealed class RemoteWebException : System.Exception
	{
		private string _message;

		/// <summary>
		/// 服务端返回的响应内容（可能为空）
		/// </summary>
		public string ResponseText { get; private set; }

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="ex"></param>
		public RemoteWebException(WebException ex)
			: base(ex.Message, ex)
		{
			if( ex == null )
				throw new ArgumentNullException("ex");

			ResponseText = TryReadResponseText(ex);

			if( ResponseText != null )
				_message = GetHtmlTitle(ResponseText);
		}



		/// <summary>
		/// 尝试从WebException实例中读取服务端响应文本
		/// </summary>
		/// <param name="ex">WebException的实例</param>
		/// <returns>异常的描述信息，通常是一段HTML代码</returns>
		private string TryReadResponseText(WebException ex)
		{
			if( ex.Response == null )
				return null;

			HttpWebResponse response = ex.Response as HttpWebResponse;

			if( response == null )
				return null;
			else {
				using( response ) {
					using( ResponseReader reader = new ResponseReader(response) ) {
						return reader.Read<string>();
					}
				}
			}
		}

		/// <summary>
		/// 尝试从一段HTML代码中读取文档标题部分
		/// </summary>
		/// <param name="text">HTML代码</param>
		/// <returns>文档标题</returns>
		internal static string GetHtmlTitle(string text)
		{
			if( string.IsNullOrEmpty(text) )
				return null;

			int p1 = text.IndexOfIgnoreCase("<title>");
			int p2 = text.IndexOfIgnoreCase("</title>");

			if( p2 > p1 && p1 > 0 ) {
				p1 += "<title>".Length;
				return text.Substring(p1, p2 - p1);
			}

			return null;
		}


		/// <summary>
		/// 异常的简单描述
		/// </summary>
		public override string Message
		{
			get
			{
				return  _message ?? base.Message;
			}
		}


		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private RemoteWebException(SerializationInfo info, StreamingContext context)
			: base(info, context)
        {
        }

	}
}
