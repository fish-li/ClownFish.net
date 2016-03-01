using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DEMO.Model;
using DEMO.Common;

namespace DEMO.BLL
{
	/// <summary>
	/// 操作“商品记录”的业务逻辑层
	/// 为了简单演示，每个方法将打开一个连接。
	/// </summary>
	public sealed class ProductXmlBLL : IProductBLL
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Insert(Product product)
		{
			int maxId = WebSiteDB.MyNorthwind.Products.Max(x => x.ProductID);
			product.ProductID = maxId + 1;
			WebSiteDB.MyNorthwind.Products.Add(product);
		}

		public void Delete(int productId)
		{
			WebSiteDB.MyNorthwind.Products = (
				from c in WebSiteDB.MyNorthwind.Products
				where c.ProductID != productId
				select c).ToList();
		}


		public void Update(Product product)
		{
			Product dest = WebSiteDB.MyNorthwind.Products.FirstOrDefault(c => c.ProductID == product.ProductID);
			if( dest != null ) {
				dest.ProductName = product.ProductName;
				dest.CategoryID = product.CategoryID;
				dest.Quantity = product.Quantity;
				dest.Remark = product.Remark;
				dest.Unit = product.Unit;
				dest.UnitPrice = product.UnitPrice;
			}
		}


		/// <summary>
		/// 获取指定的商品详细资料（加载所有字段）
		/// </summary>
		/// <param name="productId"></param>
		/// <returns></returns>
		public Product GetProductById(int productId)
		{
			return WebSiteDB.MyNorthwind.Products.FirstOrDefault(p => p.ProductID == productId);
		}




		// 搜索商品
		public List<Product> SearchProduct(ProductSearchInfo info)
		{
			var query = from product in WebSiteDB.MyNorthwind.Products.AsQueryable()
						select product;

			if( info.CategoryId > 0 )
				query = query.Where(p => p.CategoryID == info.CategoryId);

			if( string.IsNullOrEmpty(info.SearchWord) == false )
				query = query.Where(p => p.ProductName.Contains(info.SearchWord));


			return query.OrderBy(x => x.ProductName).GetPagingList<Product>(info);
		}



		/// <summary>
		/// 更新指定的商品数量
		/// </summary>
		/// <param name="productId"></param>
		/// <param name="quantity"></param>
		/// <returns></returns>
		public void ChangeProductQuantity(int productId, int quantity)
		{
			Product dest = WebSiteDB.MyNorthwind.Products.FirstOrDefault(c => c.ProductID == productId);
			if( dest != null ) {
				dest.Quantity = quantity;
			}
		}

	}
}
