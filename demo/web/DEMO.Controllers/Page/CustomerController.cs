using System;
using ClownFish.Web;
using DEMO.Model;
using DEMO.Common;
using DEMO.BLL;
using System.Web;



// ClownFish.Web的用法可参考：http://www.cnblogs.com/fish-li/archive/2012/02/12/2348395.html
// ClownFish.Web下载页面：http://www.cnblogs.com/fish-li/archive/2012/02/21/2361982.html



namespace DEMO.Controllers.Page
{

	public class CustomerController : BaseDemoController
	{
		[Action]
		[PageUrl(Url = "/mvc/Customers")]
		[PageUrl(Url = "/mvc/Customers.html")]
		[PageUrl(Url = "/mvc/test/Customers.html")]
		[PageUrl(Url = "/mvc/test/aa/bb/cc/Customers.html")]
		[PageUrl(Url = "/mvc/CustomerList.aspx")]
		[PageUrl(Url = "/Pages/Customers.aspx")]
		public object LoadModel(int? page)
		{
			// 说明：参数page表示分页数，方法名LoadModel其实可以【随便取】。

			// 根据用户选择的界面风格，计算实现要呈现的页面路径。
			string papeUrl = this.GetTargetPageUrl("Customers.aspx");

			if( this.IsStyle2 )
				// Style2 风格下，页面不需要绑定数据。数据由JS通过AJAX方式获取
				return new PageResult(papeUrl, null);


			// 为Style1 风格获取数据。
			CustomerSearchInfo info = new CustomerSearchInfo();
			info.SearchWord = string.Empty;
			info.PageIndex = page.HasValue ? page.Value - 1 : 0;
			info.PageSize = AppHelper.DefaultPageSize;


			CustomersPageModel result = new CustomersPageModel();
			result.PagingInfo = info;
			result.List = BllFactory.GetCustomerBLL().GetList(info);
			result.RequestUrlEncodeRawUrl = HttpUtility.UrlEncode(this.HttpContext.Request.RawUrl);

			return new PageResult(papeUrl, result);
		}
	}
}
