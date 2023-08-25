namespace ClownFish.Base;

/// <summary>
/// RSA算法（签名/验证签名/加密/解密）的封装工具类
/// </summary>
public static class X509Extensions
{
    /// <summary>
    /// 用X509证书对数据做签名
    /// </summary>
    /// <param name="cert"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string Sign(this X509Certificate2 cert, byte[] data)
    {
        if( cert == null )
            throw new ArgumentNullException(nameof(cert));
        if( data == null )
            throw new ArgumentNullException(nameof(data));


        if( cert.HasPrivateKey == false )
            throw new ArgumentException("指定的证书没有包含私钥：" + cert.Subject);


        using RSA rsa = (RSA)cert.GetRSAPrivateKey();

        //计算数据哈希值
        using SHA1 sha1 = SHA1.Create();
        byte[] hash = sha1.ComputeHash(data);

        //签名数据
        byte[] bb = rsa.SignHash(hash, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);

        return Convert.ToBase64String(bb);
    }


    /// <summary>
    /// 用X509证书验证数据签名
    /// </summary>
    /// <param name="cert"></param>
    /// <param name="data"></param>
    /// <param name="signature"></param>
    /// <returns></returns>
    public static bool Verify(this X509Certificate2 cert, byte[] data, string signature)
    {
        if( cert == null )
            throw new ArgumentNullException(nameof(cert));
        if( data == null )
            throw new ArgumentNullException(nameof(data));
        if( string.IsNullOrEmpty(signature) )
            throw new ArgumentNullException(nameof(signature));


        // 获得证书公钥
        using RSA rsa = (RSA)cert.GetRSAPublicKey();

        // 哈希数据
        using SHA1 sha1 = SHA1.Create();
        byte[] hash = sha1.ComputeHash(data);

        // 验证哈希签名
        byte[] bb = Convert.FromBase64String(signature);

        return rsa.VerifyHash(hash, bb, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
    }


    /// <summary>
    /// 用X509证书加密数据。
    /// 注意：这个方法只能加密比较短的内容（一般是密钥）
    /// </summary>
    /// <param name="cert"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static byte[] Encrypt(this X509Certificate2 cert, byte[] data)
    {
        if( cert == null )
            throw new ArgumentNullException(nameof(cert));
        if( data == null )
            throw new ArgumentNullException(nameof(data));

        // 获得证书公钥
        using RSA rsa = (RSA)cert.GetRSAPublicKey();

        return rsa.Encrypt(data, RSAEncryptionPadding.Pkcs1);
    }


    /// <summary>
    /// 用X509证书解密数据
    /// </summary>
    /// <param name="cert"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static byte[] Decrypt(this X509Certificate2 cert, byte[] data)
    {
        if( cert == null )
            throw new ArgumentNullException(nameof(cert));
        if( data == null )
            throw new ArgumentNullException(nameof(data));


        if( cert.HasPrivateKey == false )
            throw new ArgumentException("证书没有私钥。");


        // 读取证书私钥
        using RSA rsa = (RSA)cert.GetRSAPrivateKey();

        return rsa.Decrypt(data, RSAEncryptionPadding.Pkcs1);
    }




}
