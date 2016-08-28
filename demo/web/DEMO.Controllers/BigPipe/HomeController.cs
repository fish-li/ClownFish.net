using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using ClownFish.Web;
using DEMO.BLL.BigPipe;

// ClownFish.Web的用法可参考：http://www.cnblogs.com/fish-li/archive/2012/02/12/2348395.html
// ClownFish.Web下载页面：http://www.cnblogs.com/fish-li/archive/2012/02/21/2361982.html

// BigPipe相关参考资料：
// http://www.searchtb.com/2011/04/an-introduction-to-bigpipe.html
// http://baike.baidu.com/view/4601904.htm

namespace DEMO.Controllers.BigPipe
{
	public class HomeController : BaseController
	{
		[Action]
		[PageUrl(Url = "/BigPipe/BigPipeDemo.aspx")]
		public void ShowHomePage()
		{
			// 先输出页框架
			ResponseWriter.WritePage(null /* pageVirtualPath */, null /* model */, true /* flush */);

			string appRootPath = this.WebRuntime.GetWebSitePath();

			BlogBLL bll = new BlogBLL();

			// 加载博客内容，第一个数据
			string blogFilePath = Path.Combine(appRootPath, "App_Data\\BigPipe\\BlogBody.txt");
			ResponseWriter.WriteUserControl("~/BigPipe/UserControls/BlogBody.ascx",
									bll.GetBlog(blogFilePath), "blog-body", true);

			// 加载左链接导航栏，第二个数据
			string linksFilePath = Path.Combine(appRootPath, "App_Data\\BigPipe\\Links.txt");
			ResponseWriter.WriteUserControl("~/BigPipe/UserControls/TagLinks.ascx",
									bll.GetLinks(linksFilePath), "right", true);

			// 加载评论，第三个数据
			string commentFilePath = Path.Combine(appRootPath, "App_Data\\BigPipe\\Comments.txt");
			ResponseWriter.WriteUserControl("~/BigPipe/UserControls/CommentList.ascx",
									bll.GetComments(commentFilePath), "blog-comments-placeholder", true);


			ResponseWriter.WriteUserControl("~/BigPipe/UserControls/PageEnd.ascx", null /* model */, true /* flush */);
		}
	}
}

