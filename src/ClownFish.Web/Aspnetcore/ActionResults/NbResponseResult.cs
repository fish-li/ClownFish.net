using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace ClownFish.Web.AspnetCore.ActionResults;

/// <summary>
/// 
/// </summary>
public sealed class NbResponseResult : ActionResult
{
    private readonly HttpResult<string> _httpResult;
    private readonly HttpResponseMessage _responseMessage;
    private readonly HttpWebResponse _webResponse;

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

        _webResponse = webResponse;
    }

    /// <summary>
    /// ExecuteResultAsync
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task ExecuteResultAsync(ActionContext context)
    {
        NHttpContext httpContextNetCore = new HttpContextNetCore(context.HttpContext);

        if( _httpResult != null ) {
            await httpContextNetCore.HttpReplyAsync(_httpResult);
            return;
        }

        if( _responseMessage != null ) {
            await httpContextNetCore.HttpReplyAsync(_responseMessage);
            return;
        }

        if( _webResponse != null ) {
            await httpContextNetCore.HttpReplyAsync(_webResponse);
            return;
        }
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

}
