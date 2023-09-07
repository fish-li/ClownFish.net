using ClownFish.UnitTest.Http.Proxy;

namespace ClownFish.UnitTest.Http.Mock;

public class MockHttpPipeline : IDisposable
{
    public readonly HttpPipelineContext PipelineContext;

    public readonly MockHttpContext HttpContext;

    internal NHttpApplication Application { get; private set; }

    public Exception LastException { get; private set; }


    public MockHttpPipeline(MockRequestData requestData)
    {
        HttpContext = new MockHttpContext(requestData);
        PipelineContext = HttpPipelineContext.Start(HttpContext);
    }

    public void Dispose()
    {
        ((IDisposable)this.PipelineContext).Dispose();
        NHttpModuleFactory.Clear();
        UrlRoutingManager.ClearRules();
    }

    public void Init()
    {
        if( this.Application == null )
            Application = NHttpApplication.Start(false);
    }

    public async Task ProcessRequest()
    {
        // 此处代码源头：ClownFish.netfx\WebHost\WebHostSpacer.cs

        Init();
        NHttpApplication app = this.Application;

        MockHttpContext httpContext = this.HttpContext;            

        bool flag = false;   // 一个标记，用于判断当前请求是否已经被httphandler处理
        
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
                    app.PostFindAction(httpContext);

                    app.AuthorizeRequest(httpContext);
                    app.PreRequestExecute(httpContext);

                    await app.ExecuteHttpHandlerAsync(httpContext);
                }
            }

            app.PostRequestExecute(httpContext);
            app.UpdateRequestCache(httpContext);

        }
        catch( AbortRequestException ex ) { /* 这里就是一个标记异常，所以直接吃掉 */
            this.LastException = ex;
        }

        catch( Exception ex ) {
            this.LastException = ex;
            PipelineContext.SetException(ex);
            app.OnError(httpContext);
        }
        finally {
            app.EndRequest(httpContext);
            httpContext.Response.Close();
        }
    }

    
}
