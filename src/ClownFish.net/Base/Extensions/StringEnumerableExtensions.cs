namespace ClownFish.Base;

/// <summary>
/// 字符串枚举扩展方法工具类
/// </summary>
public static class StringEnumerableExtensions
{
    /// <summary>
    /// 将字符串数组合并成单一的字符串
    /// </summary>
    /// <param name="items"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static string Merge(this IEnumerable<string> items, string separator)
    {
        if( items == null )
            return string.Empty;

        return string.Join(separator, items);
    }


    /// <summary>
    /// 判断集合中的字符串是不是有效的名称，即只包含：字母，数字，下划线。
    /// </summary>
    /// <param name="lines"></param>
    public static void CheckNames(this IEnumerable<string> lines)
    {
        if( lines == null )
            return;

        foreach( string text in lines ) {  // 循环内直接复制上面方法的代码，手工强制【内联】

            if( string.IsNullOrEmpty(text) )
                continue;

            foreach( char ch in text ) {
                if( (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9') || ch == '_' )
                    continue;
                else
                    throw new ArgumentOutOfRangeException(nameof(text), "参数包含了不允许的字符。");
            }
        }
    }
}
