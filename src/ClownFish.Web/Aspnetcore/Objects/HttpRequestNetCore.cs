using ClownFish.Base.Http;
using Microsoft.Extensions.Primitives;

namespace ClownFish.Web.AspnetCore.Objects;

internal class HttpRequestNetCore : NHttpRequest
{
    private readonly HttpRequest _request;
    public override object OriginalHttpRequest => _request;

    private readonly bool _hasForm;

    public HttpRequestNetCore(HttpRequest request, NHttpContext httpContext)
        : base(httpContext)
    {
        _request = request;

        SerializeFormat format = RequestContentType.GetFormat(_request.ContentType);
        _hasForm = HttpUtils.RequestHasBody(request.Method) && (format == SerializeFormat.Form || format == SerializeFormat.Multipart);
    }

    public override bool IsHttps => _request.IsHttps;

    public override string HttpMethod => _request.Method;

    public override string RootUrl => string.Concat(
                                             _request.Scheme,
                                             "://",
                                             _request.Host.ToString());

    public override string Path => string.Concat(
                                            _request.PathBase.ToUriComponent(),
                                            _request.Path.ToUriComponent());

    public override string Query => _request.QueryString.ToUriComponent();

    // https://stackoverflow.com/questions/58614864/whats-the-difference-between-httprequest-path-and-httprequest-pathbase-in-asp-n
    public override string RawUrl => string.Concat(
                                            _request.PathBase.ToUriComponent(),
                                            _request.Path.ToUriComponent(),
                                            _request.QueryString.ToUriComponent());

    public override string ContentType => _request.ContentType;

    public override string UserAgent => _request.Headers[HttpHeaders.Request.UserAgent];

    public override Stream InputStream => _request.Body;

    public override string[] QueryStringKeys => _request.Query.Keys.ToArray();

    public override string[] FormKeys => _hasForm ? _request.Form.Keys.ToArray() : Array.Empty<string>();

    public override string[] CookieKeys => _request.Cookies.Keys.ToArray();

    public override string[] HeaderKeys => _request.Headers.Keys.ToArray();




    public override string QueryString(string name)
    {
        if( _request.Query.TryGetValue(name, out StringValues value) )
            return value.ToString();

        return null;
    }

    public override string Form(string name)
    {
        if( _hasForm == false )
            return null;

        // https://docs.microsoft.com/zh-cn/aspnet/core/performance/performance-best-practices?view=aspnetcore-6.0#prefer-readformasync-over-requestform
        // 直接访问 _request.Form 是个很低效的做法， 但是在这种接口定义下，也没其它方法了，所以建议不要调用当前方法！
        // 或者在 Action方法中增加一个 IFormCollection form 参数

        if( _request.Form.TryGetValue(name, out StringValues value) )
            return value.ToString();

        return null;
    }

    public override string Cookie(string name)
    {
        if( _request.Cookies.TryGetValue(name, out string value) )
            return value;

        return null;
    }

    public override string Header(string name)
    {
        if( _request.Headers.TryGetValue(name, out StringValues value) )
            return value.ToString();

        return null;
    }

    public override string[] GetHeaders(string name)
    {
        if( _request.Headers.TryGetValue(name, out StringValues value) )
            return value.ToArray();

        return null;
    }


}
