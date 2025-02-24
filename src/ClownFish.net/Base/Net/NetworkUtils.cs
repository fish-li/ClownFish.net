using System.Net.NetworkInformation;

namespace ClownFish.Base.Net;
/// <summary>
/// 
/// </summary>
public static class NetworkUtils
{
    /// <summary>
    /// 获取本机IP
    /// </summary>
    /// <returns></returns>
    public static string GetLocalIp()
    {
        // 万一遇到极端情况，可以人工指定一个配置参数，程序就不再执行下面的自动获取IP逻辑
        string ipAddr = LocalSettings.GetSetting("LocalIpAddrV4");
        if( ipAddr.HasValue() )
            return ipAddr;

        if( NetworkInterface.GetIsNetworkAvailable() == false )
            return "127.0.0.1";


        NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

        // 判断逻辑：
        // 1，挑选那些有设置 “网关” 的网络
        // 2，优先选择有线网络

        IPInterfaceProperties adapterProperties = (from x in interfaces.Where(x1 => x1.OperationalStatus == OperationalStatus.Up)
                                                   let x2 = x.GetIPProperties()
                                                   where x2 != null && x2.GatewayAddresses != null && x2.GatewayAddresses.Count > 0
                                                   orderby x.NetworkInterfaceType
                                                   select x2).FirstOrDefault();

        if( adapterProperties != null ) {
            UnicastIPAddressInformationCollection uniCast = adapterProperties.UnicastAddresses;
            foreach( UnicastIPAddressInformation uni in uniCast ) {
                // 一般一块网卡有2个地址，一个是IPv4/InterNetwork，一个是IPv6/InterNetworkV6
                if( uni.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ) {
                    return uni.Address.ToString();
                }
            }
        }

        //Console2.Info("No active network connection detected.");
        return "127.0.0.1";
    }


    /// <summary>
    /// 判断一个IP地址是不是 局域网IP或者本机IP
    /// </summary>
    /// <param name="hostIp"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLanIP(string hostIp)
    {
        if( IPAddress.TryParse(hostIp, out IPAddress ip) ) {
            return IsLanIP(ip);
        }

        return false;
    }


    /// <summary>
    /// 判断一个IP地址是不是 局域网IP或者本机IP
    /// </summary>
    /// <param name="ip"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLanIP(IPAddress ip)
    {
        // 局域网IP地址范围（3段）：
        // A类：10段，     后三位自由分配， 10.0.0.0 - 10.255.255.255
        // B类：172.16段， 后两位自由分配， 172.16.0.0 - 172.31.255.255
        // C类：192.168段，后两位自由分配， 192.168.0.0 - 192.168.255.255
        byte[] bb = ip.GetAddressBytes();
        return (bb[0] == 10
                || (bb[0] == 172 && (bb[1] >= 16 && bb[1] <= 31))
                || (bb[0] == 192 && bb[1] == 168)
                || IPAddress.IsLoopback(ip));
    }

}
