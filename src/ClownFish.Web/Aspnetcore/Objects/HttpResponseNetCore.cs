using System.Collections.Generic;

namespace ClownFish.Web.AspnetCore.Objects;

internal class HttpResponseNetCore : NHttpResponse
{
    private readonly HttpResponse _response;

    public override object OriginalHttpResponse => _response;

    public HttpResponseNetCore(HttpResponse response, NHttpContext httpContext)
        : base(httpContext)
    {
        _response = response;
    }


    public override int StatusCode {
        get => _response.StatusCode;
        set => _response.StatusCode = value;
    }

    public override bool HasStarted => _response.HasStarted;

    public override long ContentLength {
        get => _response.ContentLength ?? -1;
        set => _response.ContentLength = value;
    }

    public override Stream OutputStream => _response.Body;

    /// <summary>
    /// 这个属性对于ASP.NET CORE 来说不起作用
    /// </summary>
    public override Encoding ContentEncoding { get; set; }

    public override string ContentType {
        get => _response.ContentType;
        set => _response.ContentType = value;
    }


    public override void SetCookie2(string name, string value, DateTime? expires = null)
    {
        var httpRequest = HttpContext.Request;

        CookieOptions options = GetCookieOptions(httpRequest);
        options.Path = "/";

        if( expires != null && expires.HasValue ) {
            options.Expires = expires;
        }

        _response.Cookies.Append(name, value, options);
    }


    private static CookieOptions GetCookieOptions(NHttpRequest request)
    {
        // https://docs.microsoft.com/zh-cn/aspnet/core/security/samesite?view=aspnetcore-3.1
        // https://docs.microsoft.com/zh-cn/dotnet/core/compatibility/3.0-3.1

        // https://www.cnblogs.com/ziyunfei/p/5637945.html
        // https://www.cnblogs.com/JulianHuang/p/12218026.html

        var options = new CookieOptions {
            HttpOnly = true,
            SameSite = SameSiteMode.Unspecified
        };

        // 请求在转发时，有可能会从 https 变成 http，所以这里先检查有没有转发相关的请求头，用它来判断原始请求是不是 https
        string xfp = request.Header("X-Forwarded-Proto");

        if( xfp.IsNullOrEmpty() == false ) {
            options.Secure = xfp.Is("https");
        }
        else {
            // 如果没有转发相关的请求头，就按当前请求来处理，如果当前请求是 https 就设置 secure 标记
            options.Secure = request.IsHttps;
        }

        //options.SameSite = options.Secure 
        //					// 禁用同源策略，支持跨域访问
        //					? SameSiteMode.None

        //					// 当以http访问时，Chrome会拒绝接收 SameSiteMode.None 的 Cookie
        //					// https://www.chromestatus.com/feature/5633521622188032
        //					: SameSiteMode.Unspecified;

        return options;
    }

    public override bool SetHeader(string name, string value, bool ignoreExist)
    {
        value = value ?? string.Empty;

        if( (ignoreExist == false) || (_response.Headers.ContainsKey(name) == false) ) {
            _response.Headers.Add(name, value);
            return true;
        }
        else {
            return false;
        }
    }

    public override bool RemoveHeader(string name)
    {
        return _response.Headers.Remove(name);
    }

    public override bool SetHeaders(string name, string[] values, bool ignoreExist)
    {
        if( values.IsNullOrEmpty() )
            return false;

        if( (ignoreExist == false) || (_response.Headers.ContainsKey(name) == false) ) {
            _response.Headers.Add(name, values);
            return true;
        }
        else {
            return false;
        }
    }

    public override IEnumerable<KeyValuePair<string, IEnumerable<string>>> GetAllHeaders()
    {
        foreach( var x in _response.Headers ) {
            IEnumerable<string> values = x.Value;
            yield return new KeyValuePair<string, IEnumerable<string>>(x.Key, values);
        }
    }

    public override void ClearHeaders()
    {
        _response.Headers.Clear();
    }


    // 从 .net core 2.2 升级到 3.1 后，会遇到以下问题：
    // https://docs.microsoft.com/en-us/dotnet/core/compatibility/2.2-3.1#http-synchronous-io-disabled-in-all-servers

    // 这是MS给出的解决方案
    //var syncIOFeature = _response.HttpContext.Features.Get<IHttpBodyControlFeature>();
    //if( syncIOFeature != null ) {
    //	syncIOFeature.AllowSynchronousIO = true;
    //}

    // 也可以从全局上设置
    // https://stackoverflow.com/questions/47735133/asp-net-core-synchronous-operations-are-disallowed-call-writeasync-or-set-all
    // https://khalidabuhakmeh.com/dotnet-core-3-dot-0-allowsynchronousio-workaround


    public override void Write(byte[] buffer)
    {
        if( buffer != null && buffer.Length > 0 ) {
            _response.Body.Write(buffer, 0, buffer.Length);
        }
    }

    public override void WriteAll(byte[] buffer)
    {
        if( buffer != null && buffer.Length > 0 ) {
            _response.Headers.ContentLength = buffer.Length;
            _response.Body.Write(buffer, 0, buffer.Length);
        }
    }

    public override async Task WriteAsync(byte[] buffer)
    {
        if( buffer != null && buffer.Length > 0 ) {
            await _response.Body.WriteAsync(buffer, 0, buffer.Length);
        }
    }

    public override async Task WriteAllAsync(byte[] buffer)
    {
        if( buffer != null && buffer.Length > 0 ) {
            _response.Headers.ContentLength = buffer.Length;
            await _response.Body.WriteAsync(buffer, 0, buffer.Length);
        }
    }

    public override void Close()
    {
        // 在 ASP.NET CORE 中不需要处理
    }
}
