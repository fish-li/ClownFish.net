using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
	public sealed class RemoteWebException : System.Exception
	{
		private string _message;

        /// <summary>
        /// 发生异常时的调用网址
        /// </summary>
        public string Url { get; private set; }

		/// <summary>
		/// 服务端返回的响应内容（可能为NULL）
		/// </summary>
		public string ResponseText { get; private set; }

        /// <summary>
        /// 响应头集合（可能为NULL）
        /// </summary>
        public NameValueCollection Headers { get; private set; }

        /// <summary>
        /// 异常的简单描述
        /// </summary>
        public override string Message {
            get {
                return (_message ?? base.Message)
                        + (string.IsNullOrEmpty(Url) ? string.Empty : ("\r\n当前调用网址：" + this.Url));
                    }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="url"></param>
        public RemoteWebException(WebException ex, string url)
            : base(ex.Message, ex)
        {
            if( ex == null )
                throw new ArgumentNullException("ex");

            this.Url = url;
            ReadResponse(ex);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ex"></param>
        public RemoteWebException(WebException ex)
			: this(ex, null)
		{
		}
        

		/// <summary>
		/// 尝试从WebException实例中读取服务端响应内容
		/// </summary>
		/// <param name="ex">WebException的实例</param>
		private void ReadResponse(WebException ex)
		{
			if( ex.Response == null )
				return;

			HttpWebResponse response = ex.Response as HttpWebResponse;
			if( response == null )
				return;


            using( response ) {
                using( ResponseReader reader = new ResponseReader(response) ) {
                    this.ResponseText = reader.Read<string>();
                }

                // 读取响应头
                this.Headers = new NameValueCollection();
                this.Headers.Add(response.Headers);

                // 确定错误消息
                if( string.IsNullOrEmpty(this.ResponseText) == false ) {
                    if( response.ContentType?.IndexOf("text/plain") == 0 ) {
                        _message = this.ResponseText;
                    }
                    else if( response.ContentType?.IndexOf("text/html") == 0 ) {
                        _message = GetHtmlTitle(this.ResponseText);
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
				return "服务器错误：" +  text.Substring(p1, p2 - p1);
			}

			return null;
		}


		/// <summary>
		/// 用序列化数据初始化 RemoteWebException 类的新实例
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private RemoteWebException(SerializationInfo info, StreamingContext context)
			: base(info, context)
        {
			this._message = info.GetString("_message");
			this.ResponseText = info.GetString("ResponseText");
            this.Url = info.GetString("Url");
        }

		/// <summary>
		/// 重写方法，用关于异常的信息设置
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("_message", this._message, typeof(string));
			info.AddValue("ResponseText", this.ResponseText, typeof(string));
            info.AddValue("Url", this.Url, typeof(string));
        }

	}
}
