using ClownFish.Jwt;

namespace ClownFish.Web.Security.Auth;

public sealed class JwtOptions
{
    /// <summary>
    /// 生成JWT-TOKEN时所用的算法名称
    /// </summary>
    public string AlgorithmName { get; set; }

    /// <summary>
    /// 是否为 “非对称算法”，例如：RSA/ECDsa
    /// </summary>
    public bool IsAsymmetricAlgorithm { get; private set; }

    /// <summary>
    /// 当使用HMACSHA算法时的密钥
    /// </summary>
    public byte[] HashKeyBytes { get; set; }

    /// <summary>
    /// 当使用RSA/ECDsa算法时的密钥证书
    /// </summary>
    public X509Certificate2 X509Cert { get; set; }


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
        if( this.AlgorithmName.IsNullOrEmpty() )
            this.AlgorithmName = JwtUtils.DefaultAlgorithm;

        if( AlgorithmName.StartsWith0("HS") ) {
            if( this.HashKeyBytes.IsNullOrEmpty() )
                throw new ArgumentNullException(nameof(HashKeyBytes));
        }
        else {
            if( this.X509Cert == null )
                throw new ArgumentNullException(nameof(X509Cert));

            this.IsAsymmetricAlgorithm = true;
        }

        if( this.IssuerName.IsNullOrEmpty() )
            this.IssuerName = EnvUtils.GetAppName();
    }


}
