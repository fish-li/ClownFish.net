using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Reflection;

namespace DEMO.Common.Extension
{
	public static class TestDataTypeConvertor
	{
		public static int[] GetIntArray(HttpContext conext, ParameterInfo p)
		{
			string[] val = conext.Request.QueryString.GetValues(p.Name);

			if( val == null )
				val = conext.Request.Form.GetValues(p.Name);

			return (from s in val select int.Parse(s)).ToArray();
		}
	}
}
