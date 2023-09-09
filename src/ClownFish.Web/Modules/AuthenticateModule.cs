using ClownFish.Web.Security.Auth;

namespace ClownFish.Web.Modules;

public sealed class AuthenticateModule : NHttpModule
{
    public override int Order => -10;

    public override void AuthenticateRequest(NHttpContext httpContext)
    {
        // 识别用户身份
        AuthenticationManager.AuthenticationUser(httpContext);
    }


}
