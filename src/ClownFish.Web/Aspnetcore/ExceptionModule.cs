namespace ClownFish.Web.Aspnetcore;

internal class ExceptionModule : NHttpModule
{
    public override int Order => base.Order + 10;

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

   
        if( ex is ClownFish.Base.HttpException ex2 ) {
            httpContext.Response.StatusCode = ex.GetErrorCode();
            httpContext.Response.ContentType = ResponseContentType.TextUtf8;
            httpContext.Response.WriteAll(ex2.Message.GetBytes());
        }
        else {
            OutputException(ex, httpContext);
        }

        httpContext.PipelineContext.ClearErrors();
    }

    private void OutputException(Exception ex, NHttpContext httpContext)
    {
        NHttpResponse response = httpContext.Response;

        response.StatusCode = ex.GetErrorCode();

        response.SetHeader(HttpHeaders.XResponse.ExceptionType, ex.GetType().FullName);
        response.SetHeader(HttpHeaders.XResponse.ErrorMessage, ex.Message.UrlEncode());

        response.ContentType = ResponseContentType.TextUtf8;
        response.WriteAll(ex.GetErrorLogText().GetBytes());
    }




}
