namespace ClownFish.Web.Security.Auth;

/// <summary>
/// 【普通用户】的登录身份信息
/// </summary>
public sealed class WebUserInfo : IUserInfo
{
    /// <summary>
    /// 租户ID
    /// </summary>
    public string TenantId { get; set; }

    /// <summary>
    /// 租户CODE
    /// </summary>
    public string TenantCode { get; set; }

    /// <summary>
    /// 用户ID 或者 登录名
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// 用户编号/短名
    /// </summary>
    public string UserCode { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// 用户角色
    /// </summary>
    public string UserRole { get; set; }
    
    /// <summary>
    /// 用户类别（可选）
    /// </summary>
    public string UserType { get; set; }

    /// <summary>
    /// 扩展用户信息（可选）
    /// </summary>
    public string UserData { get; set; }

    /// <summary>
    /// 扩展数据（可选）
    /// </summary>
    public string ExtData { get; set; }

    /// <summary>
    /// 特殊标记
    /// </summary>
    public int GrayFlag { get; set; }

    /// <summary>
    /// 检查当前类型中的数据成员是否有效
    /// </summary>
    public void Validate()
    {
        if( this.TenantId.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(TenantId));

        if( this.UserId.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(UserId));

        if( this.UserRole.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(UserRole));

        if( this.UserName.IsNullOrEmpty() )
            this.UserName = this.UserId;
    }


}
