namespace ClownFish.DTO;

/// <summary>
/// 定义基本的登录身份信息的数据接口
/// </summary>
public interface IUserInfo
{
    /// <summary>
    /// 租户ID
    /// </summary>
    string TenantId { get; }

    /// <summary>
    /// 用户ID 或者 登录名
    /// </summary>
    string UserId { get; }

    /// <summary>
    /// 用户名（或者登录名）
    /// </summary>
    string UserName { get; }
    
    /// <summary>
    /// 用户角色
    /// </summary>
    string UserRole { get; }


    /// <summary>
    /// 特殊标记。0:正常用户(非灰度) 1：外部灰度用户, 2: 内部灰度用户
    /// </summary>
#if NET6_0_OR_GREATER
    int GrayFlag => 0;
#else
    int GrayFlag { get; }
#endif

    /// <summary>
    /// 检查当前类型中的数据成员是否有效
    /// </summary>
    void Validate();
}

