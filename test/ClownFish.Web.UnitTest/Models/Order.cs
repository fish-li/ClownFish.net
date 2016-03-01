using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Web;

namespace ClownFish.Web.UnitTest.Models
{
	public sealed class Order 
	{
		public int OrderID { get; set; }
		public int? CustomerID { get; set; }
		public DateTime OrderDate { get; set; }
		public decimal SumMoney { get; set; }
		public string Comment { get; set; }

		[HttpValueIgnore]
		public bool Finished { get; set; }

	}
}
