using System;
using System.Collections.Generic;
using System.Linq;
using DEMO.Model;
using DEMO.Common;
using ClownFish;


namespace DEMO.BLL
{
	/// <summary>
	/// 操作“商品记录”的业务逻辑层
	/// 为了简单演示，每个方法将打开一个连接。
	/// </summary>
	public sealed class ProductSqlBLL : IProductBLL
	{
		public void Insert(Product product)
		{
			DbHelper.ExecuteNonQuery("InsertProduct", product);
		}

		public void Delete(int productId)
		{
			var parameters = new { ProductID = productId };
			DbHelper.ExecuteNonQuery("DeleteProduct", parameters);
		}

		public void Update(Product product)
		{
			DbHelper.ExecuteNonQuery("UpdateProduct", product);
		}

		public Product GetProductById(int productId)
		{
			var parameters = new { ProductID = productId };
			return DbHelper.GetDataItem<Product>("GetProductById", parameters);
		}

		public List<Product> SearchProduct(ProductSearchInfo info)
		{
			return DbHelper.FillListPaged<Product>("SearchProduct", info);
		}

		public void ChangeProductQuantity(int productId, int quantity)
		{
			var parameters = new { ProductID = productId, Quantity = quantity };
			DbHelper.ExecuteNonQuery("ChangeProductQuantity", parameters);
		}

	}
}
