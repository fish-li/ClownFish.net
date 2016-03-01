using System;
using System.Collections.Generic;
using System.Web;

namespace DEMO.Common
{
	/// <summary>
	/// 一些扩展方法
	/// </summary>
	public static class MyExtension
	{
		public static string ToText(this int num)
		{
			if( num == 0 )
				return string.Empty;
			else
				return num.ToString();
		}

		public static string ToText(this decimal num)
		{
			if( num == 0M )
				return string.Empty;
			else
				return num.ToString("F2");
		}


	}
}