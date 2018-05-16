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
        private static string HashString(HashAlgorithm hash, string text, Encoding encoding)
        {
            if( text == null )
                throw new ArgumentNullException(nameof(text));


            byte[] bb = encoding.GetBytes(text);
            byte[] buffer = hash.ComputeHash(bb);
            return BitConverter.ToString(buffer).Replace("-", "");
        }

        private static string HashFile(HashAlgorithm hash, string filePath)
        {
            if( string.IsNullOrEmpty(filePath) )
                throw new ArgumentNullException(nameof(filePath));
            if( RetryFile.Exists(filePath) == false )
                throw new FileNotFoundException("文件不存在：" + filePath);


            using( FileStream fs = RetryFile.OpenRead(filePath) ) {
                byte[] buffer = hash.ComputeHash(fs);
                return BitConverter.ToString(buffer).Replace("-", "");
            }
        }


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
            using( SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider() ) {
                return HashString(sha1, text, encoding);
            }
        }


        /// <summary>
        /// 计算字符串的 SHA256 签名
        /// </summary>
        /// <param name="text"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Sha256(this string text, Encoding encoding)
        {
            using( SHA256CryptoServiceProvider sha1 = new SHA256CryptoServiceProvider() ) {
                return HashString(sha1, text, encoding);
            }
        }


        /// <summary>
        /// 计算字符串的 SHA512 签名
        /// </summary>
        /// <param name="text"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Sha512(this string text, Encoding encoding)
        {
            using( SHA512CryptoServiceProvider sha1 = new SHA512CryptoServiceProvider() ) {
                return HashString(sha1, text, encoding);
            }
        }



        /// <summary>
        /// 计算文件的SHA1值
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string FileSha1(string filePath)
        {
            using( SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider() ) {
                return HashFile(sha1, filePath);
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
            using( MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider() ) {
                return HashString(md5, text, encoding);
            }
        }


        /// <summary>
        /// 计算文件的MD5值
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string FileMD5(string filePath)
        {
            using( MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider() ) {
                return HashFile(md5, filePath);
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
            if( RetryFile.Exists(filePath) == false )
                throw new FileNotFoundException("文件不存在：" + filePath);

            if( headerLength <= 0 )
                throw new ArgumentOutOfRangeException(nameof(headerLength));


            using( MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider() ) {
                using( FileStream file = RetryFile.OpenRead(filePath) ) {
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
