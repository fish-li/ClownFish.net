using System;
using ClownFish.Web;
using DEMO.Model;
using System.Collections.Generic;
using DEMO.BLL;

// ClownFish.Web的用法可参考：http://www.cnblogs.com/fish-li/archive/2012/02/12/2348395.html
// ClownFish.Web下载页面：http://www.cnblogs.com/fish-li/archive/2012/02/21/2361982.html


namespace DEMO.Controllers.Ajax
{

	public class CategoryService
	{
		[Action]
		public void Insert(Category category)
		{
			category.EnsureItemIsOK();

			BllFactory.GetCategoryBLL().Insert(category);
		}

		[Action]
		public object Delete(int id)
		{
			BllFactory.GetCategoryBLL().Delete(id);

			return new RedirectResult("/Pages/Categories.aspx");
		}

		[Action]
		public void Update(Category category)
		{
			category.EnsureItemIsOK();

			BllFactory.GetCategoryBLL().Update(category);
		}
		[Action]
		public object List()
		{
			List<Category> List = BllFactory.GetCategoryBLL().GetList();
			var result = new GridResult<Category>(List);
			return new JsonResult(result);
		}

		[Action]
		public object GetList()
		{
			List<Category> List = BllFactory.GetCategoryBLL().GetList();
			return new JsonResult(List);
		}

	}

}