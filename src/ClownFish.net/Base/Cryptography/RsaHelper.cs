namespace ClownFish.Base;

/// <summary>
/// RSA算法（签名/验证签名/加密/解密）的封装工具类。
/// 建议使用 X509Finder + X509Extensions 来代替这个类！
/// </summary>
public static class RsaHelper
{
    /// <summary>
    /// 根据证书主题查找证书，再对数据做签名
    /// </summary>
    /// <param name="data">需要做签名的数据</param>
    /// <param name="certSubject">证书主题</param>
    /// <returns></returns>
    internal static string Sign(byte[] data, string certSubject)
    {
        if( data == null )
            throw new ArgumentNullException(nameof(data));
        if( certSubject.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(certSubject));

        using X509Certificate2 x509 = X509Finder.FindBySubject(certSubject, true);
        return x509.Sign(data);
    }

    /// <summary>
    /// 用指定的证书对数据做签名
    /// </summary>
    /// <param name="data">需要做签名的数据</param>
    /// <param name="cert">X509Certificate2对象</param>
    /// <returns></returns>
    internal static string Sign(byte[] data, X509Certificate2 cert)
    {
        if( data == null )
            throw new ArgumentNullException(nameof(data));
        if( cert == null )
            throw new ArgumentNullException(nameof(cert));

        return cert.Sign(data);
    }


    /// <summary>
    /// 验证RSA签名
    /// </summary>
    /// <param name="data"></param>
    /// <param name="signature"></param>
    /// <param name="publicKey"></param>
    /// <returns></returns>
    internal static bool Verify(byte[] data, string signature, byte[] publicKey)
    {
        if( data == null )
            throw new ArgumentNullException(nameof(data));
        if( signature.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(signature));
        if( publicKey == null )
            throw new ArgumentNullException(nameof(publicKey));


        // 这里参数是 publicKey，而不是 certname 主要是为了方便，
        // publicKey 可以写死在代码中

        using X509Certificate2 x509 = X509Finder.LoadFromPublicKey(publicKey);
        return x509.Verify(data, signature);
    }

    /// <summary>
    /// 验证RSA签名
    /// </summary>
    /// <param name="data"></param>
    /// <param name="signature"></param>
    /// <param name="publicKeyText"></param>
    /// <returns></returns>
    internal static bool Verify(byte[] data, string signature, string publicKeyText)
    {
        // 这里参数是 publicKey，而不是 certname 主要是为了方便，
        // publicKey 可以写死在代码中

        using X509Certificate2 x509 = X509Finder.LoadFromPublicKey(publicKeyText);
        return x509.Verify(data, signature);
    }


    /// <summary>
    /// 验证RSA签名
    /// </summary>
    /// <param name="data"></param>
    /// <param name="signature"></param>
    /// <param name="cert"></param>
    /// <returns></returns>
    internal static bool Verify(byte[] data, string signature, X509Certificate2 cert)
    {
        if( data == null )
            throw new ArgumentNullException(nameof(data));
        if( signature.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(signature));
        if( cert == null )
            throw new ArgumentNullException(nameof(cert));

        return cert.Verify(data, signature);
    }









    /// <summary>
    /// RSA数据加密。
    /// 注意：这个方法只能加密比较短的内容（一般是密钥）
    /// </summary>
    /// <param name="data">二进制数据</param>
    /// <param name="certSubject">证书名称</param>
    /// <returns>加密后的数据</returns>
    internal static byte[] Encrypt(byte[] data, string certSubject)
    {
        if( data == null )
            throw new ArgumentNullException(nameof(data));
        if( certSubject.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(certSubject));

        using X509Certificate2 x509 = X509Finder.FindBySubject(certSubject, true);
        return x509.Encrypt(data);
    }



    /// <summary>
    /// RSA解密数据
    /// </summary>
    /// <param name="data"></param>
    /// <param name="certSubject"></param>
    /// <returns></returns>
    internal static byte[] Decrypt(byte[] data, string certSubject)
    {
        if( data == null )
            throw new ArgumentNullException(nameof(data));
        if( certSubject.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(certSubject));

        using X509Certificate2 x509 = X509Finder.FindBySubject(certSubject, true);
        return x509.Decrypt(data);
    }


    /// <summary>
    /// 用AES加密一个文本段，密钥为随机文本并用RSA加密
    /// </summary>
    /// <param name="x509"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string EncryptText(X509Certificate2 x509, string text)
    {
        if( x509 == null )
            throw new ArgumentNullException(nameof(x509));

        if( x509.HasPrivateKey )        // 增加这个检查可以防止把私钥写到字符串中，从而泄露私钥
            throw new ArgumentException("公钥证书中不允许包含私钥！");

        if( text.IsNullOrEmpty() )
            return string.Empty;

        // 每次加密时，生成一个随机密钥
        byte[] key = Guid.NewGuid().ToByteArray();

        // 用RSA加密"密钥"
        byte[] encKey = x509.Encrypt(key);
        string keyString = Convert.ToBase64String(encKey);


        // 用AES加密"字符串"
        string aesString = AesHelper.Encrypt(text, key.ToHexString());


        // 返回结果分为二个部分：前部分是加密后的密钥，后部分是加密后的文本
        return keyString + "." + aesString;
    }

    /// <summary>
    /// 解密由EncryptText()方法得到的结果
    /// </summary>
    /// <param name="x509"></param>
    /// <param name="encString"></param>
    /// <returns></returns>
    public static string DecryptText(X509Certificate2 x509, string encString)
    {
        if( x509 == null )
            throw new ArgumentNullException(nameof(x509));

        if( string.IsNullOrEmpty(encString) )
            return string.Empty;

        int p = encString.IndexOf('.');
        if( p <= 0 || p == encString.Length - 1 )
            throw new ArgumentException("字符串参数encString的格式不正确");


        string keyString = encString.Substring(0, p);
        string encData = encString.Substring(p + 1);

        byte[] encKey = null;
        try {
            encKey = Convert.FromBase64String(keyString);
        }
        catch {
            throw new ArgumentException("字符串参数encString的内容不正确，不能解析密钥。");
        }

        // 解密【加密密钥】，结果将用于AES解密
        byte[] key = x509.Decrypt(encKey);

        try {
            return AesHelper.Decrypt(encData, key.ToHexString());
        }
        catch( Exception ex ) {
            throw new InvalidDataException("解密数据失败。", ex);
        }
    }

}
