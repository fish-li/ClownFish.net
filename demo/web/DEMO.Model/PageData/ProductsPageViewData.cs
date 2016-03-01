using System.Collections.Generic;

using ClownFish;

namespace DEMO.Model
{
	public class ProductsPageModel
	{
		public PagingInfo PagingInfo;
		public List<Category> Categories;
		public int CurrentCategoryId;
		public List<Product> Products;

		public string RequestUrlEncodeRawUrl;

		public ProductInfoModel ProductInfo { get; set; }
	}
}