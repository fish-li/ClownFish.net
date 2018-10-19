using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using ClownFish.Base;
using Microsoft.Win32;

namespace ClownFish.Log.Model
{
    /// <summary>
    /// 描述机器环境信息的数据类型
    /// </summary>
    public sealed class EnvironmentInfo
    {
        /// <summary>
        /// 机器名称
        /// </summary>
        public string ComputerName { get; set; }

        /// <summary>
        /// MAC地址
        /// </summary>
        public string Mac { get; set; }
        /// <summary>
        /// 当前用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Windows版本
        /// </summary>
        public string WinVersion { get; set; }
        /// <summary>
        /// IE版本
        /// </summary>
        public string IeVersion { get; set; }
        /// <summary>
        /// CPU架构，32位还是64位
        /// </summary>
        public string CPU { get; set; }




        private static EnvironmentInfo s_instance;
        private static readonly object s_lock = new object();


        /// <summary>
        /// 获取当前机器环境信息
        /// </summary>
        /// <returns></returns>
        public static EnvironmentInfo GetCurrent()
        {
            if( s_instance == null ) {
                lock( s_lock ) {
                    if( s_instance == null ) {

                        EnvironmentInfo info = new EnvironmentInfo();
                        info.ComputerName = EnvironmentHelper.GetComputerName();
                        info.Mac = EnvironmentHelper.GetMacAddress();
                        info.UserName = Environment.UserName;
                        info.WinVersion = Environment.OSVersion.ToString();
                        info.IeVersion = EnvironmentHelper.GetIeVersion();
                        info.CPU = EnvironmentHelper.IsWin64() ? "X64" : "X86";
                        s_instance = info;
                    }
                }
            }

            return s_instance;
        }

        
    }
}
