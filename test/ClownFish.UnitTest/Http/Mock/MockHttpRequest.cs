using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Http.Pipleline;

namespace ClownFish.UnitTest.Http.Mock
{
    public class MockHttpRequest : NHttpRequest
    {
        private readonly MockRequestData _requestData;
        private readonly NameValueCollection _queryString;
        private readonly NameValueCollection _form;
        private readonly NameValueCollection _cookies;

        internal MockHttpRequest(MockRequestData requestData, MockHttpContext context) : base(context)
        {
            _requestData = requestData;
            if( requestData.Headers == null )
                requestData.Headers = new NameValueCollection();

            _queryString = System.Web.HttpUtility.ParseQueryString(requestData.Url.Query);

            if( requestData.Body != null ) {
                string bodyText = Encoding.UTF8.GetString(requestData.Body);
                _form = System.Web.HttpUtility.ParseQueryString(bodyText);
            }
            else {
                _form = new NameValueCollection();
            }

            _cookies = new NameValueCollection();
            string cookieText = requestData.GetHeader("cookie");

            if( cookieText.IsNullOrEmpty() == false ) {
                cookieText.ToKVList(';', '=').ForEach(x => _cookies.Add(x.Name, x.Value));
            }
        }


        public static implicit operator MockHttpRequest(MockRequestData requestData)
        {
            MockHttpContext context = new MockHttpContext(requestData);
            return context.MRequest;
        }

        public static MockHttpRequest FromRequestText(string rawText)
        {
            MockRequestData requestData = MockRequestData.FromText(rawText);
            MockHttpContext context = new MockHttpContext(requestData);
            return context.MRequest;
        }

        public override object OriginalHttpRequest => null;

        public override bool IsHttps => _requestData.Url.Scheme == "https";

        public override string HttpMethod => _requestData.HttpMethod;

        public override string RootUrl => string.Concat(
                                                 _requestData.Url.Scheme,
                                                 "://",
                                                 _requestData.Url.Authority);

        public override string Path => _requestData.Url.AbsolutePath;

        public override string Query => _requestData.Url.Query;

        public override string RawUrl => _requestData.Url.PathAndQuery;

        public override string ContentType => _requestData.GetHeader(HttpHeaders.Request.ContentType);

        public override string UserAgent => _requestData.GetHeader(HttpHeaders.Request.UserAgent);

        public override Stream InputStream => _requestData.InputStream;

        public override string[] QueryStringKeys => _queryString.AllKeys;

        public override string[] FormKeys => _form.AllKeys;

        public override string[] CookieKeys => _cookies.AllKeys;

        public override string[] HeaderKeys => _requestData.Headers.AllKeys;

        public override string Cookie(string name) => _cookies.Get(name);

        public override string Form(string name) => _form.Get(name);


        public override string[] GetHeaders(string name) => _requestData.Headers.GetValues(name);

        public override string Header(string name) => _requestData.Headers.Get(name);

        public override string QueryString(string name) => _queryString.Get(name);
    }
}
