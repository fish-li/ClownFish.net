using ClownFish.Base.Jwt.Impl;

namespace ClownFish.Base.Jwt;

/// <summary>
/// JWT加密解密工具类。
/// 这个类仅供 框架内部使用，因此有些地方做了简化处理，例如：仅使用 HS256 签名算法。
/// </summary>
public static class JwtUtils
{
    /// <summary>
    /// 默认的Hash算法名称
    /// </summary>
    public static readonly string DefaultHashAlgorithmName = "HS256";

    internal static readonly JwtBase JwtHs256 = new JwtHMACSHA256();
    internal static readonly JwtBase JwtHs512 = new JwtHMACSHA512();

    // 以后如果有需要再增加对RSA的支持，
    // 除了增加新类型 RS256,RS512, 还要再增加 SecretKey类型（它包含 byte[], X509Certificate2）

    internal static JwtBase CreateImpl(string algorithmName)
    {
        if( algorithmName.IsNullOrEmpty() )
            algorithmName = DefaultHashAlgorithmName;

        return algorithmName switch {
            "HS256" => JwtHs256,
            "HS512" => JwtHs512,
            _ => throw new NotSupportedException()
        };
    }

    /// <summary>
    /// 创建一个 JWT Token 字符串
    /// </summary>
    /// <param name="payload">Token中包含的数据对象</param>
    /// <param name="secretKey">密钥</param>
    /// <param name="algorithmName"></param>
    /// <returns></returns>
    public static string Encode(string payload, byte[] secretKey, string algorithmName = null)
    {
        JwtBase jwtImpl = CreateImpl(algorithmName);
        return jwtImpl.Encode(payload, secretKey);
    }



    /// <summary>
    /// 解析 JWT Token
    /// </summary>
    /// <param name="token">Token字符串</param>
    /// <param name="secretKey">用于校验Token的密钥，如果为空则不做校验</param>
    /// <param name="algorithmName"></param>
    /// <returns>返回 payload 部分，是一个JSON字符串</returns>
    public static string Decode(string token, byte[] secretKey, string algorithmName = null)
    {
        JwtBase jwtImpl = CreateImpl(algorithmName);
        return jwtImpl.Decode(token, secretKey);
    }

    
}
