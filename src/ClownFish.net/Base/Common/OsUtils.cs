using System.Runtime.InteropServices;

namespace ClownFish.Base;

/// <summary>
/// OsUtils
/// </summary>
public static class OsUtils
{
    /// <summary>
    /// 当前操作是否为 Windows
    /// </summary>
#if NETFRAMEWORK
    public static readonly bool IsWindows = true;
#else
    public static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#endif

    /// <summary>
    /// 当前操作是否为 Linux
    /// </summary>
#if NETFRAMEWORK
    public static readonly bool IsLinux = false;
#else
    public static readonly bool IsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
#endif


    /// <summary>
    /// 获取当前操作系统名称
    /// </summary>
    /// <returns></returns>
    public static string GetOsName()
    {
        if( OsUtils.IsLinux )
            return GetLinuxName();

        return Environment.OSVersion.ToString();
    }


    private static string GetLinuxName()
    {
        string filePath = "/etc/os-release";
        string text = File.Exists(filePath) ? File.ReadAllText(filePath) : null;
        return GetLinuxName0(text);
    }


    private static string GetLinuxName0(string text)
    {
        if( text.IsNullOrEmpty() )
            return "NULL";

        List<NameValue> list = (from line in text.ToLines()
                                let a = NameValue.Parse(line, '=')
                                where a != null
                                select new NameValue(a.Name, a.Value.Trim('"'))
                                ).ToList();

        return list.FirstOrDefault(x => x.Name == "PRETTY_NAME")?.Value ?? "UNKNOW-OS";
    }
}
