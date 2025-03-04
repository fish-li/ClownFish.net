using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace ClownFish.Base;

/// <summary>
/// 从Windows注册表中读取参数的工具类
/// </summary>
internal static class WinRegSetting
{
    internal static readonly string DefaultRegPath = @"HKEY_CURRENT_USER\SOFTWARE\ClownFish_LocalSettings\" + Path.GetFileNameWithoutExtension(AsmHelper.GetExeFilePath());

    private static string s_regPath = DefaultRegPath;

    /// <summary>
    /// 设置存储当前应用程序配置参数的Windows注册表路径，如果不指定将使用默认值：HKEY_CURRENT_USER\SOFTWARE\ClownFish_LocalSettings\appname
    /// </summary>
    /// <param name="regPath"></param>
    internal static void SetRegPath(string regPath)
    {
        s_regPath = regPath.HasValue() ? regPath : DefaultRegPath;
    }

    /// <summary>
    /// 从Windows注册表中读取一个配置参数，如果在Linux中运行则永远返回null
    /// </summary>
    /// <param name="name">参数名称</param>
    /// <returns></returns>
    internal static string GetSetting(string name)
    {
#if NETFRAMEWORK
        return Registry.GetValue(s_regPath, name, null)?.ToString();
#else
        if( RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ) {
            return Registry.GetValue(s_regPath, name, null)?.ToString();
        }
        else {
            // .net 6+ on Linux
            return null;
        }
#endif


    }
}
