using ClownFish.Http.Proxy;

namespace ClownFish.Http.Utils;

/// <summary>
/// HttpContext 相关的扩展方法工具类
/// </summary>
public static partial class HttpContextExtensions
{
    /// <summary>
    /// 响应HTTP请求
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="body"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    public static void HttpReply(this NHttpContext httpContext, string body, string contentType = null)
    {
        HttpReply(httpContext, 200, body, contentType);
    }


    /// <summary>
    /// 响应HTTP请求
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="body"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    public static Task HttpReplyAsync(this NHttpContext httpContext, string body, string contentType = null)
    {
        return HttpReplyAsync(httpContext, 200, body, contentType);
    }


    /// <summary>
    /// 响应HTTP请求
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="statusCode"></param>
    /// <param name="body"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    public static void HttpReply(this NHttpContext httpContext, int statusCode, string body, string contentType = null)
    {
        if( httpContext == null )
            throw new ArgumentNullException(nameof(httpContext));

        if( string.IsNullOrEmpty(body) ) {
            httpContext.Response.StatusCode = 204;
        }
        else {
            NHttpResponse response = httpContext.Response;
            response.StatusCode = statusCode;
            response.ContentType = contentType ?? ResponseContentType.TextUtf8;

            response.WriteAll(body.GetBytes());
            httpContext.PipelineContext.RespResult = body;
        }
    }


    /// <summary>
    /// 响应HTTP请求
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="statusCode"></param>
    /// <param name="body"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    public static async Task HttpReplyAsync(this NHttpContext httpContext, int statusCode, string body, string contentType = null)
    {
        if( httpContext == null )
            throw new ArgumentNullException(nameof(httpContext));

        if( string.IsNullOrEmpty(body) ) {
            httpContext.Response.StatusCode = 204;
        }
        else {
            NHttpResponse response = httpContext.Response;
            response.StatusCode = statusCode;
            response.ContentType = contentType ?? ResponseContentType.TextUtf8;

            await response.WriteAllAsync(body.GetBytes());
            httpContext.PipelineContext.RespResult = body;
        }
    }


    /// <summary>
    /// 响应HTTP请求
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="statusCode"></param>
    /// <param name="stream"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    public static async Task HttpReplyAsync(this NHttpContext httpContext, int statusCode, Stream stream, string contentType = null)
    {
        if( httpContext == null )
            throw new ArgumentNullException(nameof(httpContext));

        if( stream == null )
            throw new ArgumentNullException(nameof(stream));

        if( stream.CanRead == false )
            throw new InvalidOperationException("流不可读！");

        if( stream.CanSeek && stream.Length == 0 ) {
            httpContext.Response.StatusCode = 204;
            return;
        }

        NHttpResponse response = httpContext.Response;
        response.StatusCode = statusCode;
        response.ContentType = contentType ?? ResponseContentType.OctetStream;

        if( stream.CanSeek ) {
            response.ContentLength = stream.Length;
        }

        await stream.CopyToAsync(response.OutputStream);
    }


    /// <summary>
    /// 响应HTTP请求
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="statusCode"></param>
    /// <param name="data"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    public static async Task HttpReplyAsync(this NHttpContext httpContext, int statusCode, byte[] data, string contentType = null)
    {
        if( httpContext == null )
            throw new ArgumentNullException(nameof(httpContext));

        if( data == null )
            throw new ArgumentNullException(nameof(data));

        if( data.Length == 0 ) {
            httpContext.Response.StatusCode = 204;
        }
        else {
            NHttpResponse response = httpContext.Response;
            response.StatusCode = statusCode;
            response.ContentType = contentType ?? ResponseContentType.OctetStream;

            await response.WriteAllAsync(data);
        }
    }


    /// <summary>
    /// 响应HTTP请求
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="statusCode"></param>
    /// <param name="body"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    public static async Task HttpGzipReplyAsync(this NHttpContext httpContext, int statusCode, string body, string contentType = null)
    {
        if( httpContext == null )
            throw new ArgumentNullException(nameof(httpContext));

        if( string.IsNullOrEmpty(body) ) {
            httpContext.Response.StatusCode = 204;
        }
        else {
            NHttpResponse response = httpContext.Response;
            response.StatusCode = statusCode;
            response.ContentType = contentType ?? ResponseContentType.TextUtf8;

            response.SetHeader(HttpHeaders.Response.ContentEncoding, "gzip");

            byte[] data = Encoding.UTF8.GetBytes(body);

            using( GZipStream gZipStream = new GZipStream(response.OutputStream, CompressionMode.Compress, true) ) {
                await gZipStream.WriteAsync(data, 0, data.Length);
                gZipStream.Close();
            }
        }
    }

    /// <summary>
    /// 响应HTTP请求
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="httpResult">做为输出的数据对象</param>
    /// <returns></returns>
    public static async Task HttpReplyAsync(this NHttpContext httpContext, HttpResult<string> httpResult)
    {
        if( httpContext == null )
            throw new ArgumentNullException(nameof(httpContext));
        if( httpResult == null )
            throw new ArgumentNullException(nameof(httpResult));

        NHttpResponse response = httpContext.Response;

        response.StatusCode = httpResult.StatusCode;

        // 复制响应头
        response.SetResponseHeaders(httpResult.Headers);

        // response.Content != null 对于 204 这种响应来说没有用，仍然会引发异常，所以需要增加下面的判断
        if( HttpUtils.CanWriteResponseBody(httpContext.Request.HttpMethod, httpResult.StatusCode) == false )
            return;

        if( httpResult.Result.IsNullOrEmpty() )
            return;

        await response.WriteAllAsync(httpResult.Result.GetBytes());
        httpContext.PipelineContext.RespResult = httpResult.Result;
    }


    /// <summary>
    /// 响应HTTP请求
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="httpResult">做为输出的数据对象</param>
    /// <returns></returns>
    public static async Task HttpReplyAsync(this NHttpContext httpContext, HttpResult<byte[]> httpResult)
    {
        if( httpContext == null )
            throw new ArgumentNullException(nameof(httpContext));
        if( httpResult == null )
            throw new ArgumentNullException(nameof(httpResult));

        NHttpResponse response = httpContext.Response;

        response.StatusCode = httpResult.StatusCode;

        // 复制响应头
        response.SetResponseHeaders(httpResult.Headers);

        // response.Content != null 对于 204 这种响应来说没有用，仍然会引发异常，所以需要增加下面的判断
        if( HttpUtils.CanWriteResponseBody(httpContext.Request.HttpMethod, httpResult.StatusCode) == false )
            return;

        if( httpResult.Result.IsNullOrEmpty() )
            return;

        await response.WriteAllAsync(httpResult.Result);
    }


    /// <summary>
    /// 按HTTP500方法处理响应
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="ex"></param>
    /// <returns></returns>
    public static async Task Http500Async(this NHttpContext httpContext, Exception ex)
    {
        if( httpContext == null )
            throw new ArgumentNullException(nameof(httpContext));
        if( ex == null )
            throw new ArgumentNullException(nameof(ex));


        //httpContext.Response.Clear();

        string body = ex.ToString();

        NHttpResponse response = httpContext.Response;
        response.StatusCode = 500;
        response.SetHeader(HttpHeaders.XResponse.ExceptionType, ex.GetType().FullName);
        response.ContentType = ResponseContentType.TextUtf8;

        await response.WriteAllAsync(body.GetBytes());
        httpContext.PipelineContext.RespResult = body;
    }



}
