namespace ClownFish.Web.Aspnetcore;

public sealed class ExceptionModule : NHttpModule
{
    public override int Order => 99999;   // 尽量放在最后面，允许项目中定义自己的ExceptionModule抢先执行异常处理

    internal static bool AlwaysShowFullException = LocalSettings.GetBool("ExceptionModule_AlwaysShowFullException", 1);

    /// <summary>
    /// 决定在异常发生时，是否要给客户端输出异常详情的回调委托
    /// </summary>
    public static Func<NHttpContext, Exception, bool> IsShowFullExceptionCallback = IsShowFullException;

    private static bool IsShowFullException(NHttpContext httpContext, Exception ex)
    {
        return AlwaysShowFullException;
    }

    public override void OnError(NHttpContext httpContext)
    {
        Exception ex = httpContext.LastException;
        if( ex == null )
            return;

        // 如果请求体已发出，就不能再处理了，否则会出现新的异常
        // 例如：System.InvalidOperationException: StatusCode cannot be set because the response has already started.
        if( httpContext.Response.HasStarted ) {
            Console2.Info("Response.HasStarted is true, ExceptionModule ignore current error....., original exception:");
            Console2.Warnning(ex);
            return;
        }

        httpContext.LogFxEvent(new NameTime("ExceptionModule.OnError begin"));

        if( ex is HttpException ex2 ) {
            httpContext.HttpReply(ex.GetErrorCode(), ex2.ToString());
        }
        else {
            if( IsShowFullExceptionCallback.Invoke(httpContext, ex) ) {
                OutExceptionForDebug(ex, httpContext);
            }
            else {
                OutExceptionForPublic(ex, httpContext);
            }
        }

        httpContext.PipelineContext.ClearErrors();

        httpContext.LogFxEvent(new NameTime("ExceptionModule.OnError begin"));
    }


    private void OutExceptionForDebug(Exception ex, NHttpContext httpContext)
    {
        string reponseBody = ex.GetErrorLogText();
        WriteResponse(httpContext, null, reponseBody, ex);
    }


    private void OutExceptionForPublic(Exception ex, NHttpContext httpContext)
    {
        RemoteWebException ex2 = ex as RemoteWebException;
        if( ex2 != null ) {
            // 如果是【远程服务端异常】
            // 默认按照【透明代理】方式处理：直接将远程服务的结果输出给客户方

            string contentType = ex2.Result?.ContentType;
            string reponseBody = ex2.ResponseText;
            WriteResponse(httpContext, contentType, reponseBody, ex);
        }
        else {
            ErrorPageModel model = new ErrorPageModel {
                Message = ex.Message,
                ExceptionType = ex.GetType().FullName,
                RequestId = httpContext.PipelineContext.ProcessId,
                StatusCode = ex.GetErrorCode()
            };

            string json = model.ToJson();
            WriteResponse(httpContext, ResponseContentType.JsonUtf8, json, ex);
        }
    }


    private void WriteResponse(NHttpContext httpContext, string contentType, string reponseBody, Exception ex)
    {
        NHttpResponse response = httpContext.Response;
        response.SetHeader(HttpHeaders.XResponse.ExceptionType, ex.GetType().FullName);
        response.SetHeader(HttpHeaders.XResponse.ErrorMessage, ex.Message.UrlEncode());

        httpContext.HttpReply(ex.GetErrorCode(), reponseBody, contentType);
    }
}

