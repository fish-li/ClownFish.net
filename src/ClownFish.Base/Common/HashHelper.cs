using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Base
{
    /// <summary>
    /// 封装常用的HASH算法
    /// </summary>
    public static class HashHelper
    {
        /// <summary>
        /// 计算字符串的 SHA1 签名
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Sha1(this string text)
        {
            return Sha1(text, Encoding.UTF8);
        }

        /// <summary>
        /// 计算字符串的 SHA1 签名
        /// </summary>
        /// <param name="text"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Sha1(this string text, Encoding encoding)
        {
            if( text == null )
                throw new ArgumentNullException(nameof(text));

            using( SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider() ) {
                byte[] bb = encoding.GetBytes(text);
                byte[] buffer = sha1.ComputeHash(bb);
                return BitConverter.ToString(buffer).Replace("-", "");
            }
        }


        /// <summary>
        /// 计算文件的SHA1值
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string FileSha1(string filePath)
        {
            if( string.IsNullOrEmpty(filePath) )
                throw new ArgumentNullException(nameof(filePath));
            if( File.Exists(filePath) == false )
                throw new FileNotFoundException("文件不存在：" + filePath);

            using( SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider() ) {
                using( FileStream fs = File.OpenRead(filePath) ) {
                    byte[] buffer = sha1.ComputeHash(fs);
                    return BitConverter.ToString(buffer).Replace("-", "");
                }
            }
        }

        /// <summary>
        /// 计算字符串的 MD5 签名
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Md5(this string text)
        {
            return Md5(text, Encoding.UTF8);
        }

        /// <summary>
        /// 计算字符串的 MD5 签名
        /// </summary>
        /// <param name="text"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Md5(this string text, Encoding encoding)
        {
            if( text == null )
                throw new ArgumentNullException(nameof(text));

            using( MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider() ) {
                byte[] bb = encoding.GetBytes(text);
                byte[] buffer = md5.ComputeHash(bb);
                return BitConverter.ToString(buffer).Replace("-", "");
            }
        }


        /// <summary>
        /// 计算文件的MD5值
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string FileMD5(string filePath)
        {
            if( string.IsNullOrEmpty(filePath) )
                throw new ArgumentNullException(nameof(filePath));
            if( File.Exists(filePath) == false )
                throw new FileNotFoundException("文件不存在：" + filePath);

            using( MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider() ) {
                using( FileStream fs = File.OpenRead(filePath) ) {
                    byte[] buffer = md5.ComputeHash(fs);
                    return BitConverter.ToString(buffer).Replace("-", "");
                }
            }
        }


        /// <summary>
        /// 快速计算文件哈希值（只计算文件开头部分）
        /// </summary>
        /// <param name="filePath">要计算哈希值的文件路径</param>
        /// <param name="headerLength">读取开头多长的部分，默认值：2M</param>
        /// <returns></returns>
        public static string FastHash(string filePath, int headerLength = 2 * 1024 * 1024)
        {
            if( string.IsNullOrEmpty(filePath) )
                throw new ArgumentNullException(nameof(filePath));
            if( File.Exists(filePath) == false )
                throw new FileNotFoundException("文件不存在：" + filePath);

            if( headerLength <= 0 )
                throw new ArgumentOutOfRangeException(nameof(headerLength));


            using( MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider() ) {
                using( FileStream file = File.OpenRead(filePath) ) {
                    if( headerLength > file.Length )
                        headerLength = (int)file.Length;

                    // 将文件长度转成字节数组
                    byte[] intBytes = BitConverter.GetBytes(file.Length);
                    if( BitConverter.IsLittleEndian )
                        Array.Reverse(intBytes);

                    // 申请字节缓冲区
                    byte[] buffer = new byte[headerLength + intBytes.Length];
                    Array.Copy(intBytes, buffer, intBytes.Length);

                    // 读取文件的开头部分
                    file.Read(buffer, intBytes.Length, headerLength);

                    // 计算缓冲区的哈希值
                    byte[] result = md5.ComputeHash(buffer);
                    return BitConverter.ToString(result).Replace("-", "");
                }
            }
        }


    }
}
