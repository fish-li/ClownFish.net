using System;
using System.Linq;
using System.Collections.Generic;
using ClownFish.Web;
using DEMO.Model;
using DEMO.Common;
using DEMO.BLL;


// ClownFish.Web的用法可参考：http://www.cnblogs.com/fish-li/archive/2012/02/12/2348395.html
// ClownFish.Web下载页面：http://www.cnblogs.com/fish-li/archive/2012/02/21/2361982.html


namespace DEMO.Controllers.Ajax
{

	public class ProductService
	{
		[Action]
		public void Insert(Product product)
		{
			product.EnsureItemIsOK();

			BllFactory.GetProductBLL().Insert(product);
		}

		[Action]
		public object Delete(int id, string returnUrl)
		{
			BllFactory.GetProductBLL().Delete(id);

			if( string.IsNullOrEmpty(returnUrl) )
				return null;
			else
				return new RedirectResult(returnUrl);
		}

		[Action]
		public void Update(Product product)
		{
			product.EnsureItemIsOK();

			BllFactory.GetProductBLL().Update(product);
		}

		[Action]
		public void ChangeProductQuantity(int productId, int quantity)
		{
			if( productId < 0 )
				throw new MyMessageException("没有指定ProductID");

			BllFactory.GetProductBLL().ChangeProductQuantity(productId, quantity);
		}

		[Action]
		public object GetById(int id)
		{
			Product product = BllFactory.GetProductBLL().GetProductById(id);
			if( product == null )
				throw new MyMessageException("指定的ID值无效。不能找到对应的记录。");

			return new JsonResult(product);
		}

		[Action]
		public object Show(int id)
		{
			Product product = BllFactory.GetProductBLL().GetProductById(id);
			if( product == null )
				throw new MyMessageException("指定的ID值无效。不能找到对应的记录。");

			List<Category> categories = BllFactory.GetCategoryBLL().GetList();
			ProductInfoModel data = new ProductInfoModel(categories, product);

			return new PageResult("/Pages/Style1/Controls/ProductInfo.cshtml", data);
		}

		[Action]
		public object ShowProductPicker(int? categoryId, string searchWord, int? page)
		{
			ProductSearchInfo info = new ProductSearchInfo();
			info.CategoryId = categoryId.HasValue ? categoryId.Value : 0;
			info.SearchWord = searchWord ?? string.Empty;
			info.PageIndex = page.HasValue ? page.Value - 1 : 0;
			info.PageSize = AppHelper.DefaultPageSize;


			ProductPickerModel data = new ProductPickerModel();
			data.SearchInfo = info;
			data.List = BllFactory.GetProductBLL().SearchProduct(info);
			data.CategoryList = BllFactory.GetCategoryBLL().GetList();

			return new PageResult("/Pages/Style1/Controls/ProductPicker.cshtml", data);
		}

		[Action]
		public object List(ProductSearchInfo pagingInfo)
		{
			pagingInfo.CheckPagingInfoState();

			List<Product> List = BllFactory.GetProductBLL().SearchProduct(pagingInfo);
			var result = new GridResult<Product>(List, pagingInfo.TotalRecords);
			return new JsonResult(result);
		}


		private static readonly string[] UnitArray = new string[] { "个", "件", "只", "双" };

		[Action]
		public object GetUnitList(string unit)
		{
			object result = null;

			if( string.IsNullOrEmpty(unit) ) {
				result = UnitArray.Select((x, index) => new { text = x, selected = index == 0 }).ToList();
			}
			else {
				int p = Array.IndexOf(UnitArray, unit);
				if( p >= 0 ) {
					result = UnitArray.Select((x, index) => new { text = x, selected = index == p }).ToList();
				}
				else {
					List<string> list = UnitArray.ToList();
					list.Add(unit);
					result = list.Select((x, index) => new { text = x, selected = index == 0 }).ToList();
				}
			}
			return new JsonResult(result);
		}


	}
}
