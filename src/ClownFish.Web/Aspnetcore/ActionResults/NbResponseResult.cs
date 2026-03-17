using System.Net;

namespace ClownFish.Web.Aspnetcore.ActionResults;

/// <summary>
/// 
/// </summary>
public sealed class NbResponseResult : ActionResult, IOutActionResult
{
    private readonly HttpResult<string> _httpResult;
    private readonly HttpResult<byte[]> _httpResult2;
    private readonly HttpResponseMessage _responseMessage;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="httpResult"></param>
    public NbResponseResult(HttpResult<string> httpResult)
    {
        if( httpResult == null )
            throw new ArgumentNullException(nameof(httpResult));

        _httpResult = httpResult;
    }

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="httpResult"></param>
    public NbResponseResult(HttpResult<byte[]> httpResult)
    {
        if( httpResult == null )
            throw new ArgumentNullException(nameof(httpResult));

        _httpResult2 = httpResult;
    }


    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="responseMessage"></param>
    public NbResponseResult(HttpResponseMessage responseMessage) 
    {
        if( responseMessage == null )
            throw new ArgumentNullException(nameof(responseMessage));

        _responseMessage = responseMessage;
    }

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="webResponse"></param>
    public NbResponseResult(HttpWebResponse webResponse)
    {
        if( webResponse == null )
            throw new ArgumentNullException(nameof(webResponse));

        _responseMessage = webResponse.ToResponseMessage();
    }

    /// <summary>
    /// ExecuteResultAsync
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task ExecuteResultAsync(ActionContext context)
    {
        NHttpContext httpContextNetCore = HttpPipelineContext.Get2().HttpContext;
        await OutResultAsync(httpContextNetCore);
    }


    /// <summary>
    /// ExecuteResult
    /// </summary>
    /// <param name="context"></param>
    /// <exception cref="NotImplementedException"></exception>
    public override void ExecuteResult(ActionContext context)
    {
        throw new NotImplementedException();
    }

    public async Task OutResultAsync(NHttpContext httpContext)
    {
        if( _httpResult != null ) {
            await httpContext.HttpReplyAsync(_httpResult);
            return;
        }

        if( _httpResult2 != null ) {
            await httpContext.HttpReplyAsync(_httpResult2);
            return;
        }

        if( _responseMessage != null ) {
            await httpContext.HttpReplyAsync(_responseMessage);
            return;
        }
    }
}
