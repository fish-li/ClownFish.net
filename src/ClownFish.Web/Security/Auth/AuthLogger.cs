namespace ClownFish.Web.Security.Auth;

internal static class AuthLogger
{
    public static void LogMsg(string message)
    {
        //if( ClownFishWebOptions.ShowAuthFailedMsg == false )
        //    return;

        //NHttpContext httpContext = HttpPipelineContext.Get()?.HttpContext;
        //if( httpContext == null )
        //    return;

        //OprLogScope scope = httpContext.PipelineContext.OprLogScope;

        OprLogScope scope = OprLogScope.Get();
        if( scope.IsNull )
            return;

        scope.Log(message);
    }

}
