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

	public class ProductController : BaseDemoController
	{
		[Action]
		[PageUrl(Url = "/mvc/Products")]
		[PageUrl(Url = "/mvc/Products.html")]
		[PageUrl(Url = "/mvc/ProductList.aspx")]
		[PageUrl(Url = "/Pages/Products.aspx")]
		public object LoadModel(int? categoryId, int? page)
		{
			string papeUrl = this.GetTargetPageUrl("Products.aspx");

			if( this.IsStyle2 )
				// Style2 风格下，页面不需要绑定数据。数据由JS通过AJAX方式获取
				return new PageResult(papeUrl, null);


			ProductsPageModel result = new ProductsPageModel();
			result.Categories = BllFactory.GetCategoryBLL().GetList();

			if( result.Categories.Count == 0 ) {
				return new RedirectResult("/Pages/Categories.aspx");
			}

			// 获取当前用户选择的商品分类ID
			ProductSearchInfo info = new ProductSearchInfo();
			info.CategoryId = categoryId.HasValue ? categoryId.Value : 0;
			if( info.CategoryId == 0 )
				info.CategoryId = result.Categories[0].CategoryID;
			info.PageIndex = page.HasValue ? page.Value - 1 : 0;
			info.PageSize = AppHelper.DefaultPageSize;


			result.ProductInfo = new ProductInfoModel(
						result.Categories, new Product { CategoryID = info.CategoryId });
			result.PagingInfo = info;
			result.CurrentCategoryId = info.CategoryId;
			result.RequestUrlEncodeRawUrl = HttpUtility.UrlEncode(this.HttpContext.Request.RawUrl);


			// 加载商品列表
			result.Products = BllFactory.GetProductBLL().SearchProduct(info);

			return new PageResult(papeUrl, result);
		}


	}
}
