using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace ClownFish.Base;

/// <summary>
/// 提供一些访问系统环境信息的工具类
/// </summary>
internal static class SystemHelper
{
    /// <summary>
    /// 获取计算机名称
    /// </summary>
    /// <returns></returns>
    public static string GetComputerName()
    {
        // 注意：这段代码需要在Windows XP及较新版本的操作系统中才能正常运行。
        //SelectQuery query = new SelectQuery("SELECT PartOfDomain, DNSHostName, Domain FROM  Win32_ComputerSystem");
        //using( ManagementObjectSearcher searcher = new ManagementObjectSearcher(query) ) {
        //    foreach( ManagementObject mo in searcher.Get() ) {
        //        if( (bool)mo["PartOfDomain"] )
        //            return mo["DNSHostName"].ToString() + "." + mo["Domain"].ToString();
        //    }
        //}

        // 因为跨平台的原因，这里简化处理
        return System.Environment.MachineName;
    }

    /// <summary>
    /// 获取有效的网卡信息
    /// </summary>
    /// <returns></returns>
    public static NetworkInterface GetCurrentNetworkInfo()
    {
        if( NetworkInterface.GetIsNetworkAvailable() == false )
            return null;

        // 参考链接
        // https://stackoverflow.com/questions/6803073/get-local-ip-address/28621250#28621250

        NetworkInterface[] itmes = NetworkInterface.GetAllNetworkInterfaces();

        return GetCurrentNetworkInfo1(itmes.Where(x => x.NetworkInterfaceType == NetworkInterfaceType.Ethernet))
               ?? GetCurrentNetworkInfo1(itmes.Where(x => x.NetworkInterfaceType == NetworkInterfaceType.Wireless80211));
    }


    private static NetworkInterface GetCurrentNetworkInfo1(IEnumerable<NetworkInterface> itmes)
    {
        foreach( NetworkInterface item in itmes.Where(x => x.OperationalStatus == OperationalStatus.Up) ) {
            IPInterfaceProperties props = item.GetIPProperties();

            if( props.GatewayAddresses.FirstOrDefault() != null ) {
                foreach( UnicastIPAddressInformation ip in props.UnicastAddresses ) {
                    if( ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ) {

                        return item;
                    }
                }
            }
        }

        return null;
    }


    public static string GetMac(this NetworkInterface item)
    {
        if( item == null )
            return "NOTFOUND";

        return item.GetPhysicalAddress().ToString();
    }

    public static string GetIPv4(this NetworkInterface item)
    {
        if( item == null )
            return "NOTFOUND";

        IPInterfaceProperties props = item.GetIPProperties();

        foreach( UnicastIPAddressInformation ip in props.UnicastAddresses ) {
            if( ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ) {
                return ip.Address.ToString();
            }
        }

        return "NOTFOUND";
    }



    /// <summary>
    /// 获取当前操作系统名称
    /// </summary>
    /// <returns></returns>
    public static string GetOsName()
    {
#if NETCOREAPP
        if( RuntimeInformation.IsOSPlatform(OSPlatform.Windows) )
            return "Windows";

        if( RuntimeInformation.IsOSPlatform(OSPlatform.Linux) )
            return "Linux";

        if( RuntimeInformation.IsOSPlatform(OSPlatform.OSX) )
            return "OSX";
#endif
        return "Windows";
    }




}
