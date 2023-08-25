using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Http.Pipleline;

namespace ClownFish.WebHost.Objects
{
	internal sealed class HttpContextSysNet : NHttpContext
	{
		private readonly System.Net.HttpListenerContext _context;
		private readonly HttpRequestSysNet _request;
		private readonly HttpResponseSysNet _response;

		public HttpContextSysNet(System.Net.HttpListenerContext context)
		{
			_context = context;

			_request = new HttpRequestSysNet(context.Request, this);
			_response = new HttpResponseSysNet(context.Response, this);
		}


		public override object OriginalHttpContext => _context;


		public override NHttpRequest Request => _request;

		public override NHttpResponse Response => _response;


		public override bool SkipAuthorization { get; set; }

		public override IPrincipal User { get; set; }


		private XDictionary _items;
		public override XDictionary Items {
			get {
				if( _items == null )
					_items = new XDictionary();
				return _items;
			}
		}


    }
}
