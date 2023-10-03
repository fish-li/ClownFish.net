namespace ClownFish.Web.Handlers;
public sealed class StaticFileHandler : IAsyncNHttpHandler
{
    private readonly string _filePath;
    private readonly string _contentType;
    private readonly int _cacheMaxAge;

    public StaticFileHandler(string filePath, string contentType, int cacheMaxAge = 2592000)
    {
        if( filePath.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(filePath));
        if( File.Exists(filePath) == false )
            throw new FileNotFoundException($"file [{filePath}] not found!");

        if( contentType.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(contentType));
        

        _filePath = filePath;
        _contentType = contentType;
        _cacheMaxAge = cacheMaxAge;
    }

    public async Task ProcessRequestAsync(NHttpContext httpContext)
    {
        NHttpResponse response = httpContext.Response;
        response.StatusCode = 200;
        response.ContentType = _contentType;

        if( _cacheMaxAge > 0 ) {
            if( _cacheMaxAge == 2592000 )
                response.SetHeader("Cache-Control", "public, max-age=2592000", true);
            else
                response.SetHeader("Cache-Control", $"public, max-age={_cacheMaxAge}", true);
        }
        else if( _cacheMaxAge == -1 ) {
            response.SetHeader("Cache-Control", "private", true);
        }

        byte[] fileBytes = File.ReadAllBytes(_filePath);
        await response.WriteAllAsync(fileBytes);
    }
}
