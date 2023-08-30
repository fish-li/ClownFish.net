#if NET6_0_OR_GREATER
namespace ClownFish.Web.Aspnetcore;

internal sealed class SimpleSpacerModule
{
    private static readonly int s_requestBufferSize = LocalSettings.GetUInt("ClownFish_Aspnet_RequestBufferSize", 0);
    private static readonly bool s_debugHttpLine = LocalSettings.GetBool("ClownFish_Aspnet_DebugHttpLine");
    private static readonly bool s_deleteUselessHeaders = LocalSettings.GetBool("ClownFish_Aspnet_DeleteUselessHeaders", 1);
    private static readonly bool s_logExecutTime = LocalSettings.GetBool("ClownFish_Aspnet_LogExecutTime");

    private readonly RequestDelegate _next;

    public SimpleSpacerModule(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        if( s_logExecutTime ) {
            HttpContextUtils.LogExecutTime(httpContext);
        }

        if( s_deleteUselessHeaders ) {
            HttpHeaderUtils.DeleteUselessHeaders(httpContext.Request);
        }

        // 将不规范的URL，例如 //aa/bb/cc  强制修改为  /aa/bb/cc
        string currentUrl = httpContext.Request.Path.Value;
        if( currentUrl.StartsWith0("//") ) {
            httpContext.Request.Path = currentUrl.Substring(1);
        }

        NHttpContext httpContextNetCore = new HttpContextNetCore(httpContext);

        // 设置一些上下文及日志作用域
        using( HttpPipelineContext pipelineContext = HttpPipelineContext.Start(httpContextNetCore) ) {

            NHttpApplication app = AspnetCoreStarter.NApplication;

            try {
                app.InitResponse(httpContextNetCore);
                app.AuthenticateRequest(httpContextNetCore);

                SetRequestBuffering(httpContextNetCore);  // 允许 body 多次读取
                app.BeginRequest(httpContextNetCore);

                bool flag = await app.ExecuteHttpHandlerAsync(httpContextNetCore);
                if( flag == false ) {
                    httpContextNetCore.Response.StatusCode = 404;
                }
            }
            catch( AbortRequestException ) {
                // 提前结束请求
            }
            catch( Exception ex ) {
                pipelineContext.SetException(ex);
                app.OnError(httpContextNetCore);
            }
            finally {
                app.EndRequest(httpContextNetCore);
            }
        }


        if( s_debugHttpLine )
            Console2.Info("HTTP/" + httpContext.Response.StatusCode.ToString() + " " + httpContextNetCore.Request.HttpMethod + " " + httpContextNetCore.Request.FullUrl);
    }

    private void SetRequestBuffering(NHttpContext httpContextNetCore)
    {
        if( s_requestBufferSize <= 0 )
            return;

        if( httpContextNetCore.Request.HasBody == false )
            return;

        long bodySize = httpContextNetCore.Request.ContentLength;
        if( bodySize <= 0 )
            return;

        HttpContext httpContext = httpContextNetCore.OriginalHttpContext as HttpContext;
        if( httpContext.Request.Body.CanSeek )
            return;

        // 判断是否需要启用【请求体多次读取】功能，即：允许多次读取 Request.Body
        // https://stackoverflow.com/questions/57407472/what-is-the-alternate-of-httprequest-enablerewind-in-asp-net-core-3-0
        // 如果需要多次读取 “application/x-www-form-urlencoded” 这类请求，则必须在很早的阶段就设置


        // 下面这个方法默认 只接受【文本请求体】，且长度小于 _requestBufferSize，但是可以被重写，结果不可控
        long len = httpContextNetCore.Request.GetBodyTextLength();
        bool enabled = len > 0 && len < s_requestBufferSize;
        if( enabled == false )
            return;


        // 下面这种方式得到的流对象，在遇到请求转发时，会产生莫名奇妙的BUG（读不到请求体内容）
        //httpContext.Request.EnableBuffering(_requestBufferSize);

        MemoryStream ms = MemoryStreamPool.GetStream(nameof(SimpleSpacerModule), s_requestBufferSize);

        httpContext.Request.Body.CopyTo(ms);
        ms.Position = 0;
        httpContext.Request.Body = ms;
        httpContextNetCore.RegisterForDispose(ms);
    }

}

#endif

