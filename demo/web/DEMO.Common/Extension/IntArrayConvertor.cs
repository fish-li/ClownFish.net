using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Reflection;
using ClownFish.Web;

namespace DEMO.Common.Extension
{
	public class IntArrayConvertor : IHttpDataConvert
	{
		public object Convert(HttpContext context, string paraName)
		{
			string[] val = context.Request.QueryString.GetValues(paraName);

			if( val == null )
				val = context.Request.Form.GetValues(paraName);

			return (from s in val select int.Parse(s)).ToArray();
		}
	}
}
