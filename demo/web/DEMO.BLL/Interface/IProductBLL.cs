using System;


namespace DEMO.BLL
{
	public interface IProductBLL
	{
		void ChangeProductQuantity(int productId, int quantity);
		void Delete(int productId);
		DEMO.Model.Product GetProductById(int productId);
		void Insert(DEMO.Model.Product product);
		System.Collections.Generic.List<DEMO.Model.Product> SearchProduct(DEMO.Model.ProductSearchInfo info);
		void Update(DEMO.Model.Product product);
	}
}
