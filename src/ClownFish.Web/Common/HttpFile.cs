using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Reflection;
using System.IO;

namespace ClownFish.Web
{
	/// <summary>
	/// 表示一个符合HTTP协议的上传文件
	/// </summary>
	[Serializable]
	public sealed class HttpFile
	{
		/// <summary>
		/// 获取上载文件的大小（以字节为单位）。
		/// </summary>
		public int ContentLength { get; private set; }
		/// <summary>
		/// 获取客户端发送的文件的 MIME 内容类型。
		/// </summary>
		public string ContentType { get; private set; }
		/// <summary>
		/// 获取客户端上的文件的完全限定名称。
		/// </summary>
		public string FileName { get; set; }
		/// <summary>
		/// 获取上传文件的内容
		/// </summary>
		public byte[] FileBody { get; set; }



		internal static HttpFile CreateFromFileInfo(FileInfo file)
		{
			HttpFile result = new HttpFile();
			result.FileName = file.FullName;
			result.ContentLength = (int)file.Length;
			result.FileBody = System.IO.File.ReadAllBytes(file.FullName);
			return result;
		}

		private static HttpFile CreateHttpFileFromHttpPostedFile(HttpPostedFile file)
		{
			HttpFile result = new HttpFile();
			result.ContentLength = file.ContentLength;
			result.ContentType = file.ContentType;
			result.FileName = file.FileName;

			if( file.ContentLength > 0 ) {
				result.FileBody = new byte[file.ContentLength];		// 如果文件很大，可能会出问题！
				file.InputStream.Read(result.FileBody, 0, file.ContentLength);
			}
			else {
				result.FileBody = new byte[0];
			}
			
			return result;
		}

		internal static HttpFile GetFromHttpRequest(HttpContext context, ParameterInfo p)
		{
			HttpPostedFile file = context.Request.Files[p.Name];

			return CreateHttpFileFromHttpPostedFile(file);
		}

		internal static HttpFile[] GetFilesFromHttpRequest(HttpContext context, ParameterInfo p)
		{
			HttpFile[] files = new HttpFile[context.Request.Files.Count];

			for( int i = 0; i < context.Request.Files.Count; i++ ) {
				HttpPostedFile file = context.Request.Files[i];

				files[i] = CreateHttpFileFromHttpPostedFile(file);
			}

			return files;
		}

	}
}
