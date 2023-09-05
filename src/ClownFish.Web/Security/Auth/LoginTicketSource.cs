namespace ClownFish.Web.Security.Auth;

/// <summary>
/// 登录凭证的获取来源
/// </summary>
public enum LoginTicketSource
{
    /// <summary>
    /// 从请求头中取得
    /// </summary>
    Header = 100,

    /// <summary>
    /// 从Cookie中取得
    /// </summary>
    Cookie = 200,

    /// <summary>
    /// 来自于服务端会话
    /// </summary>
    Session = 900,

    /// <summary>
    /// 其它来源
    /// </summary>
    Others = 999
}


internal static class LoginTicketSourceExtensions
{
    public static bool IsFromClientSide(this LoginTicketSource source)
    {
        return source == LoginTicketSource.Header || source == LoginTicketSource.Cookie;
    }
}