using ClownFish.WebApi;
using ClownFish.WebHost.Objects;

namespace ClownFish.WebHost;

internal class WebHostSpacer
{
    internal async Task ProcessRequest(System.Net.HttpListenerContext context)
    {
        NHttpContext httpContext = new HttpContextSysNet(context);
        NHttpApplication app = HttpAppHost.Application;

        bool flag = false;   // 一个标记，用于判断当前请求是否已经被httphandler处理

        using( HttpPipelineContext pipelineContext = HttpPipelineContext.Start(httpContext) ) {

            try {
                // 设置基本的响应头
                app.EnableCors(httpContext);
                app.InitResponse(httpContext);
                app.BeginRequest(httpContext);

                flag = await app.ExecuteHttpHandlerAsync(httpContext);
                if( flag == false ) {

                    app.AuthenticateRequest(httpContext);
                    app.PostAuthenticateRequest(httpContext);
                    app.ResolveRequestCache(httpContext);

                    flag = await app.ExecuteHttpHandlerAsync(httpContext);
                    if( flag == false ) {

                        app.PreFindAction(httpContext);
                        ActionLocator.Instance.FindAction(pipelineContext);
                        app.PostFindAction(httpContext);

                        app.AuthorizeRequest(httpContext);
                        app.PreRequestExecute(httpContext);

                        await ActionExecutor.Instance.ExecuteAction(pipelineContext);
                    }
                }

                app.PostRequestExecute(httpContext);
                app.UpdateRequestCache(httpContext);

                ActionExecutor.Instance.SendResult(pipelineContext);
            }
            catch( AbortRequestException ) { /* 这里就是一个标记异常，所以直接吃掉 */ }

            catch( Exception ex ) {
                pipelineContext.SetException(ex);
                app.OnError(httpContext);
            }
            finally {
                app.EndRequest(httpContext);
                httpContext.Response.Close();
            }
        }
    }


}
