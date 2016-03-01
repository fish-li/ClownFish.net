using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Web;

namespace ClownFish.Web
{
	internal class GzipModule : IHttpModule
	{
		public void Init(HttpApplication app)
		{
			app.BeginRequest += new EventHandler(app_BeginRequest);
		}

		void app_BeginRequest(object sender, EventArgs e)
		{
			HttpApplication app = (HttpApplication)sender;

			HttpWorkerRequest request = (((IServiceProvider)app.Context)
							.GetService(typeof(HttpWorkerRequest)) as HttpWorkerRequest);

			string gzipHeader = request.GetUnknownRequestHeader("X-Gzip-Respond");

			if( gzipHeader == "1" ) {
				app.Response.Filter = new GZipStream(app.Response.Filter, CompressionMode.Compress);
				app.Response.AppendHeader("Content-Encoding", "gzip");
			}
		}


		public void Dispose()
		{
		}
	}
}
