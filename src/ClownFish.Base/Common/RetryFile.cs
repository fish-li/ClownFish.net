using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Base
{
    /// <summary>
    /// 提供一些与 System.IO.File 相同签名且功能相同的工具方法，
    /// 差别在于：当出现IOException时，这个类中的方法支持重试功能。
    /// </summary>
    public static class RetryFile
    {
        /// <summary>
        /// 等同于：System.IO.File.Exists()
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool Exists(string filePath)
        {
            // 由于 File.Exists 内部已经吃掉了很多异常，包含IOException，所以这里就不再对它重试。
            // 只是为了在代码中消灭 File.xxxxxxx 就提供了这个方法。
            return File.Exists(filePath);
        }

        private static Retry CreateRetry()
        {
            // 重试策略：当发生 IOException 时，最大重试 10 次，每次间隔 500 毫秒
            return Retry.Create(10, 500).Filter<IOException>();
        }

        /// <summary>
        /// 等同于：System.IO.File.Delete()，
        /// 但是会在删除文件前检查文件是否存在。
        /// </summary>
        /// <param name="filePath"></param>
        public static void Delete(string filePath)
        {
            if( File.Exists(filePath) == false )
                return;

            CreateRetry().Run(() => {
                File.Delete(filePath);
                return 1;
            });
        }

        /// <summary>
        /// 等同于：System.IO.File.ReadAllText()
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string ReadAllText(string filePath, Encoding encoding)
        {
            return CreateRetry().Run(() => {
                return File.ReadAllText(filePath, encoding);
            });
        }


        /// <summary>
        /// 等同于：System.IO.File.ReadAllBytes()
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static byte[] ReadAllBytes(string filePath)
        {
            return CreateRetry().Run(() => {
                return File.ReadAllBytes(filePath);
            });
        }



        /// <summary>
        /// 等同于：System.IO.File.WriteAllText()
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="text"></param>
        /// <param name="encoding"></param>
        public static void WriteAllText(string filePath, string text, Encoding encoding)
        {
            CreateRetry().Run(() => {
                File.WriteAllText(filePath, text, encoding);
                return 1;
            });
        }


        /// <summary>
        /// 等同于：System.IO.File.WriteAllBytes()
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="buffer"></param>
        public static void WriteAllBytes(string filePath, byte[] buffer)
        {
            CreateRetry().Run(() => {
                File.WriteAllBytes(filePath, buffer);
                return 1;
            });
        }


        /// <summary>
        /// 等同于：System.IO.File.AppendAllText()
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="text"></param>
        /// <param name="encoding"></param>
        public static void AppendAllText(string filePath, string text, Encoding encoding)
        {
            CreateRetry().Run(() => {
                File.AppendAllText(filePath, text, encoding);
                return 1;
            });
        }


        /// <summary>
        /// 等同于：System.IO.File.GetLastWriteTime()
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static DateTime GetLastWriteTime(string filePath)
        {
            return CreateRetry().Run(() => {
                return File.GetLastWriteTime(filePath);
            });
        }


        /// <summary>
        /// 等同于：System.IO.File.GetLastWriteTimeUtc()
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static DateTime GetLastWriteTimeUtc(string filePath)
        {
            return CreateRetry().Run(() => {
                return File.GetLastWriteTimeUtc(filePath);
            });
        }


        /// <summary>
        /// 等同于：System.IO.File.OpenRead()
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static FileStream OpenRead(string filePath)
        {
            return CreateRetry().Run(() => {
                return File.OpenRead(filePath);
            });
        }



       /// <summary>
        /// 等同于：System.IO.File.OpenWrite()
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static FileStream OpenWrite(string filePath)
        {
            return CreateRetry().Run(() => {
                return File.OpenWrite(filePath);
            });
        }


        /// <summary>
        /// 等同于：new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static FileStream OpenCreate(string filePath)
        {
            return CreateRetry().Run(() => {
                return new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            });
        }


    }
}
