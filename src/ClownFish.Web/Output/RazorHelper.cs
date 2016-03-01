using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.WebPages;
using ClownFish.Base.TypeExtend;

namespace ClownFish.Web
{
	/// <summary>
	/// 针对Razor视图引擎操作封装的工具类
	/// </summary>
	public class RazorHelper
	{
		/// <summary>
		/// 渲染一个Razor视图模板
		/// </summary>
		/// <param name="context">HttpContext实例引用</param>
		/// <param name="pageVirtualPath">Razor视图的路径</param>
		/// <param name="model">要渲染到视图上的数据对象</param>
		/// <returns>渲染后的HTML代码</returns>
		public static string Render(HttpContext context, string pageVirtualPath, object model)
		{
			HttpContextBase httpContextBase = new HttpContextWrapper(context);

			return Render(httpContextBase, pageVirtualPath, model);
		}

		/// <summary>
		/// 渲染一个Razor视图模板
		/// </summary>
		/// <param name="context">HttpContextBase实例引用</param>
		/// <param name="pageVirtualPath">Razor视图的路径</param>
		/// <param name="model">要渲染到视图上的数据对象</param>
		/// <returns>渲染后的HTML代码</returns>
		public static string Render(HttpContextBase context, string pageVirtualPath, object model)
		{
			RazorHelper razor = ObjectFactory.New<RazorHelper>();
			return razor.RenderPage(context, pageVirtualPath, model);
		}


		/// <summary>
		/// 渲染一个Razor视图模板
		/// </summary>
		/// <param name="context">HttpContextBase实例引用</param>
		/// <param name="pageVirtualPath">Razor视图的路径</param>
		/// <param name="model">要渲染到视图上的数据对象</param>
		/// <returns>渲染后的HTML代码</returns>
		protected virtual string RenderPage(HttpContextBase context, string pageVirtualPath, object model)
		{
			// 扩展点：如果需要实现页面替换逻辑，例如个性化页面覆盖标准产品页面，可以重写这个方法

			WebPageBase page = WebPage.CreateInstanceFromVirtualPath(pageVirtualPath);

			StringWriter output = new StringWriter();
			WebPageContext pageContext = new WebPageContext(context, null, model);

			page.ExecutePageHierarchy(pageContext, output);

			return output.ToString();
		}

	}
}
