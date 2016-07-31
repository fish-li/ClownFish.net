using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Data
{
	/// <summary>
	/// 定义一些扩展方法
	/// </summary>
	internal static class SomeExtensions
	{
		/// <summary>
		/// 创建一个与DbContext有关的CPQuery实例
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public static CPQuery CreateCPQuery(this DbContext context)
		{
			if( context == null )
				return CPQuery.Create();
			else
				return context.CPQuery.Create(null);
		}
		
	}
}
