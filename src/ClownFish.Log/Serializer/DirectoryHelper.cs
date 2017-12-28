using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Log.Configuration;

namespace ClownFish.Log.Serializer
{
    /// <summary>
    /// 日志目录的辅助工具类
    /// </summary>
    public static class DirectoryHelper
    {
        /// <summary>
        /// 初始化文件的保存目录
        /// </summary>
        /// <param name="dirName">配置文件中的目录名（不包含路径）</param>
        /// <returns>返回完整的日志根目录</returns>
        public static string InitDirectory(string dirName)
        {
            // 支持绝对路径，和相对路径
            string rootDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dirName);


            // 检查日志根目录是否存在
            if( Directory.Exists(rootDirectory) == false )
                Directory.CreateDirectory(rootDirectory);


            if( rootDirectory.EndsWith("\\") == false )
                rootDirectory = rootDirectory + "\\";


            // 检查需要记录的各个数据类型的子目录是否存在。
            foreach( var item in LogConfig.GetCurrent().Types ) {
                string path = rootDirectory + item.Type.Name;
                if( Directory.Exists(path) == false )
                    Directory.CreateDirectory(path);
            }

            // 返回完整的日志根目录
            return  rootDirectory;
        }
    }
}
