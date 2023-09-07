using System.Net.NetworkInformation;

namespace ClownFish.WebHost.Utils;

/// <summary>
/// 包含一些网络相关的工具方法
/// </summary>
public static class NetHelper
{
    /// <summary>
    /// 判断某个TCP端口是否在使用中
    /// </summary>
    /// <param name="port">要检测的TCP端口</param>
    /// <returns></returns>
    public static bool TcpPortIsUse(int port)
    {
        IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
        IPEndPoint[] endPoints = properties.GetActiveTcpListeners();

        return endPoints.FirstOrDefault(x => x.Port == port) != null;
    }


    /// <summary>
    /// 获取一个空间的TCP端口，并要求在某个区间范围内。
    /// 如果指定范围的所有端口全部占用，将返回 0
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int GetFreeTcpPort(int min, int max)
    {
        if( min <= 0 || max <= 0 || max <= min )
            throw new ArgumentException("参数范围无效。");

        IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
        IPEndPoint[] endPoints = properties.GetActiveTcpListeners();

        for(int n = min; n <= max; n++ ) {
            var e = endPoints.FirstOrDefault(x => x.Port == n);
            if( e == null )
                return n;
        }

        return 0;
    }

}
