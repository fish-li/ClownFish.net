using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using ClownFish.Web.Config;

namespace ClownFish.Web.Debug404
{
	/// <summary>
	/// 404错误的描述Handler
	/// </summary>
	public sealed class Http404PageHandler : IHttpHandler
	{
		/// <summary>
		/// DiagnoseResult实例
		/// </summary>
		public DiagnoseResult DiagnoseResult { get; private set; }


		internal Http404PageHandler(DiagnoseResult diagnoseResult)
		{
			DiagnoseResult = diagnoseResult;
		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="context">HttpContext实例</param>
		public void ProcessRequest(HttpContext context)
		{
			context.Response.StatusCode = 404;

			string http404PagePath = FrameworkConfig.Instance.Pipeline.Http404PagePath;
			IActionResult result = new PageResult(http404PagePath, DiagnoseResult);
			result.Ouput(context);
		}

		/// <summary>
		/// 指示当前HttpHanlder是否可重用（固定值：false）
		/// </summary>
		public bool IsReusable
		{
			get { return false; }
		}

	}
}
