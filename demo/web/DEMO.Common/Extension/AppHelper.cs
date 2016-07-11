using System;
using System.Web;
using System.IO;
using DEMO.Common.Extension;
using ClownFish.Web;
using ClownFish.Web.Serializer;

namespace DEMO.Common
{
	public static class AppHelper
	{
		public static readonly int DefaultPageSize = 12;
		public static readonly string AppName = "ClownFish.Web-DEMO";


		public static void Init()
		{
			// 演示：注册自定义的类型转换委托
			HttpDataConvertFactory.Register<int[]>(new IntArrayConvertor());
		}


		/// <summary>
		/// 安全地记录一个异常对象到文本文件。
		/// </summary>
		/// <param name="ex"></param>
		public static void SafeLogException(Exception ex)
		{
			if( ex is MyMessageException )
				return;

			if( ex is HttpException ) {
				HttpException ee = ex as HttpException;
				if( ee.GetHttpCode() == 404 )
					return;
			}


			try {
				string logfilePath = Path.Combine(HttpRuntime.AppDomainAppPath, @"App_Data\ErrorLog.txt");
				string message = ex.ToString() + "\r\n\r\n\r\n";
				if( HttpContext.Current != null )
					message = "Url: " + HttpContext.Current.Request.RawUrl + "\r\n" + message;

				File.AppendAllText(logfilePath, message, System.Text.Encoding.UTF8);
			}
			catch { }
		}





	}
}