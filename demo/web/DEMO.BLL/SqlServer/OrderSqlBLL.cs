using System;
using System.Collections.Generic;
using System.Linq;
using DEMO.Model;
using DEMO.Common;
using ClownFish;


namespace DEMO.BLL
{
	/// <summary>
	/// 操作“订单记录”的业务逻辑层
	/// 为了简单演示，每个方法将打开一个连接。
	/// </summary>
	public sealed class OrderSqlBLL : IOrderBLL
	{
		public int AddOrder(Order order)
		{
			// 以事务的方式创建一个DbContext对象，将使用默认的连接字符串
			using( DbContext dbContext = new DbContext(true) ) {
				// 添加记录到表Orders，同时获取新产生ID
				DbHelper.ExecuteNonQuery("InsertOrder", order, dbContext);

				// 为订单明细设置OrderId，并添加到表[Order Details]
				order.Details.ForEach(x => {
					x.OrderID = order.OrderID;
					DbHelper.ExecuteNonQuery("InsertOrderDetail", x, dbContext);
				});

				// 刷新订单总金额。
				DbHelper.ExecuteNonQuery("RefreshOrderSumMoney", new { OrderID = order.OrderID }, dbContext);

				// 提交事务。
				dbContext.CommitTransaction();

				return order.OrderID;
			}
		}


		// 根据指定的查询日期范围及分页参数，获取订单记录列表
		public List<Order> Search(OrderSearchInfo option)
		{
			return DbHelper.FillListPaged<Order>("SearchOrder", option);
		}

		public Order GetOrderById(int orderId)
		{
			using( DbContext dbContext = new DbContext(false) ) {
				Order order = DbHelper.GetDataItem<Order>("GetOrderById", new { OrderID = orderId }, dbContext);
				if( order != null )
					order.Details = DbHelper.FillList<OrderDetail>("GetOrderDetails", new { OrderID = orderId }, dbContext);

				return order;
			}
		}


		public void SetOrderStatus(int orderId, bool finished)
		{
			var parameters = new { OrderID = orderId, Finished = finished };
			DbHelper.ExecuteNonQuery("SetOrderStatus", parameters);
		}



	}
}
