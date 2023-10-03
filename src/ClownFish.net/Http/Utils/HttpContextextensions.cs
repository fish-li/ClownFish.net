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
        if( httpContext == null )
            throw new ArgumentNullException(nameof(httpContext));

        NHttpResponse response = httpContext.Response;

        if( string.IsNullOrEmpty(body) ) {
            response.StatusCode = 204;
        }
        else {
            response.StatusCode = 200;
            response.ContentType = contentType ?? ResponseContentType.TextUtf8;
            response.WriteAll(body.GetBytes());
        }
    }


    /// <summary>
    /// 响应HTTP请求
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="body"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    public static async Task HttpReplyAsync(this NHttpContext httpContext, string body, string contentType = null)
    {
        if( httpContext == null )
            throw new ArgumentNullException(nameof(httpContext));

        NHttpResponse response = httpContext.Response;

        if( string.IsNullOrEmpty(body) ) {
            response.StatusCode = 204;
        }
        else {
            response.StatusCode = 200;
            response.ContentType = contentType ?? ResponseContentType.TextUtf8;
            await response.WriteAllAsync(body.GetBytes());
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
    public static void HttpReply(this NHttpContext httpContext, int statusCode, string body, string contentType = null)
    {
        if( httpContext == null )
            throw new ArgumentNullException(nameof(httpContext));

        if( statusCode >= 300 )
            httpContext.PipelineContext.OprLogScope.OprLog.Addition = body;

        if( string.IsNullOrEmpty(body) ) {
            httpContext.Response.StatusCode = 204;
        }
        else {
            NHttpResponse response = httpContext.Response;
            response.StatusCode = statusCode;
            response.ContentType = contentType ?? ResponseContentType.TextUtf8;
            response.WriteAll(body.GetBytes());
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

        if( statusCode >= 300 )
            httpContext.PipelineContext.OprLogScope.OprLog.Addition = body;

        if( string.IsNullOrEmpty(body) ) {
            httpContext.Response.StatusCode = 204;
        }
        else {
            NHttpResponse response = httpContext.Response;
            response.StatusCode = statusCode;
            response.ContentType = contentType ?? ResponseContentType.TextUtf8;
            await response.WriteAllAsync(body.GetBytes());
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
        ResponseUtils.CopyResponseHeaders(httpResult.Headers, response);

        // response.Content != null 对于 204 这种响应来说没有用，仍然会引发异常，所以需要增加下面的判断
        if( HttpUtils.CanWriteResponseBody(httpContext.Request.HttpMethod, httpResult.StatusCode) == false )
            return;

        if( httpResult.Result.IsNullOrEmpty() )
            return;

        await response.WriteAllAsync(httpResult.Result.GetBytes());
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

        NHttpResponse response = httpContext.Response;
        response.StatusCode = 500;
        response.SetHeader(HttpHeaders.XResponse.ExceptionType, ex.GetType().FullName);
        response.ContentType = ResponseContentType.TextUtf8;
        await response.WriteAllAsync(ex.ToString().GetBytes());
    }



}
