using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DEMO.Model.AjaxPK
{
	public class Customer
	{
		public string Name { get; set; }
		public int Age { get; set; }
		public string Address { get; set; }
		public string Tel { get; set; }
		public string Email { get; set; }
	}


	public class BatchSubmitCustomer
	{
		public string[] Name { get; set; }
		public string[] Age { get; set; }
		public string[] Address { get; set; }
		public string[] Tel { get; set; }
		public string[] Email { get; set; }
	}

}