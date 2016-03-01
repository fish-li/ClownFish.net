using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using ClownFish.Web;


// ClownFish.Web的用法可参考：http://www.cnblogs.com/fish-li/archive/2012/02/12/2348395.html
// ClownFish.Web下载页面：http://www.cnblogs.com/fish-li/archive/2012/02/21/2361982.html


namespace DEMO.Controllers.Page
{
	public class CommonPageController : BaseDemoController
	{		
		[Action]
		[PageUrl(Url = "/Pages/AddOrder.aspx")]
		//[PageUrl(Url = "/Pages/CodeExplorer.aspx")]
		[PageUrl(Url = "/Pages/Default.aspx")]
		[PageUrl(Url = "/Pages/Orders.aspx")]
		public object TransferRequest()
		{
			// 这个Action要做的事较为简单，
			// 将请求 "/Pages/Orders.aspx" 用实际的页面 "/Pages/StyleX/Orders.aspx" 来响应。
			// 因为用户选择的风格不同，但URL地址是一样的，所以在这里切换。

			// 当然这样的处理也只适合页面不需要Model的情况下。

			string filePath = this.HttpContext.Request.FilePath;
			int p = filePath.LastIndexOf('/');
			string pageName = filePath.Substring(p + 1);

			return new PageResult(this.GetTargetPageUrl(pageName), null /*model*/);
		}


		[Action]
		[PageUrl(Url = "/Pages/CodeExplorer.aspx")]
		public object CodeExplorer()
		{
			// 永远只使用第一个版本的，避免维护多个版本。
			return new PageResult("/Pages/Style1/CodeExplorer.cshtml", null /*model*/);
		}

	}
}
