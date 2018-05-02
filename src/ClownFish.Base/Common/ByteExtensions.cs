using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Base
{
	/// <summary>
	/// 二进制数据操作的工具类
	/// </summary>
	public static class ByteExtensions
	{
		/// <summary>
		/// 比较二个字节数组是不是相等
		/// </summary>
		/// <param name="b1"></param>
		/// <param name="b2"></param>
		/// <returns></returns>
		public static bool IsEqual(this byte[] b1, byte[] b2)
		{
			if( b1 == null && b2 == null )
				return true;

			if( b1 == null || b2 == null )
				return false;

			if( b1.Length != b2.Length )
				return false;

			for( int i = 0; i < b1.Length; i++ ) {
				if( b1[i] != b2[i] )
					return false;
			}

			return true;
		}



        /// <summary>
		/// 将byte[]做BASE64编码，Convert.ToBase64String(bytes);
		/// </summary>
		/// <param name="bytes"></param>
		/// <returns></returns>
		public static string ToBase64(this byte[] bytes)
        {
            if( bytes == null || bytes.Length == 0 )
                return string.Empty;

            return Convert.ToBase64String(bytes);
        }


        /// <summary>
		/// 将byte[]按十六进制转换成字符串，BitConverter.ToString(bytes).Replace("-", "");
		/// </summary>
		/// <param name="bytes"></param>
		/// <returns></returns>
		public static string ToHexString(this byte[] bytes)
        {
            if( bytes == null || bytes.Length == 0 )
                return string.Empty;

            return BitConverter.ToString(bytes).Replace("-", "");
        }


    }
}
