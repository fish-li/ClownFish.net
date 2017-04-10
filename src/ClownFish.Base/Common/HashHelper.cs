using System;
using System.Collections.Generic;
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

			SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
			byte[] bb = encoding.GetBytes(text);
			byte[] buffer = sha1.ComputeHash(bb);
			return BitConverter.ToString(buffer).Replace("-", "");
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

			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
			byte[] bb = encoding.GetBytes(text);
			byte[] buffer = md5.ComputeHash(bb);
			return BitConverter.ToString(buffer).Replace("-", "");
		}

	}
}
