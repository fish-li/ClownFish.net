using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Base
{
    /// <summary>
    /// 用于处理临时文件的工具类
    /// </summary>
    public static class TempFilesUtils
    {
        /// <summary>
        /// 删除过早的临时文件
        /// </summary>
        /// <param name="path">包含临时文件的目录，删除时会查找所有子目录</param>
        /// <param name="timeAgo">指定一个时间间隔，超过这个时间间隔前的文件将被删除</param>
        /// <param name="topDirectoryOnly">是否只清除指定目录，不包含它的子目录</param>
        /// <returns>过程中所有遇到的异常</returns>
        public static List<Exception> DeleteOldFiles(string path, TimeSpan timeAgo, bool topDirectoryOnly = true)
        {
            List<Exception> list = new List<Exception>();

            try {
                if( Directory.Exists(path) == false )
                    return list;

                SearchOption searchOption = topDirectoryOnly 
                                            ? SearchOption.TopDirectoryOnly 
                                            : SearchOption.AllDirectories;

                IEnumerable<string> files = Directory.EnumerateFiles(path, "*.*", searchOption);
                DateTime now = DateTime.Now;

                foreach( string file in files ) {

                    // 清除过程中，也有可能其它进程正在删除文件，所有文件不存在就忽略
                    if( File.Exists(file) == false )
                        continue;
                    
                    // 以文件的最后修改时间做为对比标准
                    DateTime time = File.GetLastWriteTime(file);
                    TimeSpan span = now - time;

                    // 删除 指定时间 前的文件
                    if( span >= timeAgo ) {
                        try {
                            //Console.WriteLine(file);
                            File.Delete(file);
                        }
                        catch( Exception ex ) {
                            list.Add(ex);
                        }
                    }
                }
            }
            catch(Exception ex2) {
                list.Add(ex2);
            }

            return list;
        }



    }
}
