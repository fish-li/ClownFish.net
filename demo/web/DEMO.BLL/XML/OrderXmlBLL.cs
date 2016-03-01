using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DEMO.Model;
using DEMO.Common;


namespace DEMO.BLL
{
	/// <summary>
	/// 操作“订单记录”的业务逻辑层
	/// 为了简单演示，每个方法将打开一个连接。
	/// </summary>
	public sealed class OrderXmlBLL : IOrderBLL
	{

		[MethodImpl(MethodImplOptions.Synchronized)]
		public int AddOrder(Order order)
		{
			int maxId = WebSiteDB.MyNorthwind.Orders.Max(x => x.OrderID);
			order.OrderID = maxId + 1;

			if( order.CustomerID > 0 )
				order.CustomerName = WebSiteDB.MyNorthwind.Customers.Find(c => c.CustomerID == order.CustomerID).CustomerName;
			
			// 为每个订单明细记录设置【商品属性】，累加订单总金额。
			for( int i = order.Details.Count - 1; i >= 0; i-- ) {
				OrderDetail detail = order.Details[i];

				Product product = WebSiteDB.MyNorthwind.Products.FirstOrDefault(p => p.ProductID == detail.ProductID);
				if( product != null ) {
					detail.UnitPrice = product.UnitPrice;
					detail.ProductName = product.ProductName;
					detail.Unit = product.Unit;
					order.SumMoney += detail.UnitPrice * detail.Quantity;
				}
				else {
					order.Details.Remove(detail);
				}
			}

			WebSiteDB.MyNorthwind.Orders.Add(order);

			return order.OrderID;
		}


		// 根据指定的查询日期范围及分页参数，获取订单记录列表
		public List<Order> Search(OrderSearchInfo option)
		{
			option.EndDate = option.EndDate.AddDays(1);

			var query = from ord in WebSiteDB.MyNorthwind.Orders.AsQueryable()
						where ord.OrderDate >= option.StartDate && ord.OrderDate < option.EndDate
						orderby ord.OrderDate descending
						select ord;

			// 获取订单列表，此时将返回符合分页的结果。
			return query.GetPagingList<Order>(option);
		}

		/// <summary>
		/// 根据订单ID获取订单相关的所有信息
		/// </summary>
		/// <param name="orderId"></param>
		/// <returns></returns>
		public Order GetOrderById(int orderId)
		{
			return WebSiteDB.MyNorthwind.Orders.FirstOrDefault(x => x.OrderID == orderId);
		}

		/// <summary>
		/// 修改指定的订单状态
		/// </summary>
		/// <param name="orderId"></param>
		/// <param name="finished"></param>
		/// <returns></returns>
		public void SetOrderStatus(int orderId, bool finished)
		{
			Order dest = WebSiteDB.MyNorthwind.Orders.FirstOrDefault(c => c.OrderID == orderId);
			if( dest != null )
				dest.Finished = finished;
		}



	}
}
