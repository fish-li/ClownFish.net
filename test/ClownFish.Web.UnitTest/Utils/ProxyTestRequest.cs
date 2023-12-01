using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Web.Utils;

namespace ClownFish.Web.UnitTest.Utils;

internal class ProxyTestRequest : NHttpRequest
{
    private readonly Uri _uri;


    public ProxyTestRequest(string fullUrl) : base(NullHttpContext.Instance)
    {
        _uri = new Uri(fullUrl);
    }

    public static implicit operator ProxyTestRequest(string fullUrl)
    {
        return new ProxyTestRequest(fullUrl);
    }

    public override object OriginalHttpRequest => throw new NotImplementedException();

    public override bool IsHttps => throw new NotImplementedException();

    public override string HttpMethod => throw new NotImplementedException();

    public override string RootUrl => $"{_uri.Scheme}://{_uri.Authority}";

    public override string Path => _uri.AbsolutePath;

    public override string Query => _uri.Query;

    public override string RawUrl => _uri.PathAndQuery;

    public override string ContentType => throw new NotImplementedException();

    public override string UserAgent => throw new NotImplementedException();

    public override Stream InputStream => throw new NotImplementedException();

    public override string[] QueryStringKeys => throw new NotImplementedException();

    public override string[] FormKeys => throw new NotImplementedException();

    public override string[] CookieKeys => throw new NotImplementedException();

    public override string[] HeaderKeys => throw new NotImplementedException();

    public override string Cookie(string name)
    {
        throw new NotImplementedException();
    }

    public override string Form(string name)
    {
        throw new NotImplementedException();
    }

    public override string[] GetHeaders(string name)
    {
        throw new NotImplementedException();
    }

    public override string Header(string name)
    {
        throw new NotImplementedException();
    }

    public override string QueryString(string name)
    {
        throw new NotImplementedException();
    }
}
