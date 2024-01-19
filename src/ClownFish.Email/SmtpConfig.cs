namespace ClownFish.Email;

/// <summary>
/// 邮箱发送参数
/// </summary>
public class SmtpConfig
{
    /// <summary>
    /// 服务器地址
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// TCP端口
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// 是否使用SSL连接
    /// </summary>
    public bool IsSSL { get; set; }


    internal void Validate()
    {
        if( this.Host.IsNullOrEmpty() )
            throw new ValidationException2("Host 不能为空");

        if( this.UserName.IsNullOrEmpty() )
            throw new ValidationException2("UserName 不能为空");
    }
}
