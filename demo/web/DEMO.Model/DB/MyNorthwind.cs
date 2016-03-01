using System.Collections.Generic;

namespace DEMO.Model
{
	public class MyNorthwind
	{
		public volatile List<Category> Categories;
		public volatile List<Customer> Customers;
		public volatile List<Product> Products;
		public volatile List<Order> Orders;
	}

}