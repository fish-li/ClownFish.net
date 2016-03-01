using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ClownFish.Web
{
	/// <summary>
	/// 表示Action结果的接口
	/// </summary>
	public interface IActionResult
	{
		/// <summary>
		/// 执行输出操作
		/// </summary>
		/// <param name="context"></param>
		void Ouput(HttpContext context);
	}
}
