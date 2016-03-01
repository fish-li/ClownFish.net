using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Web.UnitTest.Models
{
	public class Product
	{
		public int ProductID { get; set; }
		public string ProductName { get; set; }
		public int CategoryID { get; set; }
		public string Unit { get; set; }
		public decimal UnitPrice { get; set; }
		public int Quantity { get; set; }
		public string Remark { get; set; }

		public override string ToString()
		{
			return string.Format("id={0};name={1}", this.ProductID, this.ProductName);
		}
	}
}
