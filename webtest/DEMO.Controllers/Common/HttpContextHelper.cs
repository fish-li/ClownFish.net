using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEMO.Controllers.Common
{
	public static class HttpContextHelper
	{
		public static string GetLoginName(this HttpContext context)
		{
			if( context.Request.IsAuthenticated == false )
				return null;

			return context.User.Identity.Name;
		}
	}
}