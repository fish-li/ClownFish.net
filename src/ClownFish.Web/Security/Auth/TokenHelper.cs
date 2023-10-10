namespace ClownFish.Web.Security.Auth;

internal static class TokenHelper
{
    public static void WriteCookie(string token, int expirationSeconds, NHttpContext httpContext)
    {
        DateTime? expires = null;
        if( expirationSeconds > 0 ) {
            expires = DateTime.Now.AddSeconds(expirationSeconds);
        }

        httpContext.Response.SetCookie2(AuthOptions.CookieName, token, expires);
    }

    public static void WriteHeader(string token, NHttpContext httpContext)
    {
        httpContext.Response.SetHeader(AuthOptions.HeaderName, token);
    }


    /// <summary>
    /// 加载请求中的身份凭证，并设置 httpContext.User ,
    /// 当前方法可多次调用（后面的调用不会发生作用）
    /// </summary>
    /// <param name="httpContext"></param>
    public static void LoadToken(NHttpContext httpContext)
    {
        if( httpContext.User != null )
            return;


        // 尝试从请求头中查找 JWT Token
        // 类似于： Authentication: Bearer xxxxxxxxxxxxxxx  的思路，
        // Bearer 会导致频繁的字符串拼接和拆分影响性能，所以直接用一个单独的请求头
        string token = httpContext.Request.Header(AuthOptions.HeaderName);
        if( string.IsNullOrEmpty(token) == false ) {
            SetContextUser(httpContext, token, LoginTicketSource.Header);

            // 只要存在请求头，就认为是有效
            return;
        }

        // 尝试从Authorization请求头中加载 JWT Token
        token = httpContext.Request.Header("Authorization");
        if( string.IsNullOrEmpty(token) == false && token.StartsWith0("Bearer ") ) {
            token = token.Substring(7);  // 去掉 schema 前缀
            if( string.IsNullOrEmpty(token) == false ) {
                SetContextUser(httpContext, token, LoginTicketSource.Header);
                return;
            }
        }

        // 尝试从Cookie中查找 JWT Token
        token = httpContext.Request.Cookie(AuthOptions.CookieName);
        if( string.IsNullOrEmpty(token) == false ) {
            SetContextUser(httpContext, token, LoginTicketSource.Cookie);
            return;
        }


        string authHeaderName = httpContext.Request.Header("x-auth-headername");
        if( authHeaderName.HasValue() ) {
            token = httpContext.Request.Header(authHeaderName);
            if( string.IsNullOrEmpty(token) == false ) {
                SetContextUser(httpContext, token, LoginTicketSource.Header);
                return;
            }
        }

        string authCookiename = httpContext.Request.Header("x-auth-cookiename");
        if( authCookiename.HasValue() ) {
            token = httpContext.Request.Cookie(authCookiename);
            if( string.IsNullOrEmpty(token) == false ) {
                SetContextUser(httpContext, token, LoginTicketSource.Cookie);
                return;
            }
        }
    }


    private static void SetContextUser(NHttpContext httpContext, string token, LoginTicketSource source)
    {
        LoginTicket ticket = AuthenticationManager.DecodeToken(token);
        if( ticket == null || ticket.User == null )
            return;

        httpContext.User = new NbPrincipal(ticket, source, token);
    }


    
}
