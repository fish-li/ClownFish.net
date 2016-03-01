using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ClownFish.Web
{
	/// <summary>
	/// 实现一个HTTP 304的执行结果。
	/// </summary>
	public sealed class Http304Result : IActionResult
	{
		/// <summary>
		/// 实现IActionResult接口，执行输出操作
		/// </summary>
		/// <param name="context">HttpContext实例</param>
		public void Ouput(HttpContext context)
		{
			context.Response.StatusCode = 304;
		}
		
	}
}
