namespace ClownFish.UnitTest.Http.Mock;

public class MockHttpContext : NHttpContext
{
    private readonly MockHttpRequest _request;
    private readonly MockHttpResponse _response;

    public MockHttpContext(MockRequestData requestData)
    {
        _request = new MockHttpRequest(requestData, this);
        _response = new MockHttpResponse(this);
    }


    public override object OriginalHttpContext => null;

    public override NHttpRequest Request => _request;

    public MockHttpRequest MRequest => _request;

    public override NHttpResponse Response => _response;


    public override bool SkipAuthorization { get; set; }


    public override IPrincipal User { get; set; }


    private XDictionary _items;
    public override XDictionary Items {
        get {
            if( _items == null )
                _items = new XDictionary();
            return _items;
        }
    }

}
