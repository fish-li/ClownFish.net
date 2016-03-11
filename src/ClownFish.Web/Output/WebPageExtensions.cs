using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.WebPages;

namespace ClownFish.Web
{
	/// <summary>
	/// WebPage的一些扩展方法
	/// </summary>
	public static class WebPageExtensions
	{
		/// <summary>
		/// 生成一个引用JS文件的HTML代码，其中URL包含了文件的最后更新时间。
		/// </summary>
		/// <param name="page">当前页面</param>
		/// <param name="jsPath">要引用的JS文件路径</param>
		/// <returns></returns>
		public static IHtmlString RenderJs(this WebPage page, string jsPath)
		{
			string html = UiHelper.RefJsFileHtml(jsPath);
			return new HtmlString(html);
		}

		/// <summary>
		/// 生成一个引用CSS文件的HTML代码，其中URL包含了文件的最后更新时间。
		/// </summary>
		/// <param name="page">当前页面</param>
		/// <param name="cssPath">要引用的CSS文件路径</param>
		/// <returns></returns>
		public static IHtmlString RenderCss(this WebPage page, string cssPath)
		{
			string html = UiHelper.RefCssFileHtml(cssPath);
			return new HtmlString(html);
		}
	}
}
