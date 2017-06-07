using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ClownFish.Log
{
	internal static class SomeExtenstions
	{
		/// <summary>
		/// 读取输入的流
		/// </summary>
		/// <param name="request">HttpRequest</param>
		/// <returns>输入的流</returns>
		public static string ReadInputStream(this HttpRequest request)
		{
			request.InputStream.Position = 0;
			using( StreamReader sr = new StreamReader(request.InputStream, request.ContentEncoding, true, 1024, true) ) {
				return sr.ReadToEnd();
			}
		}


	}
}
