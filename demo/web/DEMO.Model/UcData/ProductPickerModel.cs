using System.Collections.Generic;



namespace DEMO.Model
{
	public class ProductPickerModel
	{
		public ProductSearchInfo SearchInfo { get; set; }
		public List<Product> List { get; set; }
		public List<Category> CategoryList { get; set; }
	}

}
