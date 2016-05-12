using System.Collections.Generic;
using DEMO.Common;
using ClownFish;

namespace DEMO.Model
{
	public class CustomersPageModel
	{
		public Customer Customer { get; set; }
		public PagingInfo PagingInfo { get; set; }
		public List<Customer> List { get; set; }

		public string RequestUrlEncodeRawUrl { get; set; }

		public CustomersPageModel()
		{
			this.Customer = new Customer();
		}
	}
}