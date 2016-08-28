using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ClownFish.Base.TypeExtend;

namespace ClownFish.Web.UnitTest.Ext
{
	public class WebRuntimeExt : WebRuntime
	{
		public Hashtable CallMessage = new Hashtable();

		public override void WriteResponseHeader(HttpResponse response, string headerName, string headerValue)
		{
			CallMessage[headerName] = headerValue;
		}

		public override bool IsDebugMode
		{
			get
			{
				return true;
			}
		}
	}
}
