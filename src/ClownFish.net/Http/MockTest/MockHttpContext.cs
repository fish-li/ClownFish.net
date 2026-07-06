namespace ClownFish.Http.MockTest;

/// <summary>
/// MockHttpContext
/// </summary>
public class MockHttpContext : NHttpContext
{
    private readonly MockHttpRequest _request;
    private readonly MockHttpResponse _response;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="requestData"></param>
    public MockHttpContext(MockRequestData requestData)
    {
        if( requestData == null )
            throw new ArgumentNullException(nameof(requestData));

        _request = new MockHttpRequest(requestData, this);
        _response = new MockHttpResponse(this);
    }


    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override object OriginalHttpContext => null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override NHttpRequest Request => _request;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public MockHttpRequest MRequest => _request;

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
