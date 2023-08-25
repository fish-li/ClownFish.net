namespace ClownFish.Base;

/// <summary>
/// 对AES算法封装的工具类
/// </summary>
public static class AesHelper
{
    private static Aes GetProvider(string password)
    {
        Aes sa = Aes.Create();
        CryptoHelper.SetKeyIV(sa, password);
        return sa;
    }

    /// <summary>
    /// 使用AES算法加密字符串
    /// </summary>
    /// <param name="text"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static string Encrypt(string text, string password)
    {
        using( SymmetricAlgorithm sa = GetProvider(password) ) {
            return CryptoHelper.Encrypt(text, sa);
        }
    }
    /// <summary>
    /// 使用AES算法加密字节数组
    /// </summary>
    /// <param name="input"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static byte[] Encrypt(byte[] input, string password)
    {
        using( SymmetricAlgorithm sa = GetProvider(password) ) {
            return CryptoHelper.Encrypt(input, sa);
        }
    }

    /// <summary>
    /// 使用AES算法解密一个以Base64编码的加密字符串
    /// </summary>
    /// <param name="base64"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static string Decrypt(string base64, string password)
    {
        using( SymmetricAlgorithm sa = GetProvider(password) ) {
            return CryptoHelper.Decrypt(base64, sa);
        }
    }

    /// <summary>
    /// 使用AES算法解密字节数组
    /// </summary>
    /// <param name="input"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static byte[] Decrypt(byte[] input, string password)
    {
        using( SymmetricAlgorithm sa = GetProvider(password) ) {
            return CryptoHelper.Decrypt(input, sa);
        }
    }
}




