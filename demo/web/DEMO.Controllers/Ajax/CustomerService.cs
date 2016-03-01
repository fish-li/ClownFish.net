using System;
using ClownFish.Web;
using DEMO.Model;
using DEMO.Common;
using System.Collections.Generic;
using DEMO.BLL;


// ClownFish.Web的用法可参考：http://www.cnblogs.com/fish-li/archive/2012/02/12/2348395.html
// ClownFish.Web下载页面：http://www.cnblogs.com/fish-li/archive/2012/02/21/2361982.html


namespace DEMO.Controllers.Ajax
{

	public class CustomerService
	{
		[Action]
		public void Insert(Customer customer)
		{
			customer.EnsureItemIsOK();

			BllFactory.GetCustomerBLL().Insert(customer);
		}

		[Action]
		public object Delete(int id, string returnUrl)
		{
			BllFactory.GetCustomerBLL().Delete(id);

			if( string.IsNullOrEmpty(returnUrl) )
				return null;
			else
				return new RedirectResult(returnUrl);
		}

		[Action]
		public void Update(Customer customer)
		{
			customer.EnsureItemIsOK();

			BllFactory.GetCustomerBLL().Update(customer);
		}

		[Action]
		public object GetById(int id)
		{
			Customer customer = BllFactory.GetCustomerBLL().GetById(id);
			if( customer == null )
				throw new MyMessageException("指定的ID值无效。不能找到对应的记录。");

			return new JsonResult(customer);
		}

		[Action]
		public object Show(int id)
		{
			Customer customer = BllFactory.GetCustomerBLL().GetById(id);
			if( customer == null )
				throw new MyMessageException("指定的ID值无效。不能找到对应的记录。");

			return new PageResult("/Pages/Style1/Controls/CustomerInfo.cshtml", customer);
		}

		[Action]
		public object ShowCustomerPicker(string searchWord, int? page)
		{
			CustomerSearchInfo info = new CustomerSearchInfo();
			info.SearchWord = searchWord ?? string.Empty;
			info.PageIndex = page.HasValue ? page.Value - 1 : 0;
			info.PageSize = AppHelper.DefaultPageSize;


			CustomerPickerModel data = new CustomerPickerModel();
			data.SearchInfo = info;
			data.List = BllFactory.GetCustomerBLL().GetList(info);

			return new PageResult("/Pages/Style1/Controls/CustomerPicker.cshtml", data);
		}

		[Action]
		public object List(CustomerSearchInfo pagingInfo)
		{
			pagingInfo.CheckPagingInfoState();

			List<Customer> List = BllFactory.GetCustomerBLL().GetList(pagingInfo);
			var result = new GridResult<Customer>(List, pagingInfo.TotalRecords);
			return new JsonResult(result);
		}

	}
}
