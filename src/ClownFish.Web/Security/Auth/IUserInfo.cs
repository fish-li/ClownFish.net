namespace ClownFish.Web.Security.Auth;

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
    /// 特殊标记。0:正常用户(非灰度) 1：灰度用户
    /// </summary>
    int GrayFlag => 0;
    // 如果以后要支持多级灰度，可以指定 GrayFlag=2 or GrayFlag=3，对应的 NodeGrayRule.xml 格式要增加分级


    /// <summary>
    /// 检查当前类型中的数据成员是否有效
    /// </summary>
    void Validate();
}

