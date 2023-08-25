namespace ClownFish.Base;

/// <summary>
/// 封装常用的HASH算法
/// </summary>
public static class HashHelper
{
    private static string HashString(HashAlgorithm hash, string text, Encoding encoding)
    {
        if( text == null )
            throw new ArgumentNullException(nameof(text));

        if( encoding == null )
            encoding = Encoding.UTF8;

        byte[] bb = encoding.GetBytes(text);
        byte[] buffer = hash.ComputeHash(bb);
        return buffer.ToHexString();
    }

    private static string HashFile(HashAlgorithm hash, string filePath)
    {
        if( string.IsNullOrEmpty(filePath) )
            throw new ArgumentNullException(nameof(filePath));
        if( RetryFile.Exists(filePath) == false )
            throw new FileNotFoundException("文件不存在：" + filePath);


        using( FileStream fs = RetryFile.OpenRead(filePath) ) {
            byte[] buffer = hash.ComputeHash(fs);
            return buffer.ToHexString();
        }
    }


    /// <summary>
    /// 计算字符串的 SHA1 签名
    /// </summary>
    /// <param name="text"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static string Sha1(this string text, Encoding encoding = null)
    {
        using( SHA1 sha1 = SHA1.Create() ) {
            return HashString(sha1, text, encoding);
        }
    }


    /// <summary>
    /// 计算字符串的 SHA256 签名
    /// </summary>
    /// <param name="text"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static string Sha256(this string text, Encoding encoding = null)
    {
        using( SHA256 sha1 = SHA256.Create() ) {
            return HashString(sha1, text, encoding);
        }
    }


    /// <summary>
    /// 计算字符串的 SHA512 签名
    /// </summary>
    /// <param name="text"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static string Sha512(this string text, Encoding encoding = null)
    {
        using( SHA512 sha1 = SHA512.Create() ) {
            return HashString(sha1, text, encoding);
        }
    }



    /// <summary>
    /// 计算文件的SHA1值
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string FileSha1(string filePath)
    {
        using( SHA1 sha1 = SHA1.Create() ) {
            return HashFile(sha1, filePath);
        }
    }


    /// <summary>
    /// 计算字符串的 MD5 签名
    /// </summary>
    /// <param name="text"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static string Md5(this string text, Encoding encoding = null)
    {
        using( MD5 md5 = MD5.Create() ) {
            return HashString(md5, text, encoding);
        }
    }


    /// <summary>
    /// 计算文件的MD5值
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string FileMD5(string filePath)
    {
        using( MD5 md5 = MD5.Create() ) {
            return HashFile(md5, filePath);
        }
    }



    /// <summary>
    /// 用HMACSHA256算法计算签名
    /// </summary>
    /// <param name="key"></param>
    /// <param name="data"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static string HMACSHA256(string key, string data, Encoding encoding = null)
    {
        if( encoding == null )
            encoding = Encoding.UTF8;

        using( HMACSHA256 hmac = new HMACSHA256(encoding.GetBytes(key)) ) {
            byte[] hashValue = hmac.ComputeHash(encoding.GetBytes(data));
            return Convert.ToBase64String(hashValue);
        }
    }



    /// <summary>
    /// 用HMACSHA256算法计算签名
    /// </summary>
    /// <param name="key"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static byte[] HMACSHA256(byte[] key, byte[] data)
    {
        using( HMACSHA256 hmac = new HMACSHA256(key) ) {
            return hmac.ComputeHash(data);
        }
    }


    /// <summary>
    /// 用HMACSHA512算法计算签名
    /// </summary>
    /// <param name="key"></param>
    /// <param name="data"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static string HMACSHA512(string key, string data, Encoding encoding = null)
    {
        if( encoding == null )
            encoding = Encoding.UTF8;

        using( HMACSHA512 hmac = new HMACSHA512(encoding.GetBytes(key)) ) {
            byte[] hashValue = hmac.ComputeHash(encoding.GetBytes(data));
            return Convert.ToBase64String(hashValue);
        }
    }



    /// <summary>
    /// 用HMACSHA512算法计算签名
    /// </summary>
    /// <param name="key"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static byte[] HMACSHA512(byte[] key, byte[] data)
    {
        using( HMACSHA512 hmac = new HMACSHA512(key) ) {
            return hmac.ComputeHash(data);
        }
    }

}
