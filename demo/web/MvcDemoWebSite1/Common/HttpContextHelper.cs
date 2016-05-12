using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcDemoWebSite1.Common
{
	public static class HttpContextHelper
	{
		public static string UserIdentityName {
			get {
				if( HttpContext.Current.Request.IsAuthenticated == false )
					return null;

				return HttpContext.Current.User.Identity.Name;
			}
		}
	}
}