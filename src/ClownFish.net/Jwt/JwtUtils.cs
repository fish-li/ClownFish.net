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


    internal static JwtBase GetImpl(string algorithmName)
    {
        if( algorithmName.IsNullOrEmpty() )
            algorithmName = DefaultAlgorithm;

        return algorithmName switch {
            JwtHMACSHA256.AlgorithmName => JwtHMACSHA256.Instance,
            JwtHMACSHA512.AlgorithmName => JwtHMACSHA512.Instance,
#if NET46_OR_GREATER|| NETCOREAPP
            JwtRSA256.AlgorithmName => JwtRSA256.Instance,
            JwtRSA512.AlgorithmName => JwtRSA512.Instance,
#endif
#if NET461_OR_GREATER|| NETCOREAPP
            JwtECD256.AlgorithmName => JwtECD256.Instance,
            JwtECD512.AlgorithmName => JwtECD512.Instance,
#endif
            _ => JwtExtMananger.GetImpl(algorithmName) ?? throw new NotSupportedException("不支持的JWT签名算法：" + algorithmName)
        };
    }


    /// <summary>
    /// 注册扩展的JWT实现
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static void RegisterAlgorithmImpl<T>() where T : IJwtAlgorithm2, new()
    {
        IJwtAlgorithm2 impl = new T();
        JwtExtMananger.RegisterAlgorithmImpl(impl);
    }

    /// <summary>
    /// 创建一个 JWT Token 字符串
    /// </summary>
    /// <param name="payloadJson"></param>
    /// <param name="secretKey">密钥</param>
    /// <param name="algorithmName">算法名称</param>
    /// <returns></returns>
    public static string Encode(string payloadJson, byte[] secretKey, string algorithmName)
    {
        JwtBase jwtImpl = GetImpl(algorithmName);
        return jwtImpl.Encode(payloadJson, secretKey);
    }

    /// <summary>
    /// 创建一个 JWT Token 字符串
    /// </summary>
    /// <param name="headerJson"></param>
    /// <param name="payloadJson"></param>
    /// <param name="secretKey">密钥</param>
    /// <param name="algorithmName">算法名称</param>
    /// <returns></returns>
    public static string Encode(string headerJson, string payloadJson, byte[] secretKey, string algorithmName)
    {
        JwtBase jwtImpl = GetImpl(algorithmName);
        return jwtImpl.Encode(headerJson, payloadJson, secretKey);
    }

    /// <summary>
    /// 创建一个 JWT Token 字符串
    /// </summary>
    /// <param name="payloadJson"></param>
    /// <param name="x509">x509证书</param>
    /// <param name="algorithmName">算法名称</param>
    /// <returns></returns>
    public static string Encode2(string payloadJson, X509Certificate2 x509, string algorithmName)
    {
        JwtBase jwtImpl = GetImpl(algorithmName);
        return jwtImpl.Encode(payloadJson, x509);
    }


    /// <summary>
    /// 创建一个 JWT Token 字符串
    /// </summary>
    /// <param name="headerJson"></param>
    /// <param name="payloadJson"></param>
    /// <param name="x509">x509证书</param>
    /// <param name="algorithmName">算法名称</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string Encode2(string headerJson, string payloadJson, X509Certificate2 x509, string algorithmName)
    {
        JwtBase jwtImpl = GetImpl(algorithmName);
        return jwtImpl.Encode(headerJson, payloadJson, x509);
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
        JwtBase jwtImpl = GetImpl(algorithmName);
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
        JwtBase jwtImpl = GetImpl(algorithmName);
        return jwtImpl.Decode(token, x509);
    }


    /// <summary>
    /// JWT Base64 UrlEncode
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string Base64UrlEncode(this byte[] input)
    {
        if( input.IsNullOrEmpty() )
            return string.Empty;

        return NbJwtBase64UrlEncoder.Encode(input);
    }


    /// <summary>
    /// JWT Base64 UrlEncode
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string Base64UrlEncode(this string input)
    {
        if( input.IsNullOrEmpty() )
            return string.Empty;

        byte[] bytes = Encoding.UTF8.GetBytes(input);
        return NbJwtBase64UrlEncoder.Encode(bytes);
    }


    /// <summary>
    /// JWT Base64 UrlDecode
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string Base64UrlDecode(this string input)
    {
        if( input.IsNullOrEmpty() )
            return input;

        return NbJwtBase64UrlEncoder.Decode(input).ToUtf8String();
    }
}
