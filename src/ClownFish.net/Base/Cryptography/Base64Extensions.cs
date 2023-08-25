namespace ClownFish.Base;

/// <summary>
/// BASE64编码的工具类
/// </summary>
public static class Base64Extensions
{
    /// <summary>
    /// 将字符串做BASE64编码
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string ToBase64(this string input)
    {
        if( string.IsNullOrEmpty(input) )
            return input;

        byte[] bb = Encoding.UTF8.GetBytes(input);
        return Convert.ToBase64String(bb);
    }

    /// <summary>
    /// 从BASE64文本中还原结果
    /// </summary>
    /// <param name="base64"></param>
    /// <param name="throwError"></param>
    /// <returns></returns>
    public static string FromBase64(this string base64, bool throwError = false)
    {
        if( string.IsNullOrEmpty(base64) )
            return base64;

        if( throwError ) {
            byte[] bb = Convert.FromBase64String(base64);
            return Encoding.UTF8.GetString(bb);
        }

        try {
            byte[] bb = Convert.FromBase64String(base64);
            return Encoding.UTF8.GetString(bb);
        }
        catch {     // 这个异常没有必要抛出，因为肯定是人为修改了数据                
            return null;
        }
    }
}
