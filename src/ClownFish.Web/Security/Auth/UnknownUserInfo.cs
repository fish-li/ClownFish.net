namespace ClownFish.Web.Security.Auth;

internal sealed class LoginTicketUnknown
{
    public Dictionary<string, object> User { get; set; }
    public string Issuer { get; set; }
    public long IssueTime { get; set; }
    public long Expiration { get; set; }
}

/// <summary>
/// JWT-TOKEN有效，但是无法映射到某个用户类型的用户身份
/// </summary>
public sealed class UnknownUserInfo : IUserInfo
{
    internal Dictionary<string, object> User { get; set; }

    /// <summary>
    /// TenantId
    /// </summary>
    public string TenantId => GetValue(nameof(IUserInfo.TenantId));
    /// <summary>
    /// UserId
    /// </summary>
    public string UserId => GetValue(nameof(IUserInfo.UserId));
    /// <summary>
    /// UserName
    /// </summary>
    public string UserName => GetValue(nameof(IUserInfo.UserName));
    /// <summary>
    /// UserRole
    /// </summary>
    public string UserRole => GetValue(nameof(IUserInfo.UserRole));

    /// <summary>
    /// 读取身份中的数据
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public string GetValue(string name)
    {
        return User.TryGet(name)?.ToString();
    }

    void IUserInfo.Validate()
    {
    }
}


