using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ClownFish.Web
{
	/// <summary>
	/// 用于直接写输出到Response的工具类
	/// </summary>
	public static class ResponseWriter
	{
		/// <summary>
		/// 用指定的Page以及视图数据呈现结果（HTML），
		/// 然后将产生的HTML代码写入HttpContext.Current.Response
		/// 用户控件应从MyPageView&lt;T&gt;继承
		/// </summary>
		/// <param name="pageVirtualPath">Page的虚拟路径</param>
		/// <param name="model">视图数据</param>
		/// <param name="flush">是否需要在输出html后调用Response.Flush()</param>
		public static void WritePage(string pageVirtualPath, object model, bool flush)
		{
			HttpContext context = HttpContext.Current;
			if( context == null )
				return;

			if( string.IsNullOrEmpty(pageVirtualPath) )
				pageVirtualPath = context.Request.FilePath;

			string html = PageExecutor.Render(context, pageVirtualPath, model);
			WriteHtml(html, flush);
		}



		/// <summary>
		/// 用指定的用户控件以及视图数据呈现结果（HTML），
		/// 然后将产生的HTML代码写入HttpContext.Current.Response
		/// 用户控件应从MyUserControlView&lt;T&gt;继承
		/// </summary>
		/// <param name="ucVirtualPath">用户控件的虚拟路径</param>
		/// <param name="model">视图数据</param>
		/// <param name="flush">是否需要在输出html后调用Response.Flush()</param>
		public static void WriteUserControl(string ucVirtualPath, object model, bool flush)
		{
			HttpContext context = HttpContext.Current;
			if( context == null )
				return;

			string html = UcExecutor.Render(ucVirtualPath, model);
			WriteHtml(html, flush);
		}



		/// <summary>
		/// 用指定的用户控件以及视图数据呈现结果（HTML），
		/// 并生成一段代码，用于将生成的HTML替换哪个DOM节点，
		/// 然后将所有产生的HTML代码写入HttpContext.Current.Response
		/// 用户控件应从MyUserControlView&lt;T&gt;继承
		/// </summary>
		/// <param name="ucVirtualPath">用户控件的虚拟路径</param>
		/// <param name="model">视图数据</param>
		/// <param name="targetDomId">需要将输出内容替换哪个DOM节点的内容</param>
		/// <param name="flush">是否需要在输出html后调用Response.Flush()</param>
		public static void WriteUserControl(string ucVirtualPath, object model, string targetDomId, bool flush)
		{
			HttpContext context = HttpContext.Current;
			if( context == null )
				return;

			SetResponseContentType(context.Response);

			string html = UcExecutor.Render(ucVirtualPath, model);

			string tempId = "div-" + Guid.NewGuid().ToString();

			context.Response.Write(string.Format("<div id=\"{0}\" style=\"display: none\">", tempId));

			context.Response.Write(html);

			context.Response.Write(string.Format("</div><script type=\"text/javascript\">document.getElementById(\"{0}\").innerHTML = document.getElementById(\"{1}\").innerHTML;</script>",
				targetDomId, tempId));

			if( flush )
				context.Response.Flush();
		}

		private static void SetResponseContentType(HttpResponse response)
		{
			if( string.IsNullOrEmpty(response.ContentType) )
				response.ContentType = "text/html";
		}

		/// <summary>
		/// 将指定的HTML代码写入HttpContext.Current.Response
		/// </summary>
		/// <param name="html">要写入的HTML文本</param>
		/// <param name="flush">是否需要在输出html后调用Response.Flush()</param>
		public static void WriteHtml(string html, bool flush)
		{
			HttpContext context = HttpContext.Current;
			if( context == null )
				return;

			if( string.IsNullOrEmpty(html) )
				return;


			SetResponseContentType(context.Response);

			context.Response.Write(html);

			if( flush )
				context.Response.Flush();
		}

	}
}
