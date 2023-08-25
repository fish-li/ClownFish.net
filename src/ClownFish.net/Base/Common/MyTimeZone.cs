using System.Runtime.InteropServices;

namespace ClownFish.Base;

/// <summary>
/// 时区工具类
/// </summary>
public static class MyTimeZone
{
    private class TZMapping
    {
        public string Windows { get; set; }

        public string Linux { get; set; }
    }

    internal static readonly bool OsIsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    // https://dejanstojanovic.net/aspnet/2018/july/differences-in-time-zones-in-net-core-on-windows-and-linux-host-os/

    private static readonly Dictionary<string, string> s_win = new Dictionary<string, string>(256, StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, string> s_linux = new Dictionary<string, string>(256, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// 当前时区名称，Linux 风格，例如："Asia/Shanghai"
    /// </summary>
    public static readonly string CurrentTZ;

    static MyTimeZone()
    {
        string json = typeof(MyTimeZone).Assembly.ReadResAsText("ClownFish.WindowsLinuxTimeZone.json");
        List<TZMapping> list = json.FromJson<List<TZMapping>>();

        foreach( var x in list ) {
            s_win[x.Windows] = x.Linux;
            s_linux[x.Linux] = x.Windows;
        }


#if NETFRAMEWORK
        CurrentTZ = s_win[TimeZoneInfo.Local.Id];
#else
        if( OsIsWindows )
            CurrentTZ = s_win[TimeZoneInfo.Local.Id];
        else
            CurrentTZ = TimeZoneInfo.Local.Id;
#endif
    }


    /// <summary>
    /// 根据一个 TimeZoneInfo.Id 获取对应的 TimeZoneInfo 实例
    /// </summary>
    /// <param name="tzId">时区ID，可以是 “China Standard Time” 也可以是 “Asia/Shanghai” </param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static TimeZoneInfo Get(string tzId)
    {
        if( tzId.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(tzId));

        if( OsIsWindows ) {
            return GetTzForWin(tzId);
        }
        else {
            return GetTzForLinux(tzId);
        }
    }


    internal static TimeZoneInfo GetTzForWin(string tzId)
    {
        bool isLinuxStyle = tzId.IndexOf('/') > 0;
        if( isLinuxStyle ) {
            string id2 = s_linux.TryGet(tzId) ?? tzId;
            return TimeZoneInfo.FindSystemTimeZoneById(id2);
        }
        else {
            return TimeZoneInfo.FindSystemTimeZoneById(tzId);
        }
    }

    internal static TimeZoneInfo GetTzForLinux(string tzId)
    {
        bool isLinuxStyle = tzId.IndexOf('/') > 0;
        if( isLinuxStyle ) {
            return TimeZoneInfo.FindSystemTimeZoneById(tzId);
        }
        else {
            string id2 = s_win.TryGet(tzId) ?? tzId;
            return TimeZoneInfo.FindSystemTimeZoneById(id2);
        }
    }


}
