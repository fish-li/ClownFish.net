using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Base
{
	/// <summary>
	/// “对称加密算法”的封装工具类
	/// </summary>
	internal static class CryptoHelper
	{
		/// <summary>
		/// 加密字符串
		/// </summary>
		/// <param name="text">等待加密的文本</param>
		/// <param name="sa">“对称加密算法”实例，要求已设置过KEY和IV</param>
		/// <returns>返回Base64编码的结果</returns>
		public static string Encrypt(string text, SymmetricAlgorithm sa)
		{
			if( text == null )
				throw new ArgumentNullException("text");
			if( sa == null )
				throw new ArgumentNullException("sa");

			byte[] input = Encoding.UTF8.GetBytes(text);
			byte[] output = Encrypt(input, sa);
			return Convert.ToBase64String(output);
		}

		/// <summary>
		/// 加密字节数组
		/// </summary>
		/// <param name="input"></param>
		/// <param name="sa">“对称加密算法”实例，要求已设置过KEY和IV</param>
		/// <returns></returns>
		public static byte[] Encrypt(byte[] input, SymmetricAlgorithm sa)
		{
			if( input == null )
				throw new ArgumentNullException("input");
			if( sa == null )
				throw new ArgumentNullException("sa");


			ICryptoTransform transform = sa.CreateEncryptor();
			return transform.TransformFinalBlock(input, 0, input.Length);
		}

		/// <summary>
		/// 解密一个以Base64编码的加密字符串
		/// </summary>
		/// <param name="base64">等待解密的BASE64文本</param>
		/// <param name="sa">“对称加密算法”实例，要求已设置过KEY和IV</param>
		/// <returns></returns>
		public static string Decrypt(string base64, SymmetricAlgorithm sa)
		{
			if( base64 == null )
				throw new ArgumentNullException("base64");
			if( sa == null )
				throw new ArgumentNullException("sa");

			byte[] input = Convert.FromBase64String(base64);
			byte[] output = Decrypt(input, sa);
			string result = Encoding.UTF8.GetString(output);
			return result.TrimEnd('\0');
		}

		/// <summary>
		/// 解密字节数组
		/// </summary>
		/// <param name="input"></param>
		/// <param name="sa">“对称加密算法”实例，要求已设置过KEY和IV</param>
		/// <returns></returns>
		public static byte[] Decrypt(byte[] input, SymmetricAlgorithm sa)
		{
			if( input == null )
				throw new ArgumentNullException("input");
			if( sa == null )
				throw new ArgumentNullException("sa");

			ICryptoTransform transform = sa.CreateDecryptor();
			return transform.TransformFinalBlock(input, 0, input.Length);
		}

		/// <summary>
		/// 根据指定的密码，设置“加密算法”的KEY和IV
		/// </summary>
		/// <param name="sa">“对称加密算法”实例</param>
		/// <param name="password">加密或解密的密码</param>
		public static void SetKeyIV(SymmetricAlgorithm sa, string password)
		{
			if( sa == null )
				throw new ArgumentNullException("sa");
			if( password == null )
				throw new ArgumentNullException("password");

			byte[] pwd = Encoding.UTF8.GetBytes(string.Concat(password, "大明王朝"));

            using( MD5CryptoServiceProvider  md5Provider = new MD5CryptoServiceProvider() ) {
                using( SHA1CryptoServiceProvider sha1Provider = new SHA1CryptoServiceProvider() ) {

                    byte[] md5 = md5Provider.ComputeHash(pwd);
                    byte[] sha1 = sha1Provider.ComputeHash(pwd);

                    sa.IV = GetByteArray(md5, sa.IV.Length);
                    sa.Key = GetByteArray(sha1, sa.Key.Length);
                }
            }			
		}

		private static byte[] GetByteArray(byte[] src, int destLen)
		{
			byte[] dest = new byte[destLen];
			int p = 0;

			while( p < destLen ) {
				foreach( byte b in src ) {
					if( p >= destLen )
						return dest;
					else
						dest[p++] = b;
				}
			}

			return dest;
		}
	}


	/// <summary>
	/// 对TripleDES封装的工具类
	/// </summary>
	public static class TripleDESHelper
	{
		private static TripleDESCryptoServiceProvider GetProvider(string password)
		{
			TripleDESCryptoServiceProvider sa = new TripleDESCryptoServiceProvider();
			CryptoHelper.SetKeyIV(sa, password);
			return sa;
		}

		/// <summary>
		/// 使用TripleDES加密字符串
		/// </summary>
		/// <param name="text"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public static string Encrypt(string text, string password)
		{
            using( SymmetricAlgorithm sa = GetProvider(password) ) {
                return CryptoHelper.Encrypt(text, sa);
            }
		}
		/// <summary>
		/// 使用TripleDES加密字节数组
		/// </summary>
		/// <param name="input"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public static byte[] Encrypt(byte[] input, string password)
		{
            using( SymmetricAlgorithm sa = GetProvider(password) ) {
                return CryptoHelper.Encrypt(input, sa);
            }
		}
		/// <summary>
		/// 使用TripleDES解密一个以Base64编码的加密字符串
		/// </summary>
		/// <param name="base64"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public static string Decrypt(string base64, string password)
		{
            using( SymmetricAlgorithm sa = GetProvider(password) ) {
                return CryptoHelper.Decrypt(base64, sa);
            }
		}
		/// <summary>
		/// 使用TripleDES解密字节数组
		/// </summary>
		/// <param name="input"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public static byte[] Decrypt(byte[] input, string password)
		{
            using( SymmetricAlgorithm sa = GetProvider(password) ) {
                return CryptoHelper.Decrypt(input, sa);
            }
		}

	}



	/// <summary>
	/// 对AES算法封装的工具类
	/// </summary>
	public static class AesHelper
	{
		private static AesCryptoServiceProvider GetProvider(string password)
		{
			AesCryptoServiceProvider sa = new AesCryptoServiceProvider();
			CryptoHelper.SetKeyIV(sa, password);
			return sa;
		}

		/// <summary>
		/// 使用AES算法加密字符串
		/// </summary>
		/// <param name="text"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public static string Encrypt(string text, string password)
		{
            using( SymmetricAlgorithm sa = GetProvider(password) ) {
                return CryptoHelper.Encrypt(text, sa);
            }
		}
		/// <summary>
		/// 使用AES算法加密字节数组
		/// </summary>
		/// <param name="input"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public static byte[] Encrypt(byte[] input, string password)
		{
            using( SymmetricAlgorithm sa = GetProvider(password) ) {
                return CryptoHelper.Encrypt(input, sa);
            }
		}

		/// <summary>
		/// 使用AES算法解密一个以Base64编码的加密字符串
		/// </summary>
		/// <param name="base64"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public static string Decrypt(string base64, string password)
		{
            using( SymmetricAlgorithm sa = GetProvider(password) ) {
                return CryptoHelper.Decrypt(base64, sa);
            }
		}

		/// <summary>
		/// 使用AES算法解密字节数组
		/// </summary>
		/// <param name="input"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public static byte[] Decrypt(byte[] input, string password)
		{
            using( SymmetricAlgorithm sa = GetProvider(password) ) {
                return CryptoHelper.Decrypt(input, sa);
            }
		}
	}
}
