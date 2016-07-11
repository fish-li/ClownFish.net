using System;
using System.Collections.Generic;
using DEMO.Common;
using ClownFish.Web;
using ClownFish.Base;

namespace DEMO.Model
{
	public sealed class Order : MyDataItem
	{
		public int OrderID { get; set; }
		public int? CustomerID { get; set; }
		public DateTime OrderDate { get; set; }
		public decimal SumMoney { get; set; }
		public string Comment { get; set; }
		public bool Finished { get; set; }

		public List<OrderDetail> Details { get; set; }

		public string OrderNo
		{
			get { return this.OrderID.ToString().PadLeft(12, '0'); }
		}
		public int ValidCustomerId
		{
			get { return this.CustomerID.HasValue ? this.CustomerID.Value : 0; }
		}
		public string CustomerName { get; set; }
	}


	public sealed class OrderDetail : MyDataItem
	{
		public int OrderID { get; set; }
		public int ProductID { get; set; }
		public decimal UnitPrice { get; set; }
		public int Quantity { get; set; }

		public string ProductName { get; set; }
		public string Unit { get; set; }
	}

	public sealed class OrderSubmitForm : MyDataItem
	{
		public DateTime OrderDate { get; set; }
		public int CustomerID { get; set; }
		public string OrderDetail { get; set; }
		public string Comment { get; set; }


		public override string IsValid()
		{
			if( this.OrderDate.Year < 2000 )
				return "无效的日期。";

			if( string.IsNullOrEmpty(OrderDetail) )
				return "没有订单明细项目。";

			return null;
		}

		public Order ConvertToOrderItem()
		{
			// 验证对象是否有效
			string error = this.IsValid();
			if( string.IsNullOrEmpty(error) == false )
				throw new MyMessageException(error);

			// 创建实体对象
			Order order = new Order();
			DateTime now = DateTime.Now;
			order.OrderDate = new DateTime(this.OrderDate.Year, this.OrderDate.Month, this.OrderDate.Day, now.Hour, now.Minute, now.Second);

			if( this.CustomerID > 0 )
				order.CustomerID = this.CustomerID;
			order.Comment = this.Comment;

			order.Details = new List<OrderDetail>();
			// 将 id1=2;id2=3; 的格式字符串分解出来
			List<NameValue> detail = this.OrderDetail.SplitString(';', '=');
			// 将列表转成订单明细
			detail.ForEach(x => order.Details.Add(new OrderDetail { ProductID = int.Parse(x.Name), Quantity = int.Parse(x.Value) }));

			return order;
		}
	}
}
