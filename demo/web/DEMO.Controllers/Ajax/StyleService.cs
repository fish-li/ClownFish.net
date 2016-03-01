using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using ClownFish.Web;
using DEMO.Controllers.Page;


// ClownFish.Web的用法可参考：http://www.cnblogs.com/fish-li/archive/2012/02/12/2348395.html
// ClownFish.Web下载页面：http://www.cnblogs.com/fish-li/archive/2012/02/21/2361982.html

namespace DEMO.Controllers.Ajax
{
	public class StyleService : BaseController
	{
		[Action]
		public void SetStyle(string style)
		{
			if( Array.IndexOf(BaseDemoController.StyleArray, style) >= 0 ) {
				HttpCookie cookie = new HttpCookie(BaseDemoController.STR_PageStyle, style);
				cookie.Expires = DateTime.Now.AddYears(1);
				this.WriteCookie(cookie);
			}
		}
	}
}
