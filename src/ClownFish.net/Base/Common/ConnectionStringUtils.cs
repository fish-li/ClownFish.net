namespace ClownFish.Base;

internal static class ConnectionStringUtils
{
    private static readonly Regex s_pwdRegex = new Regex(@"\b(password|pwd)=(?<value>[^,;]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static string HidePwd(string value)
    {
        if( string.IsNullOrEmpty(value) )
            return value;

        return s_pwdRegex.Replace(value, "$1=********");
    }



}
