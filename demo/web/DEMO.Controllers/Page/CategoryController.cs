using System;
using ClownFish.Web;
using DEMO.Model;
using DEMO.BLL;
using System.Web;



// ClownFish.Web的用法可参考：http://www.cnblogs.com/fish-li/archive/2012/02/12/2348395.html
// ClownFish.Web下载页面：http://www.cnblogs.com/fish-li/archive/2012/02/21/2361982.html


namespace DEMO.Controllers.Page
{

	public class CategoryController : BaseDemoController
	{
		[Action]
		[PageUrl(Url = "/Pages/Categories.aspx")]
		public object LoadModel()
		{
			// 根据用户选择的界面风格，计算实现要呈现的页面路径。
			string papeUrl = this.GetTargetPageUrl("Categories.aspx");


			if( this.IsStyle2 )
				// Style2 风格下，页面不需要绑定数据。数据由JS通过AJAX方式获取
				return new PageResult(papeUrl, null);

			// 为Style1 风格获取数据。
			CategoriesPageModel result = new CategoriesPageModel();
			result.List = BllFactory.GetCategoryBLL().GetList();

			return new PageResult(papeUrl, result);
		}

	}
}
