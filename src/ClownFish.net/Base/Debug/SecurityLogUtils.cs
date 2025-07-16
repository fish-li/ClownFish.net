namespace ClownFish.Base;

/// <summary>
/// 日志相关的安全防护工具类
/// </summary>
public static class SecurityLogUtils
{
    private static readonly Regex s_pwdRegex = new Regex(@"\b(password|pwd)=(?<value>[^,;]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// 隐藏连接字符串中的密码
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string HideConnectionStringPwd(string value)
    {
        if( string.IsNullOrEmpty(value) )
            return value;

        return s_pwdRegex.Replace(value, "$1=********");
    }


    private static readonly HashSet<string> s_hideEnvNames = LocalSettings.GetSetting("ClownFish_DebugReport_HideEnvNames").SplitToHashSet();

    /// <summary>
    /// 返回一个环境变量定义的字符串，它会隐藏部分敏感信息
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="nameFormatWidth"></param>
    /// <returns></returns>
    public static string GetEnvironmentVariableLine(string name, string value, int nameFormatWidth = 0)
    {
        string label = name;
        if( nameFormatWidth != 0 && nameFormatWidth != name.Length ) {
            // 这里的对齐方式和 .net 框架保持一致：  如果值为正，则字符串表示形式为右对齐；如果值为负，则为左对齐。
            // https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/tokens/interpolated#structure-of-an-interpolated-string
            if( nameFormatWidth > name.Length )
                label = name.PadLeft(nameFormatWidth, ' ');
            else if( nameFormatWidth < 0 )
                label = name.PadRight((0 - nameFormatWidth), ' ');
        }

        if( name.EndsWithIgnoreCase("ConnectionString") )
            return $"{label}: {SecurityLogUtils.HideConnectionStringPwd(value)}";
        else if( name.EndsWithIgnoreCase("Password") )
            return $"{label}: ********";
        else if( name.EndsWithIgnoreCase("_key") )
            return $"{label}: ********";
        else if( s_hideEnvNames.Contains(name) )
            return $"{label}: ********";
        else
            return $"{label}: {value}";
    }

}
