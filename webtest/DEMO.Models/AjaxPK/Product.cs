using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DEMO.Models.AjaxPK
{
	public class Product
	{
		public int CategoryId { get; set; }
		public string ProductName { get; set; }
		public string Unit { get; set; }
		public decimal Price { get; set; }
		public SaleMode SaleMode { get; set; }
		public string[] Colors { get; set; }
		public string Remark { get; set; }
	}



	public enum SaleMode
	{
		retail,
		wholesale
	}
}
