using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Base.UnitTest
{
	internal static class ByteTestHelper
	{
		/// <summary>
		/// 比较二个字节数组是不是相等
		/// </summary>
		/// <param name="b1"></param>
		/// <param name="b2"></param>
		/// <returns></returns>
		public static bool AreEqual(byte[] b1 , byte[] b2)
		{
			if( b1 == null && b2 == null )
				return true;

			if( b1 == null || b2 == null )
				return false;

			if( b1.Length != b2.Length )
				return false;

			for(int i= 0;i<b1.Length;i++ ) {
				if( b1[i] != b2[i] )
					return false;
			}

			return true;
		}
	}
}
