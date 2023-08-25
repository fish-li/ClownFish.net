namespace ClownFish.Base;

/// <summary>
/// 对TripleDES封装的工具类（建议使用 AES 代替 TripleDES）
/// </summary>
public static class TripleDESHelper
{
    private static TripleDES GetProvider(string password)
    {
        TripleDES sa = TripleDES.Create();
        CryptoHelper.SetKeyIV(sa, password);
        return sa;
    }

    /// <summary>
    /// 使用TripleDES加密字符串
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
    /// 使用TripleDES加密字节数组
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
    /// 使用TripleDES解密一个以Base64编码的加密字符串
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
    /// 使用TripleDES解密字节数组
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




