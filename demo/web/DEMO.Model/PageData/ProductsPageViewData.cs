using System.Collections.Generic;

using ClownFish;

namespace DEMO.Model
{
	public class ProductsPageModel
	{
		public PagingInfo PagingInfo { get; set; }
		public List<Category> Categories { get; set; }
		public int CurrentCategoryId { get; set; }
		public List<Product> Products { get; set; }

		public string RequestUrlEncodeRawUrl { get; set; }

		public ProductInfoModel ProductInfo { get; set; }
	}
}