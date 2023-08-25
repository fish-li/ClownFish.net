namespace ClownFish.Base;

/// <summary>
/// StringBuilder扩展方法工具类
/// </summary>
public static class StringBuilderExtensions
{

    /// <summary>
    /// 基本等效于 sb.AppendLine(...)，只是换行符固定为Windows的换行符"\r\n"
    /// </summary>
    /// <param name="sb">StringBuilder实例</param>
    /// <param name="value">需要追加的字符串</param>
    /// <returns></returns>
    public static StringBuilder AppendLineRN(this StringBuilder sb, string value = null)
    {
        if( sb == null )
            throw new ArgumentNullException(nameof(sb));

        if( value != null )
            sb.Append(value);

        sb.Append("\r\n");
        return sb;
    }


    /// <summary>
    /// 等同于 AppendLine
    /// </summary>
    /// <param name="sb"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static StringBuilder WriteLine(this StringBuilder sb, string value = null)
    {
        if( sb == null )
            throw new ArgumentNullException(nameof(sb));

        sb.AppendLine(value);
        return sb;
    }

}
