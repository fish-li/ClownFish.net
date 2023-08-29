namespace ClownFish.Web.Aspnetcore;

internal class ExceptionModule : NHttpModule
{
    public override int Order => base.Order + 10;

    internal static readonly bool AlwaysShowFullException = LocalSettings.GetBool("ExceptionModule_AlwaysShowFullException", 1);

    public override void OnError(NHttpContext httpContext)
    {
        Exception ex = httpContext.LastException;
        if( ex == null )
            return;

        // 如果请求体已发出，就不能再处理了，否则会出现新的异常
        // 例如：System.InvalidOperationException: StatusCode cannot be set because the response has already started.
        if( httpContext.Response.HasStarted ) {
            Console2.Info("Response.HasStarted is true, ExceptionModule write...");
            Console2.Warnning(ex);
            return;
        }

        httpContext.TimeEvents?.Add(new NameTime("ExceptionModule.OnError begin"));

        if( ex is ClownFish.Base.HttpException ex2 ) {
            httpContext.Response.StatusCode = ex.GetErrorCode();
            httpContext.Response.ContentType = ResponseContentType.TextUtf8;
            httpContext.Response.WriteAll(ex2.Message.GetBytes());
        }
        else {
            if( AlwaysShowFullException
            || httpContext.Request.Header(HttpHeaders.XRequest.Debug) == "Nebula.DEBUG" ) {

                OutExceptionForDebug(ex, httpContext);
            }
            else {
                OutExceptionForPublic(ex, httpContext);
            }
        }

        httpContext.PipelineContext.ClearErrors();

        httpContext.TimeEvents?.Add(new NameTime("ExceptionModule.OnError begin"));
    }


    private void SetResponseExceptionHeaders(NHttpResponse response, Exception ex)
    {
        response.StatusCode = ex.GetErrorCode();

        response.SetHeader(HttpHeaders.XResponse.ExceptionType, ex.GetType().FullName);
        response.SetHeader(HttpHeaders.XResponse.ErrorMessage, ex.Message.UrlEncode());
    }


    private void OutExceptionForDebug(Exception ex, NHttpContext httpContext)
    {
        NHttpResponse response = httpContext.Response;
        SetResponseExceptionHeaders(response, ex);

        response.ContentType = ResponseContentType.TextUtf8;
        response.WriteAll(ex.GetErrorLogText().GetBytes());
    }


    private void OutExceptionForPublic(Exception ex, NHttpContext httpContext)
    {
        NHttpResponse response = httpContext.Response;
        SetResponseExceptionHeaders(response, ex);


        RemoteWebException ex2 = ex as RemoteWebException;
        if( ex2 != null ) {
            // 如果是【远程服务端异常】
            // 默认按照【透明代理】方式处理：直接将远程服务的结果输出给客户方

            response.ContentType = ex2.Result?.ContentType ?? ResponseContentType.TextUtf8;
            response.WriteAll(ex2.ResponseText.GetBytes());
        }
        else {
            ErrorPageModel model = new ErrorPageModel {
                Message = ex.Message,
                ExceptionType = ex.GetType().FullName,
                RequestId = httpContext.PipelineContext.ProcessId,
                StatusCode = response.StatusCode
            };

            response.ContentType = ResponseContentType.JsonUtf8;
            string json = model.ToJson();
            response.WriteAll(json.GetBytes());
        }
    }



    /// <summary>
    /// 异常页面的数据结构
    /// </summary>
    public sealed class ErrorPageModel
    {
        /// <summary>
        /// 异常的消息内容
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 异常类型
        /// </summary>
        public string ExceptionType { get; set; }

        /// <summary>
        /// 异常状态码
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// 当前请求ID
        /// </summary>
        public string RequestId { get; set; }

    }


}

