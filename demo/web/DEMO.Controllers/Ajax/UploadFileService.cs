using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ClownFish.Web;

namespace DEMO.Controllers.Ajax
{
	public class UploadFileService
	{

		[Action(Verb = "post", OutFormat = SerializeFormat.Json)]
		public object Test1(HttpFile a, HttpFile b, int c, int d)
		{
			var result = new {
				file1 = new { name = a.FileName, lenght = a.FileBody.Length },
				file2 = new { name = b.FileName, lenght = b.FileBody.Length },
				sum = c + d + a.ContentLength + b.ContentLength
			};

			return result;
		}


		[Action(Verb = "post", OutFormat = SerializeFormat.Json)]
		public object Test2([ContextData("Request.Files")]HttpFileCollection files, int c, int d)
		{
			HttpPostedFile a = files["a"];
			HttpPostedFile b = files["b"];

			return new {
				file1 = new { name = a.FileName, lenght = a.InputStream.Length },
				file2 = new { name = b.FileName, lenght = b.InputStream.Length },
				sum = c + d + a.ContentLength + b.ContentLength
			};
		}


	}
}
