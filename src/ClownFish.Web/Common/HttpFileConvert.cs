using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Reflection;
using System.IO;
using ClownFish.Web.Serializer;
using ClownFish.Base.Http;

namespace ClownFish.Web
{
	
	internal class HttpFileDataConvert : IHttpDataConvert
	{
		public object Convert(HttpContext context, string paraName)
		{
			HttpPostedFile file = context.Request.Files[paraName];

			return HttpFile.CreateHttpFileFromHttpPostedFile(file);
		}
	}


	internal class HttpFileArrayDataConvert : IHttpDataConvert
	{
		public object Convert(HttpContext context, string paraName)
		{
			HttpFile[] files = new HttpFile[context.Request.Files.Count];

			for( int i = 0; i < context.Request.Files.Count; i++ ) {
				HttpPostedFile file = context.Request.Files[i];

				files[i] = HttpFile.CreateHttpFileFromHttpPostedFile(file);
			}

			return files;
		}
	}
}
