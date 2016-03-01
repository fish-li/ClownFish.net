using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DEMO.Model;
using DEMO.Common;


namespace DEMO.BLL
{

	public sealed class CustomerXmlBLL : ICustomerBLL
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Insert(Customer customer)
		{
			int maxId = WebSiteDB.MyNorthwind.Customers.Max(x => x.CustomerID);
			customer.CustomerID = maxId + 1;
			WebSiteDB.MyNorthwind.Customers.Add(customer);
		}

		public void Delete(int customerId)
		{
			WebSiteDB.MyNorthwind.Customers = (
				from c in WebSiteDB.MyNorthwind.Customers
				where c.CustomerID != customerId
				select c).ToList();
		}

		public void Update(Customer customer)
		{
			Customer dest = WebSiteDB.MyNorthwind.Customers.FirstOrDefault(c => c.CustomerID == customer.CustomerID);
			if( dest != null ) {
				dest.CustomerName = customer.CustomerName;
				dest.Address = customer.Address;
				dest.ContactName = customer.ContactName;
				dest.PostalCode = customer.PostalCode;
				dest.Tel = customer.Tel;
			}
		}


		// 根据指定的查询关键词及分页参数，获取客户列表.
		public List<Customer> GetList(CustomerSearchInfo info)
		{
			var query = from customer in WebSiteDB.MyNorthwind.Customers.AsQueryable()
						select customer;

			if( string.IsNullOrEmpty(info.SearchWord) == false )
				query = query.Where(
					c => c.CustomerName.Contains(info.SearchWord) || c.Address.Contains(info.SearchWord));

			return query.OrderBy(x => x.CustomerName).GetPagingList<Customer>(info);
		}

		public Customer GetById(int customerId)
		{
			return WebSiteDB.MyNorthwind.Customers.FirstOrDefault(c => c.CustomerID == customerId);
		}




	}
}
