using System;
using System.Collections.Generic;
using System.Linq;
using DEMO.Model;
using DEMO.Common;
using ClownFish;

namespace DEMO.BLL
{

	public sealed class CustomerSqlBLL : ICustomerBLL
	{
		public void Insert(Customer customer)
		{
			DbHelper.ExecuteNonQuery("InsertCustomer", customer);
		}

		public void Delete(int customerId)
		{
			var parameters = new { CustomerID = customerId };
			DbHelper.ExecuteNonQuery("DeleteCustomer", parameters);
		}

		public void Update(Customer customer)
		{
			DbHelper.ExecuteNonQuery("UpdateCustomer", customer);
		}


		// 根据指定的查询关键词及分页参数，获取客户列表.
		public List<Customer> GetList(CustomerSearchInfo info)
		{
			return DbHelper.FillListPaged<Customer>("GetCustomerList", info);
		}

		public Customer GetById(int customerId)
		{
			var parameters = new { CustomerID = customerId };
			return DbHelper.GetDataItem<Customer>("GetCustomerById", parameters);
		}




	}
}
