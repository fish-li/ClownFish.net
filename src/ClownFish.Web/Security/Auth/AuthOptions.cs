﻿namespace ClownFish.Web.Security.Auth;

public static class AuthOptions
{
    /// <summary>
    /// 登录凭证在请求头中的名称
    /// </summary>
    public static string HeaderName { get; set; } = LocalSettings.GetSetting("ClownFish_Authentication_HeaderName", "x-token");

    /// <summary>
    /// 登录凭证在COOKIE中的名称
    /// </summary>
    public static string CookieName { get; set; } = LocalSettings.GetSetting("ClownFish_Authentication_CookieName", "xtoken");


    /// <summary>
    /// JWT-token是否需要 “自动续期”
    /// </summary>
    public static bool JwtTokenExpirationRenewal { get; set; } = LocalSettings.GetBool("ClownFish_JwtToken_ExpirationRenewal", 1);

}
