namespace ClownFish.Web.AspnetCore.Objects;

public sealed class HttpContextNetCore : NHttpContext
{
    private readonly HttpContext _context;
    private readonly HttpRequestNetCore _request;
    private readonly HttpResponseNetCore _response;

    public HttpContextNetCore(HttpContext context)
    {
        _context = context;

        _request = new HttpRequestNetCore(context.Request, this);
        _response = new HttpResponseNetCore(context.Response, this);
    }


    public override object OriginalHttpContext => _context;

    public override NHttpRequest Request => _request;

    public override NHttpResponse Response => _response;


    public override bool SkipAuthorization { get; set; }


    public override IPrincipal User { get; set; }
    //public override IPrincipal User {
    //	get => _context.User;
    //	set => _context.User = new System.Security.Claims.ClaimsPrincipal(value);
    //}


    private XDictionary _items;
    public override XDictionary Items {
        get {
            if( _items == null )
                _items = new XDictionary(_context.Items);
            return _items;
        }
    }

}
