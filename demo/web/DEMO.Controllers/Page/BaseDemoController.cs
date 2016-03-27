using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ClownFish.Web;

namespace DEMO.Controllers.Page
{
	public abstract class BaseDemoController : BaseController
	{
		public static readonly string STR_PageStyle = "PageStyle";
		public static readonly string[] StyleArray = new string[] { "Style1", "Style2", "Style3" };


		public static string CurrentStyle
		{
			get
			{
				HttpCookie styleCookie = HttpContext.Current.Request.Cookies[STR_PageStyle];

				if( styleCookie != null && string.IsNullOrEmpty(styleCookie.Value) == false )
					return styleCookie.Value;

				return StyleArray[1];
			}
		}

		public string GetTargetPageUrl(string pageName)
		{
			string currentPageStyle = CurrentStyle;

			if( currentPageStyle == StyleArray[0] ) {
				string cshtml = pageName.Substring(0, pageName.Length - 4) + "cshtml";
				return string.Format("/Pages/{0}/" + cshtml, currentPageStyle);
			}
			else
				return string.Format("/Pages/{0}/" + pageName, currentPageStyle);
		}


		public bool IsStyle2
		{
			get { return CurrentStyle == StyleArray[1]; }
		}
	}
}
