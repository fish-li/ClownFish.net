using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Web.UnitTest.Controllers
{
	public class SomeService : BaseController
	{
		public void TestCookieAndHeader()
		{
			string value = this.GetCookie("cookie1").Value;
			this.WriteCookie(new System.Web.HttpCookie("cookie2", "hello_" + value));

			string value2 = this.GetHeader("header1");
			this.WriteHeader("header2", "hello_" + value2);
		}
	}
}
