using System;
using System.Collections.Generic;
using DEMO.Common;




namespace DEMO.Model
{
	public class ProductInfoModel
	{
		public List<HtmlOptionItem> Categories;
		public Product Product;

		public ProductInfoModel() { }

		public ProductInfoModel(List<Category> list, Product product)
		{
			if( list == null )
				throw new ArgumentNullException("list");
			if( product == null )
				throw new ArgumentNullException("product");

			this.Product = product;
			this.Categories = ConvertCategoryList(list, product.CategoryID);
		}

		public static List<HtmlOptionItem> ConvertCategoryList(List<Category> list, int categoryId)
		{
			if( list == null )
				throw new ArgumentNullException("list");

			List<HtmlOptionItem> categories = new List<HtmlOptionItem>(list.Count);

			list.ForEach(x => categories.Add(new HtmlOptionItem {
				Text = x.CategoryName,
				Value = x.CategoryID.ToString(),
				Selected = x.CategoryID == categoryId
			})
			);
			return categories;
		}



	}

}
