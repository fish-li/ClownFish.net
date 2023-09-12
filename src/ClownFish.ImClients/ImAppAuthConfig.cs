namespace ClownFish.ImClients;

/// <summary>
/// 企业微信/钉钉/飞书 应用 登录参数
/// </summary>
public sealed class ImAppAuthConfig
{
    /// <summary>
    /// IM类别
    /// </summary>
    public ImType ImType { get; set; }

    /// <summary>
    /// 企业微信的 CorpId，飞书的 AppID，钉钉的 AppKey
    /// </summary>
    public string AppId { get; set; }

    /// <summary>
    /// 企业微信的 Secret，飞书的 AppSecret，钉钉的 AppSecret
    /// </summary>
    public string AppSecret { get; set; }

    /// <summary>
    /// 企业微信/钉钉的 AgentId，飞书不使用此参数
    /// </summary>
    public long AgentId { get; set; }


    /// <summary>
    /// 验证数据成员
    /// </summary>
    /// <exception cref="ValidationException2"></exception>
    public void Validate()
    {
        if( this.AppId.IsNullOrEmpty() )
            throw new ValidationException2("AppId is empty.");

        if( this.AppSecret.IsNullOrEmpty() )
            throw new ValidationException2("AppSecret is empty.");
    }
}



