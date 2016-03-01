using System;

namespace DEMO.BLL
{
	public interface IOrderBLL
	{
		int AddOrder(DEMO.Model.Order order);
		DEMO.Model.Order GetOrderById(int orderId);
		System.Collections.Generic.List<DEMO.Model.Order> Search(DEMO.Model.OrderSearchInfo option);
		void SetOrderStatus(int orderId, bool finished);
	}
}
