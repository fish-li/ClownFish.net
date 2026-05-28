namespace ClownFish.Web.Modules;

public abstract class BaseUserLoginModule : NHttpModule
{
    internal static BaseUserLoginModule Instance;

    public override void Init()
    {
        if( BaseUserLoginModule.Instance == null ) {
            BaseUserLoginModule.Instance = this;
        }
    }

    internal static readonly HashSet<string> AllowUsers = LocalSettings.GetSetting("ClownFish_Login_AllowUsers", true).SplitToHashSet();
    internal static readonly int KeepLoginDays = LocalSettings.GetUInt("ClownFish_Login_KeepDays", 1).Min(1);

    internal static readonly string ShowLoginPageUrl = LocalSettings.GetSetting("ClownFish_Login_ShowLoginPageUrl", "/v20/web/clownfish/user/login.phtml");
    internal static readonly string SendLoginCodeUrl = LocalSettings.GetSetting("ClownFish_Login_SendLoginCodeUrl", "/v20/web/clownfish/user/send-logincode");
    internal static readonly string PostLoginDataUrl = LocalSettings.GetSetting("ClownFish_Login_PostLoginDataUrl", "/v20/web/clownfish/user/login");

    public override void BeginRequest(NHttpContext httpContext)
    {
        if( httpContext.PipelineContext.Action != null )
            return;

        string url = httpContext.Request.Path;


        if( httpContext.Request.HttpMethod == "GET" ) {
            if( url.Is(ShowLoginPageUrl) ) {
                httpContext.PipelineContext.SetHttpHandler(ShowLoginPageHandler.Instance);
                return;
            }
            else if( url.Is(SendLoginCodeUrl) ) {
                httpContext.PipelineContext.SetHttpHandler(SendLoginCodeHandler.Instance);
                return;
            }
        }
        else if( httpContext.Request.HttpMethod == "POST" ) {
            if( url.Is(PostLoginDataUrl) ) {
                httpContext.PipelineContext.SetHttpHandler(UserLoginHandler.Instance);
                return;
            }
        }
    }

    public abstract void SetLoginCode(string loginName, string code);
}

internal sealed class ShowLoginPageHandler : IAsyncNHttpHandler
{
    public static readonly ShowLoginPageHandler Instance = new ShowLoginPageHandler();

    private static readonly string s_html = typeof(ShowLoginPageHandler).Assembly.ReadResAsText("ClownFish.Web.files.UserLogin.html")
                                            .Replace("$_AppName_$", EnvUtils.GetAppName())
                                            .Replace("$_SendLoginCodeUrl_$", BaseUserLoginModule.SendLoginCodeUrl)
                                            .Replace("$_PostLoginDataUrl_$", BaseUserLoginModule.PostLoginDataUrl);

    public async Task ProcessRequestAsync(NHttpContext httpContext)
    {
        await httpContext.HttpReplyAsync(200, s_html, ResponseContentType.HtmlUtf8);
    }
}

internal sealed class SendLoginCodeHandler : IAsyncNHttpHandler
{
    public static readonly SendLoginCodeHandler Instance = new SendLoginCodeHandler();

    

    /// <summary>
    /// 验证码缓存字典
    /// </summary>
    internal static readonly CacheDictionary<string> LoginCodes = new CacheDictionary<string>(false);

    public async Task ProcessRequestAsync(NHttpContext httpContext)
    {
        string loginName = httpContext.Request.QueryString("loginName");

        if( BaseUserLoginModule.AllowUsers.Contains(loginName) == false ) {
            httpContext.Response.SetHeader(HttpHeaders.XResponse.ErrorCode, "UserNotFound");
            httpContext.Response.SetHeader(HttpHeaders.XResponse.ErrorMessage, "登录名不存在！".UrlEncode());
            await httpContext.HttpReplyAsync(200, "UserNotFound");
            return;
        }


        string code = Guid.NewGuid().ToString("N").Substring(0, 6);

        // 通过企业微信发送登录码
        string text = $"{EnvUtils.GetAppName()} 登录码： {code} ，有效期2分钟。";
        BaseUserLoginModule.Instance.SetLoginCode(loginName, text);

        // 保存登录码
        LoginCodes.Set(loginName, code, DateTime.Now.AddMinutes(2d));

        await httpContext.HttpReplyAsync(200, "200");
    }

}



internal sealed class UserLoginHandler : IAsyncNHttpHandler
{
    public static readonly UserLoginHandler Instance = new UserLoginHandler();

    public async Task ProcessRequestAsync(NHttpContext httpContext)
    {
        string loginName = httpContext.Request.Form("loginName");
        string loginCode = httpContext.Request.Form("loginCode");

        if( BaseUserLoginModule.AllowUsers.Contains(loginName) == false ) {
            httpContext.Response.SetHeader(HttpHeaders.XResponse.ErrorCode, "UserNotFound");
            httpContext.Response.SetHeader(HttpHeaders.XResponse.ErrorMessage, "登录名不存在！".UrlEncode());
            await httpContext.HttpReplyAsync(200, "UserNotFound");
            return;
        }

        string code = SendLoginCodeHandler.LoginCodes.Get(loginName);
        if( code.IsNullOrEmpty() ) {
            httpContext.Response.SetHeader(HttpHeaders.XResponse.ErrorCode, "LoginCodeIsNull");
            httpContext.Response.SetHeader(HttpHeaders.XResponse.ErrorMessage, "登录码为空！".UrlEncode());
            await httpContext.HttpReplyAsync(200, "LoginCodeIsNull");
            return;
        }

        if( code.Is(loginCode) == false ) {
            httpContext.Response.SetHeader(HttpHeaders.XResponse.ErrorCode, "LoginCodeIsError");
            httpContext.Response.SetHeader(HttpHeaders.XResponse.ErrorMessage, "登录码不正确！".UrlEncode());
            await httpContext.HttpReplyAsync(200, "LoginCodeIsError");
            return;
        }

        SendLoginCodeHandler.LoginCodes.Remove(loginName);

        WebUserInfo userInfo = new WebUserInfo {
            TenantId = "NONE",
            UserId = loginName,
            UserName = loginName,
            UserRole = "UserX"
        };


        int seconds = (loginName == "liqf01" ? 365 : BaseUserLoginModule.KeepLoginDays) * 24 * 60 * 60;
        AuthenticationManager.Login(userInfo, seconds);
        httpContext.SetUserInfoToOprLog(userInfo);

        await httpContext.HttpReplyAsync(200, "200");
    }
}

