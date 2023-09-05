using System.Security.Claims;

namespace ClownFish.Web.Security.Auth;

/// <summary>
/// 身份标识接口
/// </summary>
public interface INbIdentity
{
    /// <summary>
    /// 一个IUserInfo实例
    /// </summary>
    IUserInfo UserInfo { get; }
}

/// <summary>
/// 当前登录用户的身份封装对象
/// </summary>
public sealed class NbIdentity : ClaimsIdentity, INbIdentity
{
    /// <summary>
    /// 当前用户登录时传递给 Login 方法的 IUserInfo 对象。
    /// </summary>
    public IUserInfo UserInfo { get; private set; }

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="userInfo"></param>
    public NbIdentity(IUserInfo userInfo)
    {
        if( userInfo == null )
            throw new ArgumentNullException(nameof(userInfo));

        this.UserInfo = userInfo;
    }


    /// <summary>
    /// 当前用户名
    /// </summary>
    public override string Name => UserInfo.UserName;

    /// <summary>
    /// 当前用户是否已登录
    /// </summary>
    public override bool IsAuthenticated => true;

    /// <summary>
    /// 当前请求的认证方式
    /// </summary>
    public override string AuthenticationType => "JWT-TOKEN";

}
