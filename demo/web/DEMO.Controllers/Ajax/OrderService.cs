using System;
using ClownFish.Web;
using DEMO.Common;
using DEMO.Model;
using System.Collections.Generic;
using DEMO.BLL;


// ClownFish.Web的用法可参考：http://www.cnblogs.com/fish-li/archive/2012/02/12/2348395.html
// ClownFish.Web下载页面：http://www.cnblogs.com/fish-li/archive/2012/02/21/2361982.html


namespace DEMO.Controllers.Ajax
{

	public class OrderService
	{
		[Action]
		public void AddOrder(OrderSubmitForm form)
		{
			Order order = form.ConvertToOrderItem();
			BllFactory.GetOrderBLL().AddOrder(order);
		}

		[Action]
		public object Search(OrderSearchInfo info, int? page)
		{
			info.PageIndex = page.HasValue ? page.Value - 1 : 0;
			info.PageSize = AppHelper.DefaultPageSize;

			OrderListModel data = new OrderListModel();
			// 搜索数据库
			data.List = BllFactory.GetOrderBLL().Search(info);
			data.SearchInfo = info;

			return new PageResult("/Pages/Style1/Controls/OrderList.cshtml", data);
		}

		[Action]
		public object Search2(OrderSearchInfo info)
		{
			info.CheckPagingInfoState();

			List<Order> list = BllFactory.GetOrderBLL().Search(info);

			var result = new GridResult<Order>(list, info.TotalRecords);

			return new JsonResult(result);
		}


		[Action]
		public void SetOrderStatus(int id, bool finished)
		{
			if( id <= 0 )
				throw new MyMessageException("没有指定OrderId");

			BllFactory.GetOrderBLL().SetOrderStatus(id, finished);
		}

		[Action]
		public object Show(int id)
		{
			if( id <= 0 )
				throw new MyMessageException("没有指定OrderId");

			Order item = BllFactory.GetOrderBLL().GetOrderById(id);
			if( item == null )
				throw new MyMessageException("指定的ID值无效。不能找到对应的记录。");

			return new PageResult("/Pages/Style1/Controls/OrderInfo.cshtml", item);
		}

		[Action]
		public object GetById(int id)
		{
			Order item = BllFactory.GetOrderBLL().GetOrderById(id);
			if( item == null )
				throw new MyMessageException("指定的ID值无效。不能找到对应的记录。");

			return new JsonResult(item);
		}
	}
}
