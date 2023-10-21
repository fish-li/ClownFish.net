using ClownFish.Jwt.Impl;

namespace ClownFish.Jwt;

/// <summary>
/// JWT加密解密工具类
/// </summary>
public static class JwtUtils
{
    /// <summary>
    /// 默认的算法名称
    /// </summary>
    public static readonly string DefaultAlgorithm = JwtHMACSHA512.AlgorithmName;


    internal static JwtBase CreateImpl(string algorithmName)
    {
        if( algorithmName.IsNullOrEmpty() )
            algorithmName = DefaultAlgorithm;

        return algorithmName switch {
            JwtHMACSHA256.AlgorithmName => JwtHMACSHA256.Instance,
            JwtHMACSHA512.AlgorithmName => JwtHMACSHA512.Instance,
            JwtRSA256.AlgorithmName => JwtRSA256.Instance,
            JwtRSA512.AlgorithmName => JwtRSA512.Instance,
            JwtECD256.AlgorithmName => JwtECD256.Instance,
            JwtECD512.AlgorithmName => JwtECD512.Instance,
            _ => throw new NotSupportedException("不支持的JWT签名算法：" + algorithmName)
        };
    }

    /// <summary>
    /// 创建一个 JWT Token 字符串
    /// </summary>
    /// <param name="payload">Token中包含的数据对象</param>
    /// <param name="secretKey">密钥</param>
    /// <param name="algorithmName"></param>
    /// <returns></returns>
    public static string Encode(string payload, byte[] secretKey, string algorithmName)
    {
        if( payload.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(payload));
        if( secretKey.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(secretKey));

        JwtBase jwtImpl = CreateImpl(algorithmName);
        return jwtImpl.Encode(payload, secretKey);
    }

    /// <summary>
    /// 创建一个 JWT Token 字符串
    /// </summary>
    /// <param name="payload">Token中包含的数据对象</param>
    /// <param name="x509">x509证书</param>
    /// <param name="algorithmName"></param>
    /// <returns></returns>
    public static string Encode2(string payload, X509Certificate2 x509, string algorithmName)
    {
        if( payload.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(payload));
        if( x509 == null)
            throw new ArgumentNullException(nameof(x509));

        JwtBase jwtImpl = CreateImpl(algorithmName);
        return jwtImpl.Encode(payload, x509);
    }



    /// <summary>
    /// 解析 JWT Token
    /// </summary>
    /// <param name="token">Token字符串</param>
    /// <param name="secretKey">用于校验Token的密钥，如果为空则不做校验</param>
    /// <param name="algorithmName"></param>
    /// <returns>返回 payload 部分，是一个JSON字符串</returns>
    public static string Decode(string token, byte[] secretKey, string algorithmName)
    {
        JwtBase jwtImpl = CreateImpl(algorithmName);
        return jwtImpl.Decode(token, secretKey);
    }


    /// <summary>
    /// 解析 JWT Token
    /// </summary>
    /// <param name="token">Token字符串</param>
    /// <param name="x509">用于校验Token的x509证书，如果为空则不做校验</param>
    /// <param name="algorithmName"></param>
    /// <returns>返回 payload 部分，是一个JSON字符串</returns>
    public static string Decode2(string token, X509Certificate2 x509, string algorithmName)
    {
        JwtBase jwtImpl = CreateImpl(algorithmName);
        return jwtImpl.Decode(token, x509);
    }


}
