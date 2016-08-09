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

			HttpFile httpFile = HttpFile.CreateHttpFileFromHttpPostedFile(file);
			if( httpFile != null )
				httpFile.Key = paraName;

			return httpFile;
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
				if( files[i] != null ) {
					files[i].Key = context.Request.Files.GetKey(i);
				}
			}

			return files;
		}
	}
}
