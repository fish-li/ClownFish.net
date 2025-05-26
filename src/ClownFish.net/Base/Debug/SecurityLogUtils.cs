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


    private static readonly HashSet<string> s_hideEnvNames = LocalSettings.GetSetting("DebugReport_HideEnvNames").SplitToHashSet();

    /// <summary>
    /// 返回一个环境变量定义的字符串，它会隐藏部分敏感信息
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string GetEnvironmentVariableLine(string key, string value)
    {
        if( key.EndsWithIgnoreCase("ConnectionString") )
            return $"{key}: {SecurityLogUtils.HideConnectionStringPwd(value)}";
        else if( key.EndsWithIgnoreCase("Password") )
            return $"{key}: ********";
        else if( key.EndsWithIgnoreCase("_key") )
            return $"{key}: ********";
        else if( s_hideEnvNames.Contains(key) )
            return $"{key}: ********";
        else
            return $"{key}: {value}";
    }

}
