using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Reflection;
using System.Globalization;
using System.IO;

namespace ClownFish.AspnetMock
{

	/// <summary>
	/// 辅助测试的工具类
	/// </summary>
	public static class WebHelper
	{
		/// <summary>
		/// 当前运行环境是否为测试环境（非ASP.NET环境）
		/// </summary>
		internal static readonly bool IsTestEnvironment = (HttpRuntime.AppDomainAppId == null);

		/// <summary>
		/// 用于替代 Request.MapPath 方法，保证代码的可测试性
		/// </summary>
		/// <param name="request"></param>
		/// <param name="virtualPath"></param>
		/// <returns></returns>
		public static string GetMappingPath(this HttpRequest request, string virtualPath)
		{
			if( request == null )
				throw new ArgumentNullException("request");

			if( IsTestEnvironment )
				return InternalMapPath(request, virtualPath);
			
			else 
				return request.MapPath(virtualPath);			
		}

		/// <summary>
		/// 用于替代 Server.MapPath 方法，保证代码的可测试性
		/// </summary>
		/// <param name="server"></param>
		/// <param name="virtualPath"></param>
		/// <returns></returns>
		public static string GetMappingPath(this HttpServerUtility server, string virtualPath)
		{
			if( server == null )
				throw new ArgumentNullException("server");

			if( IsTestEnvironment ) {
				HttpContext context = (HttpContext)typeof(HttpServerUtility).GetInstanceField("_context").GetValue(server);
				// 为了兼容一些家伙们的错误写法，这里先做个斜杠的转换。
				return InternalMapPath(context.Request, virtualPath.Replace("\\", "/"));
			}

			else
				return server.MapPath(virtualPath);			
		}

		private static string InternalMapPath(HttpRequest request, string virtualPath)
		{
			if( string.IsNullOrEmpty(virtualPath) )
				throw new ArgumentNullException("virtualPath");

			string appDomainPath = HttpRuntime.AppDomainAppPath;
			if( string.IsNullOrEmpty(appDomainPath) )
				throw new InvalidOperationException("请先设置HttpRuntime.AppDomainAppPath。");



			if( virtualPath.StartsWith("/") == false && virtualPath.StartsWith("~/") == false ) {
				//throw new ArgumentOutOfRangeException("参数virtualPath不接受相对路径。");

				string currentFilePath = request.FilePath.Replace("/", "\\");
				currentFilePath = currentFilePath.StartsWith("~/") ? currentFilePath.Substring(2) : currentFilePath.Substring(1);

				string currentDirectory = Path.GetDirectoryName(Path.Combine(appDomainPath, currentFilePath));
				return Path.GetFullPath(Path.Combine(currentDirectory, virtualPath.Replace("/", "\\")));
			}
			else {
				string vpath = virtualPath.StartsWith("~/") ? virtualPath.Substring(2) : virtualPath.Substring(1);
				return Path.GetFullPath(Path.Combine(appDomainPath, vpath.Replace("/", "\\")));
			}
		}

		



		public static object CreateInstance(string typeName, params object[] args)
		{
			Type t = typeof(HttpContext).Assembly.GetType(typeName);

			return Activator.CreateInstance(t,
				BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance, null,
				args, CultureInfo.CurrentCulture);
		}


		public static object CreatVirtualPath(string virtualPath)
		{
			if( string.IsNullOrEmpty(virtualPath) )
				throw new ArgumentNullException("virtualPath");

			// private VirtualPath(string virtualPath)
			return CreateInstance("System.Web.VirtualPath", virtualPath);
		}

	}
}
