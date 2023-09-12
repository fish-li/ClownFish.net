namespace ClownFish.Log.Attributes;

/// <summary>
/// 指示当前Action是不是登录相关的操作。
/// 
/// 登录请求会有二个不同的处理方式：
/// 1、登录凭证不做自动续期处理
/// 2、日志记录时忽略请求体
/// </summary>
/// <remarks>
/// 注意：
/// 在登录请求中，尤其是接收到用户提交的密码时，必须尽快验证处理，不要再传递到外部进程，否则可能造成密码泄露。
/// 例如在查询数据库对比密码时，应该先用HASH处理，避免使用明文。
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class LoginActionAttribute : Attribute
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public static bool CurrentIsLogin(ActionDescription action)
    {
        return action.MethodInfo.GetMyAttribute<LoginActionAttribute>() != null;
    }
}
