using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Http.Pipleline;

namespace ClownFish.WebApi.Result
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
		void Ouput(NHttpContext context);
	}
}
