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
        if( OsUtils.IsLinux ) {
            if( s_linuxOsName == null )
                s_linuxOsName = GetLinuxName();
            return s_linuxOsName;
        }

        // 说明：低版本的.NET（例如： net45） 中 Environment.OSVersion.ToString() 得到的结果不正确，
        //      它在面对 Windows 8.1 或更高版本时，一律返回 Microsoft Windows NT 6.2.9200.0
        // 参考：https://learn.microsoft.com/zh-cn/dotnet/core/compatibility/core-libraries/5.0/environment-osversion-returns-correct-version
        //      https://learn.microsoft.com/zh-cn/windows/win32/sysinfo/targeting-your-application-at-windows-8-1

        // 另一个类似的属性 RuntimeInformation.OSDescription 直到 net471 才被支持

#if NET471_OR_GREATER
        return RuntimeInformation.OSDescription;
#else
        return Environment.OSVersion.ToString();
#endif
    }

    private static string s_linuxOsName = null;

    internal static string GetLinuxName()
    {
        // 参考：https://zhuanlan.zhihu.com/p/36253769  查看Linux发行版名称和版本号的8种方法



        /* 2个文件的内容不一样：
        [root@ElasticHost ~]# cat /etc/system-release
        CentOS Linux release 7.8.2003(Core)

        [root@ElasticHost ~]# cat /etc/os-release
        NAME = "CentOS Linux"
        VERSION = "7 (Core)"
        ID = "centos"
        ID_LIKE = "rhel fedora"
        VERSION_ID = "7"
        PRETTY_NAME = "CentOS Linux 7 (Core)"
        ANSI_COLOR = "0;31"
        CPE_NAME = "cpe:/o:centos:centos:7"
        HOME_URL = "https://www.centos.org/"
        BUG_REPORT_URL = "https://bugs.centos.org/"

        CENTOS_MANTISBT_PROJECT = "CentOS-7"
        CENTOS_MANTISBT_PROJECT_VERSION = "7"
        REDHAT_SUPPORT_PRODUCT = "centos"
        REDHAT_SUPPORT_PRODUCT_VERSION = "7"
        */

        string file1 = "/etc/system-release";  // 优先选择这个文件，因为它的版本信息更全
        if( File.Exists(file1) ) {
            return File.ReadAllText(file1).TrimEnd();  // CentOS 7/8 的这个文件末尾有个空行，所以必需调用TrimEnd()
        }

        string file2 = "/etc/os-release";
        IEnumerable<string> lines = File.Exists(file2) ? File.ReadLines(file2) : null;
        return GetLinuxName0(lines);
    }


    internal static string GetLinuxName0(IEnumerable<string> lines)
    {
        if( lines == null )
            return "UNKNOWN";

        var list = from line in lines
                   let a = NameValue.Parse(line, '=')
                   where a != null
                   select new NameValue(a.Name, a.Value.Trim('"'));

        return list.FirstOrDefault(x => x.Name == "PRETTY_NAME")?.Value ?? "UNKNOWN";
    }
}
