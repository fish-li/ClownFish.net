using System;


namespace DEMO.BLL
{
	public interface ICustomerBLL
	{
		void Delete(int customerId);
		DEMO.Model.Customer GetById(int customerId);
		System.Collections.Generic.List<DEMO.Model.Customer> GetList(DEMO.Model.CustomerSearchInfo info);
		void Insert(DEMO.Model.Customer customer);
		void Update(DEMO.Model.Customer customer);
	}
}
