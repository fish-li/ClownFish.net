namespace ClownFish.Web.Aspnetcore.Objects;


/// <summary>
/// NHttpContext的ASP.NETCORE实现
/// </summary>
public sealed class HttpContextNetCore : NHttpContext
{
    private readonly HttpContext _context;
    private readonly HttpRequestNetCore _request;
    private readonly HttpResponseNetCore _response;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="context"></param>
    public HttpContextNetCore(HttpContext context)
    {
        _context = context;

        _request = new HttpRequestNetCore(context.Request, this);
        _response = new HttpResponseNetCore(context.Response, this);
    }

    /// <summary>
    /// 原始的HttpContext对象
    /// </summary>
    public override object OriginalHttpContext => _context;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override NHttpRequest Request => _request;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override NHttpResponse Response => _response;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override bool SkipAuthorization { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override IPrincipal User { get; set; }
    //public override IPrincipal User {
    //	get => _context.User;
    //	set => _context.User = new System.Security.Claims.ClaimsPrincipal(value);
    //}


    
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override object TryGetCtxItem(string key)
    {
        return _context.Items.TryGetValue(key, out var value) ? value : null;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void SetCtxItem(string key, object value)
    {
        _context.Items[key] = value;
    }
}
