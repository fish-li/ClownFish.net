namespace ClownFish.Jwt;

/// <summary>
/// JWT自定义类的接口
/// </summary>
public interface IJwtAlgorithm2
{
    /// <summary>
    /// 实现类支持的算法名称，例如：SM3
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 获取【默认的】JWT头部 JSON 字符串，例如：{"typ": "JWT", "alg": "SM3"}
    /// </summary>
    /// <returns></returns>
    string GetHeaderJson();

    /// <summary>
    /// 计算签名
    /// </summary>
    /// <param name="secret">签名使用的密钥，只可能是：byte[] or X509Certificate2 ，具体哪种由签名算法来决定</param>
    /// <param name="bytesToSign">需要计算签名的数据</param>
    /// <returns></returns>
    string GetSignature(object secret, byte[] bytesToSign);

    /// <summary>
    /// 验证签名。 如果签名验证不匹配，应该抛出 SignatureVerificationException
    /// </summary>
    /// <param name="secret">签名使用的密钥，只可能是：byte[] or X509Certificate2 ，具体哪种由签名算法来决定</param>
    /// <param name="bytesToSign">需要验证签名的数据</param>
    /// <param name="signature">签名</param>
    void ValidSignature(object secret, byte[] bytesToSign, string signature);
}
