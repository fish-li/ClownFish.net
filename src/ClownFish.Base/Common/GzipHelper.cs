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
	/// GZIP压缩相关的工具方法
	/// </summary>
	public static class GzipHelper
	{
		/// <summary>
		/// 用GZIP压缩一个字符串，并以BASE64字符串的形式返回压缩后的结果
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static string Compress(string input)
		{
			if( string.IsNullOrEmpty(input) )
				return input;

			byte[] bb = Encoding.UTF8.GetBytes(input);
			byte[] gzipBB = Compress(bb);
			return Convert.ToBase64String(gzipBB);
		}

		/// <summary>
		/// 用GZIP解压缩一个BASE64字符串
		/// </summary>
		/// <param name="base64"></param>
		/// <returns></returns>
		public static string Decompress(string base64)
		{
			if( string.IsNullOrEmpty(base64) )
				return base64;

			byte[] bb = Convert.FromBase64String(base64);
			byte[] gzipBB = Decompress(bb);
			return Encoding.UTF8.GetString(gzipBB);
		}

		/// <summary>
		/// 压缩一个二进制数组
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static byte[] Compress(byte[] input)
		{
			if( input == null )
				throw new ArgumentNullException("input");


			using( MemoryStream sourceStream = new MemoryStream(input) ) {
				using( MemoryStream resultStream = new MemoryStream() ) {
					using( GZipStream gZipStream = new GZipStream(resultStream, CompressionMode.Compress, true) ) {

						byte[] buffer = new byte[1024 * 4]; //缓冲区大小

						int sourceBytes = sourceStream.Read(buffer, 0, buffer.Length);
						while( sourceBytes > 0 ) {
							gZipStream.Write(buffer, 0, sourceBytes);
							sourceBytes = sourceStream.Read(buffer, 0, buffer.Length);
						}
						//gZipStream.Flush();
						gZipStream.Close();

						resultStream.Position = 0;
						return resultStream.ToArray();
					}
				}
			}
		}

		/// <summary>
		/// 解压缩一个二进制数组
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static byte[] Decompress(byte[] input)
		{
			if( input == null )
				throw new ArgumentNullException("input");


			using( MemoryStream sourceStream = new MemoryStream(input) ) {
				using( GZipStream gZipStream = new GZipStream(sourceStream, CompressionMode.Decompress, true) ) {
					using( MemoryStream resultStream = new MemoryStream() ) {
						byte[] buffer = new byte[1024 * 4]; //缓冲区大小
						int sourceBytes = gZipStream.Read(buffer, 0, buffer.Length);
						while( sourceBytes > 0 ) {
							resultStream.Write(buffer, 0, sourceBytes);
							sourceBytes = gZipStream.Read(buffer, 0, buffer.Length);
						}
						resultStream.Position = 0;
						return resultStream.ToArray();
					}
				}
			}
		}
	}
}
