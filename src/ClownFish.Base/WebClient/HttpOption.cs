using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Http;


namespace ClownFish.Base.WebClient
{
	/// <summary>
	/// 表示一次HTTP请求的描述信息
	/// </summary>
	public sealed class HttpOption
	{
        private HttpHeaderCollection _headers;

        /// <summary>
        /// 构造方法
        /// </summary>
        public HttpOption()
		{
			_method = "GET";
			Format = SerializeFormat.Form;
        }
        

        /// <summary>
        /// URL地址（建议查询字符串参数在Data属性中指定，此处只指定文件路径即可）
        /// </summary>
        public string Url { get; set; }
		


		private string _method;
		/// <summary>
		/// HTTP请求的方法，例如： GET, POST
		/// </summary>
		public string Method
		{
			get { return _method; }
			set
			{
				if( string.IsNullOrEmpty(value) )
					throw new ArgumentNullException("value");
				_method = value.ToUpper();
			}
		}

        /// <summary>
        /// 请求头集合。
        /// 允许为当前属性指定一个 Dictionary《string, string》 类型的变量
        /// </summary>
        public HttpHeaderCollection Headers {
            get {
                if( _headers == null )
                    _headers = new HttpHeaderCollection();
                return _headers;
            }
            set {
                if( value == null )
                    throw new ArgumentNullException(nameof(value));

                _headers = value;
            }
        }


        /// <summary>
        /// 这个属性只能赋值，等同于给 Headers 属性赋值。差别在于这二个属性的类型不一样。
        /// 建议：给当前属性指定一个 匿名对象。属性名做为请求头的 NAME，值做为VALUE。
        /// 说明：如果属性名中包含【下划线】，生成的请求头中将变成【中横线】 例如：prefix_name =》 "prefix-name"
        /// </summary>
        public object Header {
            // 定义一个【只写属性】不是好的设计方式！
            // 这里没有办法，因为 C# 编译器不允许 从 object 到 HttpHeaderCollection 的类型转换，所以不能沿用 Headers ，只能再定义一个变量

            set {
                if( value == null )
                    throw new ArgumentNullException(nameof(value));

                _headers = HttpHeaderCollection.create(value);
            }
        }
        

        /// <summary>
        /// 需要提交的数据（与 $.ajax()方法的 Data 属性含义类似），
        /// 可指定一个FormDataCollection实例，或者一个 IDictionary实例，或者一个匿名对象实例
        /// 如果是GET请求，数据会自动转变成查询字参数，如果是POST，则随请求体发送
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 数据的序列化方式。相当于指定 Content-Type 请求头。
        /// 注意：不包含请求体的请求，不需要指定这个属性，例如：GET , HEAD
        /// </summary>
        public SerializeFormat Format { get; set; }

        // 不使用 ContentType 的原因有三点：
        // 1，ContentType 是个【长】字符串，容易写错，
        // 2，Json, Json2 这样的序列列没法表达
        // 3，限制范围，只允许枚举定义的几种取值



        /// <summary>
        /// 在发送请求时指定 User-Agent 头
        /// </summary>
        public string UserAgent { get; set; }

		/// <summary>
		/// 禁止自动重定向
		/// </summary>
		public bool DisableAutoRedirect { get; set; }

		/// <summary>
		/// Cookie容器
		/// </summary>
		public CookieContainer Cookie { get; set; }


		/// <summary>
		/// 直接指定要发送什么 COOKIE，通常用于不需要接收Cookier场景
		/// 注意：
		/// 1、如果需要接收Cookie，请设置 Cookie 属性，
		/// 2、cookieHeader的数据需要自行编码
		/// </summary>
		/// <param name="cookieHeader">要发送的COOKIE头内容</param>
		public HttpOption SetCookieHeader(string cookieHeader)
		{
			this.Headers.Add("Cookie", cookieHeader);
            return this;
		}

		/// <summary>
		/// 获取或设置请求的身份验证信息。
		/// </summary>
		public ICredentials Credentials { get; set; }


		/// <summary>
		/// 获取或设置 GetResponse 和 GetRequestStream 方法的超时值（以毫秒为单位）。
		/// </summary>
		public int? Timeout { get; set; }


		/// <summary>
		/// 指定一个委托，用于在发送请求前设置HttpWebRequest的其它属性
		/// </summary>
		public Action<HttpWebRequest> SetRequestAction { get; set; }
		

		/// <summary>
		/// 指定一个委托，用于在请求接收后调用，可获取请求头相关信息
		/// </summary>
		public Action<HttpWebResponse> ReadResponseAction { get; set; }



        /// <summary>
        /// 检查传入的属性是否存在冲突的设置
        /// </summary>
        internal void CheckInput()
		{
			if( string.IsNullOrEmpty(this.Url) )
				throw new ArgumentNullException("Url");

			//if( (Method == "GET" || Method == "HEAD") && Format != SerializeFormat.Form )
			//	throw new InvalidOperationException("GET, HEAD 请求只能采用 FORM 序列化方式。");
		}


        /// <summary>
		/// 根据Method属性，返回是不是必须以查询字符串形式提交数据
		/// </summary>
		private bool IsMustQueryString()
        {
            return RequestHasBody(this.Method) == false;
        }

        /// <summary>
        /// 根据一个请求的提交方法，判断是否包含请求体
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static bool RequestHasBody(string method)
        {
            if( string.IsNullOrEmpty(method) )
                throw new ArgumentNullException(nameof(method));

            method = method.ToUpper();

            // 参考 Fiddler 的判断规则
            if( method == "GET"
                || method == "HEAD"
                || method == "TRACE"
                || method == "DELETE"
                || method == "CONNECT"
                || method == "MKCOL"
                || method == "COPY"
                || method == "MOVE"
                || method == "UNLOCK"
                || method == "OPTIONS"
                )
                return false;
            else
                return true;
        }

        /// <summary>
        /// 获取实际的请求址。
        /// 如果是GET请求，将会包含提交数据。
        /// </summary>
        /// <returns></returns>
        internal string GetRequestUrl()
        {
            string requestUrl = this.Url;

            // 如果有提交数据，并且是 GET 请求，就需要将参数合并到URL，形成查询字符串参数
            if( this.Data != null && IsMustQueryString() ) {
                if( this.Url.IndexOf('?') < 0 )
                    requestUrl = this.Url + "?" + GetQueryString(this.Data);
                else
                    requestUrl = this.Url + "&" + GetQueryString(this.Data);
            }

            return requestUrl;
        }

        /// <summary>
        /// 获取需要提交的数据。
        /// 如果已指定要提交的数据，但是是GET请求，那么也认为是没有提交数据。
        /// </summary>
        /// <returns></returns>
        internal object GetPostData()
        {
            if( this.Data != null && IsMustQueryString() == false )
                return this.Data;
            else
                return null;
        }


        /// <summary>
		/// 生成查询字符串参数
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private static string GetQueryString(object data)
        {
            if( data == null )
                return null;

            if( data.GetType() == typeof(string) )
                return (string)data;


            FormDataCollection form = FormDataCollection.Create(data);
            return form.ToString();
        }

        /// <summary>
        /// 根据原始请求信息文本构建 HttpOption 对象（格式可参考Fiddler的Inspectors标签页内容）
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static HttpOption FromRawText(string text)
		{
            // text参数的 示例数据：
            //POST http://www.fish-web-demo.com/api/ns/TestAutoAction/submit.aspx HTTP/1.1
            //Host: www.fish-web-demo.com
            //User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; rv:36.0) Gecko/20100101 Firefox/36.0
            //Accept: */*
            //Accept-Language: zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3
            //Accept-Encoding: gzip, deflate
            //Content-Type: application/x-www-form-urlencoded; charset=UTF-8
            //X-Requested-With: XMLHttpRequest
            //Referer: http://www.fish-web-demo.com/Pages/Demo/TestAutoFindAction.htm
            //Content-Length: 72
            //Cookie: hasplmlang=_int_; LoginBy=productKey; PageStyle=Style2;
            //Connection: keep-alive
            //Pragma: no-cache
            //Cache-Control: no-cache

            //input=Fish+Li&Base64=%E8%BD%AC%E6%8D%A2%E6%88%90Base64%E7%BC%96%E7%A0%81

            if( string.IsNullOrEmpty(text) )
				throw new ArgumentNullException("text");

			HttpOption option = new HttpOption();
            // 放弃构造方法中的默认值格式，因为请求头中可能会指定
            option.Format = SerializeFormat.None;


            using( StringReader reader = new StringReader(text.Trim()) ) {
				string firstLine = reader.ReadLine();

				int p1 = firstLine.IndexOf(' ');
				int p2 = firstLine.LastIndexOf(' ');

				if( p1 < 0 || p1 == p2 )
					throw new ArgumentException("不能识别的请求文本格式。");

                // 设置请求方法，GET OR POST
				option.Method = firstLine.Substring(0, p1);
                

				// 不使用HTTP协议版本，只做校验。
				string httpVersion = firstLine.Substring(p2 + 1);
				if( httpVersion.StartsWith("HTTP/") == false )
					throw new ArgumentException("不能识别的请求文本格式。");

				option.Url = firstLine.Substring(p1 + 1, p2 - p1 - 1);

				string line = null;
				while( (line = reader.ReadLine()) != null ) {
					if( line.Length > 0 ) {
						// 处理请求头
						int p3 = line.IndexOf(':');
						if( p3 > 0 ) {
							string name = line.Substring(0, p3);

                            if( name.EqualsIgnoreCase("Content-Length") )   // 这个头可以直接丢弃
                                continue;

							string value = line.Substring(p3 + 2);	// 2 表示2个字符，一个冒号，一个空格
							option.Headers.Add(name, value);
						}
						else
							throw new ArgumentException("不能识别的请求文本格式。");
					}
					else
						break;
				}

				// 请求体数据
				string postText = reader.ReadToEnd();
				if( string.IsNullOrEmpty(postText) == false )
					option.Data = postText;
			}

            string contentType = option.Headers["Content-Type"];
            if( contentType != null ) {
                // 可能的格式：Content-Type: application/x-www-form-urlencoded; charset=UTF-8
                // 此时，只需要获取 "application/x-www-form-urlencoded"

                int p = contentType.IndexOf(';');                
                if( p > 0 ) {
                    // 注意：这里丢弃了 charset 设置，因为 HttpClient 固定以 utf-8 编码方式发送请求！
                    option.Headers["Content-Type"] = contentType.Substring(0, p);
                }
            }
            else {
                // 为了兼容以前的测试用例：当时为了简单，在POST时没有指定Content-Type
                if( option.Data != null )
                    option.Format = SerializeFormat.Form;
            }


            return option;
		}
	}
}
