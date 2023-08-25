namespace ClownFish.Base.Internals;

/// <summary>
/// 包含String类型相关的扩展方法
/// </summary>
public static class StringExtensions
{
    private static readonly TSafeDictionary<string, string> s_lowerDict = new TSafeDictionary<string, string>(1280);
    private static readonly TSafeDictionary<string, string> s_upperDict = new TSafeDictionary<string, string>(128);


    /// <summary>
    /// 将名称转小写，并尽量使用缓存
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string NameToLower(this string name)
    {
        if( name.IsNullOrEmpty() )
            return name;

        return s_lowerDict.GetOrAdd(name, ToLower);
    }


    private static string ToLower(string name)
    {
        return name.ToLower();
    }


    /// <summary>
    /// 将名称转大写，并尽量使用缓存
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string NameToUpper(this string name)
    {
        if( name.IsNullOrEmpty() )
            return name;

        return s_upperDict.GetOrAdd(name, ToUpper);
    }


    private static string ToUpper(string name)
    {
        return name.ToUpper();
    }
}
