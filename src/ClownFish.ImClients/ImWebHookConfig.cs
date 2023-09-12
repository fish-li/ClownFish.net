namespace ClownFish.ImClients;

/// <summary>
/// 企业微信/钉钉/飞书 WebHook 登录参数
/// </summary>
public sealed class ImWebHookConfig
{
    /// <summary>
    /// IM类别
    /// </summary>
    public ImType ImType { get; set; }

    /// <summary>
    /// WebHook URL
    /// </summary>
    public string WebHookUrl { get; set; }

    /// <summary>
    /// 签名密钥，可为空。
    /// </summary>
    public string SignKey { get; set; }


    internal void Validate()
    {
        if( this.WebHookUrl.IsNullOrEmpty() )
            throw new ValidationException2("WebHookUrl is empty.");
    }
}
