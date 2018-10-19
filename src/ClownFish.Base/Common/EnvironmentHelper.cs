using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using Microsoft.Win32;

namespace ClownFish.Base
{
    /// <summary>
    /// 提供一些访问环境信息的工具类
    /// </summary>
    public static class EnvironmentHelper
    {
        /// <summary>
        /// 获取计算机名称
        /// </summary>
        /// <returns></returns>
        public static string GetComputerName()
        {
            // 注意：这段代码需要在Windows XP及较新版本的操作系统中才能正常运行。
            SelectQuery query = new SelectQuery("SELECT PartOfDomain, DNSHostName, Domain FROM  Win32_ComputerSystem");
            using( ManagementObjectSearcher searcher = new ManagementObjectSearcher(query) ) {
                foreach( ManagementObject mo in searcher.Get() ) {
                    if( (bool)mo["PartOfDomain"] )
                        return mo["DNSHostName"].ToString() + "." + mo["Domain"].ToString();
                }
            }

            return System.Environment.MachineName;
        }

        /// <summary>
        /// 获取网卡MAC地址
        /// </summary>
        /// <returns></returns>
        public static string GetMacAddress()
        {
            if( NetworkInterface.GetIsNetworkAvailable() == false )
                return "NetworkNotReady";

            // 优先查找有网线的网卡
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach( NetworkInterface adapter in nics ) {
                if( adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet )
                    return adapter.GetPhysicalAddress().ToString();
            }


            // 再查找无线网卡
            foreach( NetworkInterface adapter in nics ) {
                if( adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 )
                    return adapter.GetPhysicalAddress().ToString();
            }

            return "NOTFOUND";
        }


        /// <summary>
        /// 判断操作系统是否为64位Windows
        /// </summary>
        /// <returns></returns>
        public static bool IsWin64()
        {
            // 为了简单，直接判断一个特殊的目录

            string path = Path.Combine(Environment.SystemDirectory, @"..\SysWOW64");

            return Directory.Exists(path);

        }

        /// <summary>
        /// 获取IE版本
        /// </summary>
        /// <returns></returns>
        public static string GetIeVersion()
        {
            using( RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Internet Explorer") ) {
                if( key != null ) {
                    string version = key.GetValue("svcVersion", null) as string;
                    if( string.IsNullOrEmpty(version) == false )
                        return version;


                    version = key.GetValue("Version", null) as string;
                    if( string.IsNullOrEmpty(version) == false )
                        return version;
                }
            }

            return "NOTFOUND";
        }


    }
}
