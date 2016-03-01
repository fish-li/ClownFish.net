using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using ClownFish.Web;

namespace DEMO.Controllers
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class CheckRightAttribute : AuthorizeAttribute
	{
		public string RightNo { get; set; }

		public override bool AuthenticateRequest(HttpContext context)
		{
			if( context.Request.IsAuthenticated == false )
				return false;

			HttpCookie rightNoCookie = context.Request.Cookies["rightNo_demo"];

			if( rightNoCookie != null )
				return (RightNo == rightNoCookie.Value);
			else
				return false;
		}
	}
}
