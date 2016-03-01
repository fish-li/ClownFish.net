using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ClownFish.Web
{
	
	/// <summary>
	/// 表示一个页面结果（页面将由框架执行）
	/// </summary>
	public sealed class PageResult : IActionResult
	{
		/// <summary>
		/// 页面的虚拟路径
		/// </summary>
		public string VirtualPath { get; private set; }
		/// <summary>
		/// 需要绑定到页面上的数据对象
		/// </summary>
		public object Model { get; private set; }

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="virtualPath">页面的虚拟路径</param>
		public PageResult(string virtualPath) : this(virtualPath, null)
		{
		}
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="virtualPath">页面的虚拟路径</param>
		/// <param name="model">需要绑定到页面上的数据对象</param>
		public PageResult(string virtualPath, object model)
		{
			this.VirtualPath = virtualPath;
			this.Model = model;
		}

		void IActionResult.Ouput(HttpContext context)
		{
			if( string.IsNullOrEmpty(this.VirtualPath) )
				this.VirtualPath = context.Request.FilePath;

			context.Response.ContentType = "text/html";

			string html = this.VirtualPath.EndsWithIgnoreCase(".cshtml")
				? RazorHelper.Render(context, VirtualPath, Model)
				: PageExecutor.Render(context, VirtualPath, Model);

			context.Response.Write(html);
		}
	}


}
