using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ClownFish.Base
{
    /// <summary>
    /// 用于处理临时文件的工具类
    /// </summary>
    public class TempFilesUtils
    {
        /// <summary>
        /// 删除过程中产生的异常
        /// </summary>
        public List<Exception> Exceptions { get; private set; } = new List<Exception>();

        /// <summary>
        /// 成功执行删除的次数
        /// </summary>
        public int Count { get; private set; }


        /// <summary>
        /// 删除临时文件
        /// </summary>
        /// <param name="path">要执行删除的根目录</param>
        /// <param name="timeAgo">一个时间间隔，表示需要删除多久前的文件</param>
        /// <param name="topDirectoryOnly">是否只扫描指定的根目录（不包含子目录），如果需要扫描子目录，请指定为 false</param>
        public void DeleteFiles(string path, TimeSpan timeAgo, bool topDirectoryOnly)
        {
            this.Count = 0;
            this.Exceptions.Clear();

            if( string.IsNullOrEmpty(path) || Directory.Exists(path) == false )
                return;
                        

            DateTime now = DateTime.Now;
            SearchOption searchOption = topDirectoryOnly
                                            ? SearchOption.TopDirectoryOnly
                                            : SearchOption.AllDirectories;

            try {
                IEnumerable<string> files = Directory.EnumerateFiles(path, "*.*", searchOption);

                foreach( string file in files ) {

                    // 清除过程中，也有可能其它进程正在删除文件，所有文件不存在就忽略
                    if( RetryFile.Exists(file) == false )
                        continue;

                    // 以文件的最后修改时间做为对比标准
                    DateTime time = RetryFile.GetLastWriteTime(file);
                    TimeSpan span = now - time;

                    // 删除 指定时间 前的文件
                    if( span >= timeAgo ) {
                        try {
                            RetryFile.Delete(file);
                            this.Count++;
                        }
                        catch( Exception ex ) {
                            this.Exceptions.Add(ex);
                        }
                    }
                }
            }
            catch(Exception ex2 ) {
                this.Exceptions.Add(ex2);
            }
        }

        /// <summary>
        /// 目录空子目录
        /// </summary>
        /// <param name="path"></param>
        /// <param name="timeAgo"></param>
        public void DeleteDirectories(string path, TimeSpan timeAgo)
        {
            this.Count = 0;
            this.Exceptions.Clear();

            if( string.IsNullOrEmpty(path) || Directory.Exists(path) == false )
                return;
                        

            try {
                DateTime now = DateTime.Now;

                // 删除空子目录时，固定扫描所有子目录
                SearchOption searchOption = SearchOption.AllDirectories;

                IEnumerable<string> dirs = Directory.EnumerateDirectories(path, "*.*", searchOption);

                foreach( string dir in dirs ) {

                    // 清除过程中，也有可能其它进程正在删除，所有不存在就忽略
                    if( RetryDirectory.Exists(dir) == false )
                        continue;


                    // 以目录的最后修改时间做为对比标准
                    DateTime time = RetryDirectory.GetLastWriteTime(dir);
                    TimeSpan span = now - time;

                    // 删除 指定时间 前的目录
                    if( span >= timeAgo ) {

                        // 判断目录是否为空（只要不包含文件，就认为是空目录，即使包含空的子目录）
                        if( DirectoryIsEmpty(dir) == false )
                            continue;

                        try {
                            // 删除目录及其空子目录
                            RetryDirectory.Delete(dir, true);
                            this.Count++;
                        }
                        catch( Exception ex ) {
                            this.Count++;
                            this.Exceptions.Add(ex);
                        }
                    }
                }
            }
            catch( Exception ex2 ) {
                this.Exceptions.Add(ex2);
            }
        }


        /// <summary>
        /// 删除过旧的临时文件
        /// </summary>
        /// <param name="path">包含临时文件的目录，删除时会查找所有子目录</param>
        /// <param name="timeAgo">指定一个时间间隔，超过这个时间间隔前的文件将被删除</param>
        /// <param name="topDirectoryOnly">是否只清除指定目录，不包含它的子目录</param>
        /// <returns>过程中所有遇到的异常</returns>
        public static List<Exception> DeleteOldFiles(string path, TimeSpan timeAgo, bool topDirectoryOnly = true)
        {
            // 这类删除临时文件的操作，绝大多数时候并不关心异常，即使有异常下次也会再重做
            // 所以，这个方法不抛出异常。

            TempFilesUtils filesUtils = new TempFilesUtils();
            filesUtils.DeleteFiles(path, timeAgo, topDirectoryOnly);
            return filesUtils.Exceptions;
        }






        /// <summary>
        /// 删除过旧的空子目录
        /// </summary>
        /// <param name="path">包含临时文件的目录，删除时会查找所有子目录</param>
        /// <param name="timeAgo">指定一个时间间隔，超过这个时间间隔前的文件将被删除</param>
        /// <returns>过程中所有遇到的异常</returns>
        public static List<Exception> DeleteEmptyDirectories(string path, TimeSpan timeAgo)
        {
            TempFilesUtils filesUtils = new TempFilesUtils();
            filesUtils.DeleteDirectories(path, timeAgo);
            return filesUtils.Exceptions;
        }

 
        private static bool DirectoryIsEmpty(string path)
        {
            IEnumerable<string> items = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories);
            return items.Any() == false;
        }



    }
}
