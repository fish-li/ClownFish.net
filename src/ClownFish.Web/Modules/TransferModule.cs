namespace ClownFish.Web.Modules;

public sealed class TransferModule : HttpProxyModule
{
    public override int Order => -900;

    private static readonly ProxyRuleManager s_proxyRuleManager = new ProxyRuleManager();
        
    public static bool IsEnable()
    {
        if( NHttpModuleFactory.ModuleIsEnable(typeof(TransferModule)) == false )
            return false;

        string filePath1 = EnvUtils.GetAppName() + ".TransferMapRule.xml";
        return s_proxyRuleManager.Init(filePath1);
    }


    protected override string GetDestUrl(NHttpRequest request)
    {
        return s_proxyRuleManager.GetProxyDestUrl(request);
    }

    protected override IAsyncNHttpHandler CreateProxyHandler(NHttpContext httpContext, string destUrl)
    {
        if( httpContext.PipelineContext.OprLogScope.IsNull == false ) {
            httpContext.PipelineContext.OprLogScope.OprLog.Addition = "destUrl: " + destUrl;
        }

        return new HttpProxyHandler2(destUrl);
    }

    protected override void PreExecuteHandler(NHttpContext httpContext, IAsyncNHttpHandler handler)
    {
        // 转发的请求都不监控执行耗时时间
        httpContext.PipelineContext.SetAsLongTask();
    }

}


