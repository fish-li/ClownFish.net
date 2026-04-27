using System.Diagnostics.CodeAnalysis;
using ClownFish.Jwt;

namespace ClownFish.Web.Security.Auth;


public sealed class JwtProvider
{
    private readonly JwtJsonSerializer _serializer;
    private readonly JwtOptions _options;

    public JwtOptions GetJwtOptions() => _options;

    public JwtProvider(JwtOptions options)
    {
        if( options == null )
            throw new ArgumentNullException(nameof(options));

        options.Validate();

        _serializer = new JwtJsonSerializer(options.ShortTypeName);
        _options = options;
    }


    /// <summary>
    /// 创建一个登录TOKEN
    /// </summary>
    /// <param name="data">用户相关数据</param>
    /// <param name="issueTime"></param>
    /// <param name="expiration">登录凭证有效期</param>
    /// <returns></returns>
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(LoginTicket))]
    public string CreateToken(IUserInfo data, DateTime issueTime, DateTime expiration)
    {
        if( data == null )
            throw new ArgumentNullException(nameof(data));

        if( data.GetType().IsGenericType )      // 不支持泛型主要是不希望生成的类型名称太长，最后影响TOKEN的长度！
            throw new NotSupportedException("UserInfo对象类型不支持泛型。");


        LoginTicket ticket = new LoginTicket {
            User = data,
            Issuer = _options.IssuerName.IfEmpty(EnvUtils.GetAppName()),
            IssueTime = _options.ShortTime ? issueTime.ToNumber() : issueTime.Ticks,
            Expiration = _options.ShortTime ? expiration.ToNumber() : expiration.Ticks,
        };

        string payloadJosn = _serializer.Serialize(ticket);

        if( _options.IsAsymmetricAlgorithm )
            return JwtUtils.Encode2(payloadJosn, _options.X509Cert, _options.AlgorithmName);
        else
            return JwtUtils.Encode(payloadJosn, _options.HashKeyBytes, _options.AlgorithmName);
    }


    public string CreateToken(IUserInfo data, long expirationSeconds)
    {
        DateTime expiration = expirationSeconds == 0
                                ? DateTime.Now.AddDays(7)
                                : DateTime.Now.AddSeconds(expirationSeconds);

        return CreateToken(data, DateTime.Now, expiration);
    }

    /// <summary>
    /// 验证并读取TOKEN中的数据
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public LoginTicket DecodeToken(string token)
    {
        if( string.IsNullOrEmpty(token) )
            throw new ArgumentNullException(nameof(token));

        string json = _options.IsAsymmetricAlgorithm
                        ? DecodePayload2(token, _options.X509Cert)
                        : DecodePayload(token, _options.HashKeyBytes);

        if( json == null ) {
            if( ClownFishWebOptions.ShowAuthFailedMsg ) {
                AuthLogger.LogMsg("[身份认证失败] DecodePayload return null");
            }
            return null;
        }

        return DecodeJson(token, json, true);
    }


    /// <summary>
    /// 直接解析TOKEN，不验证签名
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public LoginTicket DecodeToken2(string token)
    {
        if( string.IsNullOrEmpty(token) )
            throw new ArgumentNullException(nameof(token));

        string json = DecodePayload(token, null);

        return DecodeJson(token, json, true);
    }


    /// <summary>
    /// 解析Token的 Payload 部分
    /// </summary>
    /// <param name="token"></param>
    /// <param name="x509">如果不指定此参数，将不会签名验证</param>
    /// <returns></returns>
    private string DecodePayload2(string token, X509Certificate2 x509)
    {
        if( string.IsNullOrEmpty(token) )
            throw new ArgumentNullException(nameof(token));

        try {
            return JwtUtils.Decode2(token, x509, _options.AlgorithmName);
        }
        catch( Exception ex ) {
            if( ClownFishWebOptions.ShowAuthFailedMsg ) {
                AuthLogger.LogMsg($"[身份认证失败] DecodePayload2: catch-Exception: {ex.GetType().FullName}, {ex.Message}");
            }
        }

        return null;
    }

    internal string DecodePayload(string token, byte[] secretKey = null)
    {
        if( string.IsNullOrEmpty(token) )
            throw new ArgumentNullException(nameof(token));

        try {
            return JwtUtils.Decode(token, secretKey, _options.AlgorithmName);
        }
        catch( Exception ex ) {
            if( ClownFishWebOptions.ShowAuthFailedMsg ) {
                AuthLogger.LogMsg($"[身份认证失败] DecodePayload: catch-Exception: {ex.GetType().FullName}, {ex.Message}");
            }
        }

        return null;
    }


    internal LoginTicket DecodeJson(string token, string json, bool verifyExpiration)
    {
        if( string.IsNullOrEmpty(json) )
            return null;

        // 还原LoginTicket对象
        LoginTicket ticket = null;

        try {
            ticket = DecodeJson1(token, json) ?? DecodeJson2(token, json);
        }
        catch( Exception ex ) {
            // 忽略所有错误，防止攻击
            if( ClownFishWebOptions.ShowAuthFailedMsg ) {
                AuthLogger.LogMsg($"[身份认证失败] DecodeJson: catch-Exception: {ex.GetType().FullName}, {ex.Message}");
            }
            return null;
        }

        if( ticket != null && verifyExpiration && _options.VerifyTokenExpiration ) {
            // 检查过期时间
            if( ticket.VerifyExpiration() == false ) {
                if( ClownFishWebOptions.ShowAuthFailedMsg ) {
                    AuthLogger.LogMsg("[身份认证失败] DecodeJson: VerifyExpiration=false");
                }
                return null;
            }
        }

        if( ticket == null ) {
            if( ClownFishWebOptions.ShowAuthFailedMsg ) {
                AuthLogger.LogMsg("[身份认证失败] DecodeJson: ticket=null");
            }
        }
        return ticket;
    }


    private LoginTicket DecodeJson1(string token, string json)
    {
        try {
            // 先按【正常】方式做反序列化
            return _serializer.Deserialize<LoginTicket>(json);
        }
        catch( JsonSerializationException ex ) {
            // 有可能是 IUserInfo 的实现类型找不到，所以忽略这个异常，下面按无类型方式处理
            if( ClownFishWebOptions.ShowAuthFailedMsg ) {
                AuthLogger.LogMsg($"[身份认证失败] DecodeJson1: catch-Exception: {ex.GetType().FullName}, {ex.Message}");
            }
        }
        return null;
    }

    private LoginTicket DecodeJson2(string token, string json)
    {
        if( _options.LoadUnknownUser == false )
            return null;

        LoginTicketUnknown t2 = null;
        try {
            // 再按【弱类型】方式做反序列化
            t2 = json.FromJson<LoginTicketUnknown>();
        }
        catch( JsonSerializationException ex ) {  // JSON格式无法识别
            if( ClownFishWebOptions.ShowAuthFailedMsg ) {
                AuthLogger.LogMsg($"[身份认证失败] DecodeJson2: catch-Exception: {ex.GetType().FullName}, {ex.Message}");
            }
        }

        if( t2 == null )
            return null;

        if( ClownFishWebOptions.ShowAuthFailedMsg ) {
            AuthLogger.LogMsg("[身份认证消息] load token as UnknownUserInfo");
        }

        // 构造一个特殊的用户身份凭证
        return new LoginTicket {
            User = new UnknownUserInfo { User = t2.User },
            Issuer = t2.Issuer,
            IssueTime = t2.IssueTime,
            Expiration = t2.Expiration
        };
    }

}
