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
        if( IPAddress.IsLoopback(ip) )
            return true;

        byte[] bb = ip.GetAddressBytes();

        if( bb[0] == 10 )    // 私有地址（RFC 1918） 10.0.0.0 ~ 10.255.255.255
            return true;

        if( bb[0] == 172 && (bb[1] >= 16 && bb[1] <= 31) )   // 私有地址（RFC 1918）172.16.0.0 ~ 172.31.255.255
            return true;

        if( bb[0] == 192 && bb[1] == 168 )   // 私有地址（RFC 1918）192.168.0.0 ~ 192.168.255.255
            return true;

        if( bb[0] == 100 && (bb[1] >= 64 && bb[1] <= 127) )   // 运营商级 NAT 地址（RFC 6598） 100.64.0.0 ~ 100.127.255.255
            return true;

        if( bb[0] == 127 )    // 环回地址（RFC 5735）127.0.0.0 ~ 127.255.255.255
            return true;

        if( bb[0] == 169 && bb[1] == 254 )   // 自动配置地址（RFC 3927）169.254.0.0 ~ 169.254.255.255
            return true;

        if( bb[0] == 192 && bb[1] == 0 && bb[2] == 2 )   // 文档/示例地址（RFC 5737）192.0.2.0 ~ 192.0.2.255
            return true;

        if( bb[0] == 198 && bb[1] == 51 && bb[2] == 100 )   // 文档/示例地址（RFC 5737）198.51.100.0 ~ 198.51.100.255
            return true;

        if( bb[0] == 203 && bb[1] == 0 && bb[2] == 113 )   // 文档/示例地址（RFC 5737）203.0.113.0 ~ 203.0.113.255
            return true;

        // 目前只判断 下面列表中【前5类】

        return false;
    }

}



/*   非公网IP范围
 * 
1. 私有地址（RFC 1918）
   10.0.0.0/8（10.0.0.0 ~ 10.255.255.255）           大型内部网络专用
   172.16.0.0/12（172.16.0.0 ~ 172.31.255.255）      中型内部网络专用
   192.168.0.0/16（192.168.0.0 ~ 192.168.255.255）   小型网络（如家庭/办公室）专用

2. 运营商级 NAT 地址（RFC 6598）
   100.64.0.0/10（100.64.0.0 ~ 100.127.255.255）     运营商级 NAT（CGNAT）地址，ISP 用于为多个用户共享公网 IP

3. 环回地址（RFC 5735）
   127.0.0.0/8（127.0.0.0 ~ 127.255.255.255）        本机通信（如 127.0.0.1 指向本机）

4. 自动配置地址（RFC 3927）
   169.254.0.0/16（169.254.0.0 ~ 169.254.255.255）   DHCP 失败时自动分配，仅限本地链路通信（不可跨网段）

5. 文档/示例地址（RFC 5737）                          文档或示例中使用的保留地址，实际网络不可部署。
   192.0.2.0/24（192.0.2.0 ~ 192.0.2.255）
   198.51.100.0/24（198.51.100.0 ~ 198.51.100.255）
   203.0.113.0/24（203.0.113.0 ~ 203.0.113.255）

6. 多播地址（RFC 5771）                               多播通信（如视频会议、路由协议 OSPF 等）
   224.0.0.0/4（224.0.0.0 ~ 239.255.255.255）

7. 保留地址（RFC 3330/6890）                          保留给未来使用，当前不可分配
   240.0.0.0/4（240.0.0.0 ~ 255.255.255.254）

8. 特殊用途地址
   0.0.0.0/8（0.0.0.0 – 0.255.255.255）               表示无效或默认路由
   192.0.0.0/24（192.0.0.0 – 192.0.0.255）            保留给 IANA 特殊协议（如 IPv4 到 IPv6 过渡技术）
   192.88.99.0/24（192.88.99.0 – 192.88.99.255）      用于 IPv6 到 IPv4 的中继（6to4 任播）
   255.255.255.255                                    仅限本地网络广播

*/