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
	public class CodeExplorerController : BaseController
	{		


		[Action]
		[PageUrl(Url = "/Pages/CodeExplorer.aspx")]
		public object Default()
		{
			// 永远只使用第一个版本的，避免维护多个版本。
			return new PageResult("/CodeExplorer/Default.cshtml", null /*model*/);
		}

	}
}
