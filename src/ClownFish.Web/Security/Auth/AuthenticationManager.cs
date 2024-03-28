namespace ClownFish.Web.Security.Auth;

/// <summary>
/// 用户身份验证操作相关的工具类
/// </summary>
public static class AuthenticationManager
{
    private static JwtProvider s_jwtImpl;

    internal static JwtOptions GetJwtOptions() => s_jwtImpl?.GetJwtOptions();

    internal static bool Inited => s_jwtImpl != null;

    public static void Init(JwtProvider provider, ICheckRights checkRights)
    {
        if( provider == null )
            throw new ArgumentNullException(nameof(provider));
                
        s_jwtImpl = provider;

        if( checkRights != null ) {
            AuthorizeAttribute.SetCheckRightsImpl(checkRights);
        }

        DebugReport.OptionList.Add(AuthenticationManager.GetJwtOptions());
    }


    private static JwtProvider GetJwtProvider()
    {
        if( s_jwtImpl == null )
            throw new InvalidOperationException("AuthenticationManager没有初始化，因此不能完成当前调用！");

        return s_jwtImpl;
    }


    /// <summary>
    /// 注册一个IUserInfo的实现类，它们将会在 JWT TOKEN 使用短名来标记
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static void RegisterUserType<T>() where T : IUserInfo
    {
        JwtJsonUserTypesBinder.RegisterUserType<T>();
    }

    /// <summary>
    /// 获取登录COOKIE字符串
    /// </summary>
    /// <param name="userInfo"></param>
    /// <param name="expirationSeconds"></param>
    /// <returns></returns>
    public static string GetLoginToken(IUserInfo userInfo, int expirationSeconds)
    {
        if( userInfo == null )
            throw new ArgumentNullException(nameof(userInfo));

        userInfo.Validate();

        if( expirationSeconds < 0 )
            throw new ArgumentOutOfRangeException(nameof(expirationSeconds));

        return GetJwtProvider().CreateToken(userInfo, expirationSeconds);
    }


    /// <summary>
    /// 登录时调用，将UserInfo对象封装成 token（用户登录凭证），并写入 cookie
    /// </summary>
    /// <param name="userInfo">用户相关数据</param>
    /// <param name="expirationSeconds">登录凭证有效期，秒数</param>
    public static string Login(IUserInfo userInfo, int expirationSeconds)
    {
        string token = GetLoginToken(userInfo, expirationSeconds);

        HttpPipelineContext ctx = HttpPipelineContext.Get2();

        TokenHelper.WriteCookie(token, expirationSeconds, ctx.HttpContext);

        // 按正常的日志流程，在管道中取不到用户身份，所以这里直接补充上去
        ctx.HttpContext.SetUserInfoToOprLog(userInfo);

        return token;
    }


    /// <summary>
    /// 手动验证当前用户身份。
    /// 默认情况下 AuthenticateModule 会自动完成这个操作。
    /// 当前方法可多次调用（后面的调用不会发生作用）
    /// </summary>
    /// <param name="httpContext"></param>
    public static void AuthenticationUser(NHttpContext httpContext)
    {
        if( httpContext == null)
            throw new ArgumentNullException(nameof(httpContext));

        TokenHelper.LoadToken(httpContext);
    }


    /// <summary>
    /// 身份认证失败时的回调事件
    /// </summary>
    public static event EventHandler<OnAuthFailedEventArgs> OnAuthFailed;


    internal static void ExecuteEventOnAuthFailed(string token, string reason)
    {
        NHttpContext httpContext = HttpPipelineContext.Get()?.HttpContext;
        if( httpContext == null )
            return;

        EventHandler<OnAuthFailedEventArgs> handler = OnAuthFailed;
        if( handler != null ) {
            OnAuthFailedEventArgs e = new OnAuthFailedEventArgs {
                RequestId = httpContext.PipelineContext.ProcessId,
                Url = httpContext.Request.RawUrl,
                Token = token,
                Reason = reason
            };
            handler(null, e);
        }
    }


    /// <summary>
    /// 解密登录TOKEN
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public static LoginTicket DecodeToken(string token)
    {
        if( string.IsNullOrEmpty(token) )
            return null;

        return GetJwtProvider().DecodeToken(token);
    }


    /// <summary>
    /// 从一个JWT Token中还原IUserInfo实例
    /// </summary>
    /// <param name="token">由GetLoginToken()方法产生的JWT Token字符串</param>
    /// <returns>如果token有效，则返回IUserInfo实例，否则返回 null</returns>
    public static IUserInfo DecodeUserInfo(string token)
    {
        return DecodeToken(token)?.User;
    }


    /// <summary>
    /// 退出登录时调用
    /// </summary>
    public static void Logout()
    {
        HttpPipelineContext ctx = HttpPipelineContext.Get2();

        NHttpResponse response = ctx.HttpContext.Response;

        response.SetCookie2(AuthOptions.CookieName, string.Empty, new DateTime(2000, 1,1));
    }


    /// <summary>
    /// 生成一个密码的HASH值
    /// </summary>
    /// <param name="password">原始密码文本</param>
    /// <param name="salt">盐值</param>
    /// <returns>原始密码的哈希值</returns>
    public static string HashPassword(string password, string salt)
    {
        if( string.IsNullOrEmpty(password) )
            throw new ArgumentNullException(nameof(password));
        if( string.IsNullOrEmpty(salt) )
            throw new ArgumentNullException(nameof(salt));

        string input = password + "\r\n" + salt;
        return HashHelper.Sha256(input, Encoding.UTF8);
    }


    internal static readonly JwtProvider SimpleJwtDecoder = new JwtProvider(new JwtOptions {
        AlgorithmName = null,
        ShortTime = true,
        ShortTypeName = true,
        VerifyTokenExpiration = false,
        HashKeyBytes = new byte[] { 0 }  // 随便指定
    });

    /// <summary>
    /// 从身份Token中读取用户信息，此方法不做数据校验。
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public static IUserInfo DecodeUserInfoWithoutVerify(string token)
    {
        // 这个方法主要给一些后台任务的场景中使用，所以就不做数据校验，
        // 校验动作应该是在HTTP请求接入口时就已经执行了。

        return SimpleJwtDecoder.DecodeToken2(token)?.User;
    }
}
