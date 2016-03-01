using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ClownFish.Web;

namespace ClownFish.Web.UnitTest.Controllers
{
	public class SpecialDataTypeService : BaseController
	{
		public string Input_HttpContext(HttpContext context)
		{
			return context.Items["unit-test"].ToString();
		}


		public string Input_querystring(NameValueCollection queryString)
		{
			return queryString["a"];
		}


		public string Input_form(NameValueCollection form)
		{
			return form["a"];
		}


		public string Input_headers(NameValueCollection headers)
		{
			return headers["a"];
		}



















	}
}
