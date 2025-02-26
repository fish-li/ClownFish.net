using ClownFish.WebApi;
using ClownFish.WebApi.Controllers;
using ClownFish.WebHost.Objects;

namespace ClownFish.WebHost;

internal sealed class WebHostSpacer
{
    internal async Task ProcessRequest(System.Net.HttpListenerContext context)
    {
        NHttpContext httpContext = new HttpContextSysNet(context);
        NHttpApplication app = HttpAppHost.Application;

        bool isHandled = false;   // 标记当前请求是否已经被httphandler处理

        using( HttpPipelineContext pipelineContext = HttpPipelineContext.Start(httpContext) ) {

            try {
                // 设置基本的响应头
                app.EnableCors(httpContext);
                app.InitResponse(httpContext);
                app.BeginRequest(httpContext);

                isHandled = await app.TryExecuteHttpHandlerAsync(httpContext);
                if( isHandled == false ) {

                    app.AuthenticateRequest(httpContext);
                    app.PostAuthenticateRequest(httpContext);
                    app.ResolveRequestCache(httpContext);

                    isHandled = await app.TryExecuteHttpHandlerAsync(httpContext);
                    if( isHandled == false ) {

                        app.PreFindAction(httpContext);

                        ActionDescription action = ActionLocator.FindAction(pipelineContext);
                        if( action != null ) {

                            app.PostFindAction(httpContext);

                            app.AuthorizeRequest(httpContext);
                            app.PreRequestExecute(httpContext);

                            await ActionExecutor.Execute(pipelineContext);
                            app.PostRequestExecute(httpContext);
                        }
                        else {
                            app.NotFoundAction(httpContext);   // 可以在这里指定 httphandler

                            if( pipelineContext.Action == null ) {
                                action = ControllerFactory.CreateHandler(Http404Handler.Instance);
                                pipelineContext.SetAction(action);                                
                            }

                            await ActionExecutor.Execute(pipelineContext);
                        }
                    }
                }
                
                app.UpdateRequestCache(httpContext);

                await ActionExecutor.SendResultAsync(pipelineContext);
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
