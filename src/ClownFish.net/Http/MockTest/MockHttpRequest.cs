namespace ClownFish.Http.MockTest;

/// <summary>
/// 
/// </summary>
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

        string contentType = requestData.GetHeader(HttpHeaders.Request.ContentType);

        if( requestData.Body != null && contentType.HasValue() && contentType.StartsWith0(RequestContentType.Form) ) {
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="requestData"></param>
    public static implicit operator MockHttpRequest(MockRequestData requestData)
    {
        MockHttpContext context = new MockHttpContext(requestData);
        return context.MRequest;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rawText"></param>
    /// <returns></returns>
    public static MockHttpRequest FromRequestText(string rawText)
    {
        MockRequestData requestData = MockRequestData.FromText(rawText);
        MockHttpContext context = new MockHttpContext(requestData);
        return context.MRequest;
    }


    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override object OriginalHttpRequest => null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override bool IsHttps => _requestData.Url.Scheme == "https";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string HttpMethod => _requestData.HttpMethod;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string RootUrl => string.Concat(
                                             _requestData.Url.Scheme,
                                             "://",
                                             _requestData.Url.Authority);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string Path => _requestData.Url.AbsolutePath;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string Query => _requestData.Url.Query;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string RawUrl => _requestData.Url.PathAndQuery;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string ContentType => _requestData.GetHeader(HttpHeaders.Request.ContentType);

    /// <summary>
    /// 
    /// </summary>
    public long BodyLength => _requestData.Body?.Length ?? -1;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string UserAgent => _requestData.GetHeader(HttpHeaders.Request.UserAgent);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override Stream InputStream => _requestData.InputStream;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string[] QueryStringKeys => _queryString.AllKeys;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string[] FormKeys => _form.AllKeys;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string[] CookieKeys => _cookies.AllKeys;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string[] HeaderKeys => _requestData.Headers.AllKeys;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string Cookie(string name) => _cookies.Get(name);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string Form(string name) => _form.Get(name);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string[] GetHeaders(string name) => _requestData.Headers.GetValues(name);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string Header(string name) => _requestData.Headers.Get(name);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string QueryString(string name) => _queryString.Get(name);
}
