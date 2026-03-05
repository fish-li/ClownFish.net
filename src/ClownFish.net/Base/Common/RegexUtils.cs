namespace ClownFish.Base;

/// <summary>
/// 
/// </summary>
public static partial class RegexUtils
{
#if NET7_0_OR_GREATER
    [GeneratedRegex(@"{(\w+)}", RegexOptions.None, "en-US")]
    private static partial Regex GetRegex1();
#else
    private static readonly Regex s_regex = new Regex(@"{(\w+)}", RegexOptions.Compiled);
    private static Regex GetRegex1() => s_regex;
#endif


    /// <summary>
    /// 检查某个URL是否包含了占位符模式，例如：/page/{id}/{year}-{month}-{day}.aspx
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static bool HasRouteName(string url)
    {
        return GetRegex1().IsMatch(url);
    }

    /// <summary>
    /// 将包含了占位符模式的字符串翻译成等效的正则表达式，通常用于路由匹配，例如：/page/{id}/{year}-{month}-{day}.aspx
    /// </summary>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public static Regex CreateRouteRegex(string pattern)
    {
        string newString = GetRegex1().Replace(pattern, @"(?<$1>\w+)");

        if( newString[0] != '^' )
            newString = "^" + newString;

        return new Regex(newString, RegexOptions.Compiled | RegexOptions.IgnoreCase);


        // input:   /page/{id}/{year}-{month}-{day}.aspx
        // output:  ^/page/(?<id>\w+)/(?<year>\w+)-(?<month>\w+)-(?<day>\w+).aspx
    }

}
