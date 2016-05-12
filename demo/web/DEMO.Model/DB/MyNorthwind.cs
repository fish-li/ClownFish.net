using System.Collections.Generic;

namespace DEMO.Model
{
	public class MyNorthwind
	{
		public List<Category> Categories { get; set; }
		public List<Customer> Customers { get; set; }
		public List<Product> Products { get; set; }
		public List<Order> Orders { get; set; }
	}

}