namespace ClownFish.WebHost.Objects;

public sealed class HttpContextSysNet : NHttpContext
{
    private readonly System.Net.HttpListenerContext _context;
    private readonly HttpRequestSysNet _request;
    private readonly HttpResponseSysNet _response;

    public HttpContextSysNet(System.Net.HttpListenerContext context)
    {
        _context = context;

        _request = new HttpRequestSysNet(context.Request, this);
        _response = new HttpResponseSysNet(context.Response, this);
    }


    public override object OriginalHttpContext => _context;


    public override NHttpRequest Request => _request;

    public override NHttpResponse Response => _response;


    public override bool SkipAuthorization { get; set; }

    public override IPrincipal User { get; set; }


    private Dictionary<string, object> _items;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override object TryGetCtxItem(string key)
    {
        if( _items == null )
            return null;

        return _items.TryGetValue(key, out var value) ? value : null;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void SetCtxItem(string key, object value)
    {
        if( _items == null )
            _items = new Dictionary<string, object>();

        _items[key] = value;
    }


}
