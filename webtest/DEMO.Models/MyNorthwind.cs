using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Web;
using ClownFish.Base;
using ClownFish.Base.Xml;

namespace DEMO.Models
{
	public static class WebSiteDB
	{
		private static MyNorthwind s_db;

		public static MyNorthwind MyNorthwind {
			get {
				if( s_db == null )
					LoadDbFromXml();

				return s_db;
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private static void LoadDbFromXml()
		{
			if( s_db != null )
				return;

			string xmlPath = Path.Combine(HttpRuntime.AppDomainAppPath, @"App_Data\MyNorthwindDataBase.xml");

			if( RetryFile.Exists(xmlPath) == false )
				throw new ArgumentException("指定的文件不存在：" + xmlPath);

			s_db = XmlHelper.XmlDeserializeFromFile<MyNorthwind>(xmlPath);
		}

	}


	public class MyNorthwind
	{
		public List<Category> Categories { get; set; }
		public List<Customer> Customers { get; set; }
		public List<Product> Products { get; set; }
		public List<Order> Orders { get; set; }
	}


	public sealed class Category
	{
		public int CategoryID { get; set; }
		public string CategoryName { get; set; }
	}


	public sealed class Customer
	{
		public int CustomerID { get; set; }
		public string CustomerName { get; set; }
		public string ContactName { get; set; }
		public string Address { get; set; }
		public string PostalCode { get; set; }
		public string Tel { get; set; }

	}


	public sealed class Product
	{
		public int ProductID { get; set; }
		public string ProductName { get; set; }
		public int CategoryID { get; set; }
		public string Unit { get; set; }
		public decimal UnitPrice { get; set; }
		public int Quantity { get; set; }
		public string Remark { get; set; }
	}


	public sealed class Order
	{
		public int OrderID { get; set; }
		public int? CustomerID { get; set; }
		public string CustomerName { get; set; }
		public DateTime OrderDate { get; set; }
		public decimal SumMoney { get; set; }
		public string Comment { get; set; }
		public bool Finished { get; set; }

		public List<OrderDetail> Details { get; set; }

		public string OrderNo {
			get { return this.OrderID.ToString().PadLeft(12, '0'); }
		}
		public int ValidCustomerId {
			get { return this.CustomerID.HasValue ? this.CustomerID.Value : 0; }
		}		
	}


	public sealed class OrderDetail
	{
		public int OrderID { get; set; }
		public int ProductID { get; set; }
		public decimal UnitPrice { get; set; }
		public int Quantity { get; set; }

		public string ProductName { get; set; }
		public string Unit { get; set; }
	}


}