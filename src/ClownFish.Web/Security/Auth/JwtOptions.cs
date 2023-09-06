namespace ClownFish.Web.Security.Auth;

public sealed class JwtOptions
{
    public byte[] SecretKeyBytes { get; set; }

    /// <summary>
    /// 生成JWT-TOKEN时所用的HASH算法名称
    /// </summary>
    public string HashAlgorithmName { get; set; }

    /// <summary>
    /// 生成JWT-TOKEN时写入的“发布者”名称
    /// </summary>
    public string IssuerName { get; set; }

    /// <summary>
    /// 用户类型是否使用短名称
    /// </summary>
    public bool ShortTypeName { get; set; }

    /// <summary>
    /// TOKEN中的时间是否使用短时间（例如：20220206141259），否则使用 DateTime.Ticks
    /// </summary>
    public bool ShortTime { get; set; }

    /// <summary>
    /// 读取TOKEN时是否校验有效期
    /// </summary>
    public bool VerifyTokenExpiration { get; set; }

    /// <summary>
    /// 解析JWT-TOKEN时允许加载没有注册的“用户类型”
    /// </summary>
    public bool LoadUnknownUser { get; set; }


    internal void Validate()
    {
        if( this.SecretKeyBytes.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(SecretKeyBytes));

        if( this.IssuerName.IsNullOrEmpty() )
            this.IssuerName = EnvUtils.GetAppName();
    }

}
