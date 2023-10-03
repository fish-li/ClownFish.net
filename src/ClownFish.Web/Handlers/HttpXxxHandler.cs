namespace ClownFish.Web.Handlers;
public sealed class HttpXxxHandler : IAsyncNHttpHandler
{
    private readonly int _statusCode;
    private readonly string _content;
    private readonly string _contentType;
    private readonly HttpResult<string> _httpResult;

    public HttpXxxHandler(int statusCode, string content, string contentType)
    {
        _statusCode = statusCode;
        _content = content;
        _contentType = contentType;
    }

    public HttpXxxHandler(HttpResult<string> httpResult)
    {
        _httpResult = httpResult;
    }

    public async Task ProcessRequestAsync(NHttpContext httpContext)
    {
        if( _httpResult == null ) {
            await httpContext.HttpReplyAsync(_statusCode, _content, _contentType);
        }
        else {
            await httpContext.HttpReplyAsync(_httpResult);
        }
    }
}
