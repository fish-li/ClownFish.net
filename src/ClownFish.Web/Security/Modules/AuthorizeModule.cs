namespace ClownFish.Web.Modules;

public sealed class AuthorizeModule : NHttpModule
{
    public override int Order => -1;

    public override void AuthorizeRequest(NHttpContext httpContext)
    {
        if( AuthorizeCheck(httpContext) == false )
            return;



        // TOKEN自动续期处理
        if( AuthOptions.JwtTokenExpirationRenewal ) {

            // 当前请求不是登录相关的操作，要做续期处理
            if( httpContext.PipelineContext.IsLoginAction == false ) {

                // 判断是否需要做登录凭证的自动续期
                // 这个调用放在这里也是一种无奈！
                // 1，AuthenticateModule 执行时机太早，如果早早写了请求头，对于登录请求，会出现请求头重复现象（可能不影响结果）
                // 2，根源还是 asp.net core API 设计太SB，没有【删除】机制，只能追加

                TryRefreshLoginTicket(httpContext);
            }
        }
    }


    internal static bool AuthorizeCheck(NHttpContext httpContext)
    {
        // 当前Action 不允许 匿名访问
        AllowAnonymousAttribute allowAnonymous = httpContext.PipelineContext.Action.GetActionAttribute<AllowAnonymousAttribute>();
        if( allowAnonymous == null ) {

            // 当前Action已明确标记， [Authorize] ，要求做权限检查
            AuthorizeAttribute attribute = httpContext.PipelineContext.Action.GetActionAttribute<AuthorizeAttribute>();
            if( attribute != null ) {

                // 当前用户没有登录
                if( httpContext.IsAuthenticated == false ) {
                    httpContext.Response.SetHeader(HttpHeaders.XResponse.ErrorCode, "NotLogin");
                    httpContext.HttpReply(401, "Please login.");
                    httpContext.Response.End();
                    return false;
                }

                // 当前用户没有相应的访问权限，禁止访问
                if( attribute.AuthenticateRequest(httpContext) == false ) {
                    httpContext.Response.SetHeader(HttpHeaders.XResponse.ErrorCode, "NoPermission");
                    httpContext.HttpReply(403, "Authorize validate failed.");
                    httpContext.Response.End();
                    return false;
                }
            }
        }

        return true;
    }


    /// <summary>
    /// 判断是否需要做登录凭证的自动续期
    /// </summary>
    /// <param name="httpContext"></param>
    internal static void TryRefreshLoginTicket(NHttpContext httpContext)
    {
        // 当前用户没有登录
        NbPrincipal principal = httpContext.User as NbPrincipal;
        if( principal == null )
            return;

        // 对于【非客户端】来源，不做续期处理
        if( principal.Source.IsFromClientSide() == false )
            return;


        // 获取登录凭证
        LoginTicket ticket = principal.Ticket;


        // 判断是否需要续期
        if( ticket.IsNeedRefresh() == false )
            return;

        // 获取登录凭证的有效期
        int expirationSeconds = ticket.GetSeconds();

        // 重新生成新的登录凭证
        string newToken = AuthenticationManager.GetLoginToken(ticket.User, expirationSeconds);


        // 根据凭证来源，生成新的凭证
        if( principal.Source == LoginTicketSource.Header ) {
            TokenHelper.WriteHeader(newToken, httpContext);
        }
        else if( principal.Source == LoginTicketSource.Cookie ) {
            TokenHelper.WriteCookie(newToken, expirationSeconds, httpContext);
        }
    }




}
