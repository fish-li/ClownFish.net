using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Http.Pipleline;

namespace ClownFish.WebHost.Objects
{
    internal class HttpRequestSysNet : NHttpRequest
    {
        private static readonly NameValueCollection s_empty = new NameValueCollection();

        private string _rawUrl;
        private string _path;
        private string _query;
        private NameValueCollection _queryString;
        private NameValueCollection _fromData;


        private readonly System.Net.HttpListenerRequest _request;
        public override object OriginalHttpRequest => _request;

        public HttpRequestSysNet(System.Net.HttpListenerRequest request, NHttpContext httpContext) 
            : base (httpContext)
        {
            _request = request;

            // url 之类的数据成员是一定会被访问的，所以在构造时就解析出来，方便后面写代码。
            ParseUrl();
        }


        private void ParseUrl()
        {
            _rawUrl = _request.RawUrl;

            int p = _rawUrl.IndexOf('?');
            if( p < 0 ) { // 没有查询字符串参数
                _queryString = s_empty;
                _path = System.Web.HttpUtility.UrlDecode(_rawUrl);
                _query = string.Empty;
            }
            else {
                _path = _rawUrl.Substring(0, p);
                _path = System.Web.HttpUtility.UrlDecode(_path);

                if( p == _rawUrl.Length - 1 ) {   // 问号是最后个字符，没有意义，忽略
                    _query = string.Empty;
                    _queryString = s_empty;
                }
                else {
                    _query = _rawUrl.Substring(p);      // 包含开头的 ？
                    _queryString = System.Web.HttpUtility.ParseQueryString(_query);      // 固定按UTF-8来解析
                }
            }
        }


        ///// <summary>
        ///// 获取请求体内容
        ///// </summary>
        //public override string BodyText {
        //    get {
        //        if( _body == null ) {
        //            if( _request.HasEntityBody == false ) {        // 没有请求体内容
        //                _body = null;
        //            }
        //            else {
        //                if( _request.InputStream.CanRead == false ) {
        //                    _body = string.Empty;
        //                }
        //                else {
        //                    using( StreamReader reader = new StreamReader(_request.InputStream, Encoding.UTF8, true, 1024, true) ) {
        //                        _body = reader.ReadToEnd();
        //                    }
        //                }
        //            }
        //        }
        //        return _body;                
        //    }
        //}

        private NameValueCollection ParseForm()
        {
            if( _fromData == null ) {

                string contentType = this.ContentType;

                if( contentType.IsNullOrEmpty() ) {
                    _fromData = s_empty;
                }
                else if( contentType.StartsWithIgnoreCase(RequestContentType.Form) ) {
                    _fromData = System.Web.HttpUtility.ParseQueryString(this.GetBodyText());
                }
                else {
                    // TODO: Multipart 暂不支持，以后再补充。
                    _fromData = s_empty;
                }
            }
            return _fromData;
        }

        public override bool IsHttps => _request.IsSecureConnection;

        public override string HttpMethod => _request.HttpMethod;

        public override string ContentType => _request.ContentType;

        public override string UserAgent => _request.UserAgent;

       
        public override Stream InputStream => _request.InputStream;

        public override string RootUrl => $"{_request.Url.Scheme}://{_request.Url.Authority}";

        public override string Path => _path;

        public override string Query => _query;

        public override string RawUrl => _rawUrl;

        public override string[] QueryStringKeys => _queryString.AllKeys;

        public override string QueryString(string name) => _queryString[name];


        public override string[] FormKeys => ParseForm().AllKeys;

        public override string Form(string name) => ParseForm()[name];

        
        public override string[] CookieKeys => (from x in _request.Cookies.Cast<System.Net.Cookie>() select x.Name).ToArray();

        public override string Cookie(string name) => _request.Cookies[name]?.Value;


        public override string[] HeaderKeys => _request.Headers.AllKeys;

        public override string Header(string name) => _request.Headers[name];

        public override string[] GetHeaders(string name)
        {
            return _request.Headers.GetValues(name);
        }


    }
}
