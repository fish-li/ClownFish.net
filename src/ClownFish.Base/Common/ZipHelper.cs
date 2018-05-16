using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ClownFish.Base
{
    /// <summary>
    /// 提供一些操作ZIP文件的工具方法
    /// </summary>
    public static class ZipHelper
    {
        /// <summary>
        /// 将ZIP文件解压缩到指定的目录
        /// </summary>
        /// <param name="zipPath">需要解压缩的ZIP文件</param>
        /// <param name="extractPath">要释放的目录</param>
        public static void ExtractFiles(string zipPath, string extractPath)
        {
            //ZipFile.ExtractToDirectory(zipPath, extractPath);
            // .net framework 的实现版本中，如果要释放的文件已存在，会抛出异常，所以这里就不使用了。


            if( string.IsNullOrEmpty(zipPath) )
                throw new ArgumentNullException("zipPath");
            if( string.IsNullOrEmpty(extractPath) )
                throw new ArgumentNullException("extractPath");

            using( ZipArchive archive = ZipFile.OpenRead(zipPath) ) {
                foreach( ZipArchiveEntry entry in archive.Entries ) {

                    // 忽略目录
                    if( entry.FullName.EndsWith("/") ) {
                        string path = Path.Combine(extractPath, entry.FullName.TrimEnd('/'));
                        if( Directory.Exists(path) == false )
                            Directory.CreateDirectory(path);

                        continue;
                    }


                    // 计算要释放到哪里
                    string targetFile = Path.Combine(extractPath, entry.FullName);

                    // 如果要释放的目标目录不存在，就创建目标
                    string destPath = Path.GetDirectoryName(targetFile);
                    Directory.CreateDirectory(destPath);

                    // 如果目标文件已经存在，就删除
                    RetryFile.Delete(targetFile);

                    // 释放文件
                    entry.ExtractToFile(targetFile, true);
                }
            }
        }


        /// <summary>
        /// 读取ZIP到内存字典中
        /// </summary>
        /// <param name="zipPath">需要读取的ZIP文件</param>
        /// <returns></returns>
        public static List<Tuple<string, byte[]>> Read(string zipPath)
        {
            if( string.IsNullOrEmpty(zipPath) )
                throw new ArgumentNullException("zipPath");

            List<Tuple<string, byte[]>> result = new List<Tuple<string, byte[]>>();


            using( ZipArchive archive = ZipFile.OpenRead(zipPath) ) {
                foreach( ZipArchiveEntry entry in archive.Entries ) {

                    // 目录
                    if( entry.FullName.EndsWith("/") )
                        continue;


                    // 读取某个文件内容
                    // 由于读取过程涉及解压缩，所以不能一次到 byte[]，只好引入一个内存流
                    using( MemoryStream ms = new MemoryStream() ) {
                        using( Stream steam = entry.Open() ) {
                            steam.CopyTo(ms);
                        }
                        ms.Position = 0;

                        Tuple<string, byte[]> tuple = new Tuple<string, byte[]>(entry.FullName, ms.ToArray());
                        result.Add(tuple);
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// 将指定的目录打包成ZIP文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="zipPath"></param>
        public static void Compress(string path, string zipPath)
        {
            RetryFile.Delete(zipPath);
            
            // 直接调用 .NET framework
            ZipFile.CreateFromDirectory(path, zipPath);
        }



        /// <summary>
        /// 根据指定的包内文件名及对应的文件内容打包成ZIP文件
        /// </summary>
        /// <param name="files"></param>
        /// <param name="zipPath"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202")]
        public static void Compress(List<Tuple<string, byte[]>> files, string zipPath)
        {
            if( files == null )
                throw new ArgumentNullException(nameof(files));
            if( string.IsNullOrEmpty(zipPath) )
                throw new ArgumentNullException(nameof(zipPath));


            RetryFile.Delete(zipPath);


            using( FileStream file = new FileStream(zipPath, FileMode.Create) ) {
                using( ZipArchive zip = new ZipArchive(file, ZipArchiveMode.Create, true, Encoding.UTF8) ) {

                    foreach( Tuple<string, byte[]> tuple in files ) {

                        var entry = zip.CreateEntry(tuple.Item1, CompressionLevel.Optimal);

                        using( BinaryWriter writer = new BinaryWriter(entry.Open()) ) {
                            writer.Write(tuple.Item2);
                        }
                    }
                }
            }
        }




    }
}
