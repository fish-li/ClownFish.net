namespace ClownFish.UnitTest.Http.Pipleline.Test;

public class TestModule1 : NHttpModule
{
    public override void Init()
    {
        base.Init();
    }
    public override void BeginRequest(NHttpContext httpContext)
    {
        base.BeginRequest(httpContext);
    }

    public override void AuthenticateRequest(NHttpContext httpContext)
    {
        base.AuthenticateRequest(httpContext);
    }

    public override void PostAuthenticateRequest(NHttpContext httpContext)
    {
        base.PostAuthenticateRequest(httpContext);
    }

    public override void ResolveRequestCache(NHttpContext httpContext)
    {
        base.ResolveRequestCache(httpContext);
    }

    public override void PreFindAction(NHttpContext httpContext)
    {
        base.PreFindAction(httpContext);

        // 这里代替URL路由，直接选择了一个 Handler
        httpContext.PipelineContext.SetHttpHandler(new TestHandler1());
    }

    public override void PostFindAction(NHttpContext httpContext)
    {
        base.PostFindAction(httpContext);
    }
    public override void AuthorizeRequest(NHttpContext httpContext)
    {
        base.AuthorizeRequest(httpContext);
    }

    public override void PreRequestExecute(NHttpContext httpContext)
    {
        base.PreRequestExecute(httpContext);
    }

    public override void PostRequestExecute(NHttpContext httpContext)
    {
        base.PostRequestExecute(httpContext);

        httpContext.PipelineContext.ActionResult = "OK/1533";
    }

    public override void UpdateRequestCache(NHttpContext httpContext)
    {
        base.UpdateRequestCache(httpContext);
    }

    public override void EndRequest(NHttpContext httpContext)
    {
        base.EndRequest(httpContext);
    }

    public override void OnError(NHttpContext httpContext)
    {
        base.OnError(httpContext);
    }

}
