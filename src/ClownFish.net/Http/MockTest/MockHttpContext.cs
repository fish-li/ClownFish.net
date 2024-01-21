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


    private XDictionary _items;
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override XDictionary Items {
        get {
            if( _items == null )
                _items = new XDictionary();
            return _items;
        }
    }

}
