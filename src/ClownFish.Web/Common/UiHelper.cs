using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using ClownFish.Base;

namespace ClownFish.Web
{
	/// <summary>
	/// UI相关的工具类
	/// </summary>
	public static class UiHelper
	{
		// 网站根目录。
		// 注意：如果这段代码没有运行在ASP.NET环境中，会出现异常！
		internal static readonly string AppRoot = WebRuntime.Instance.GetWebSitePath().TrimEnd('\\');


		/// <summary>
		/// 生成一个引用JS文件的HTML代码，其中URL包含了文件的最后更新时间。
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string RefJsFileHtml(string path)
		{
			return RefJsFileHtml(path, false);
		}

		/// <summary>
		/// 生成引入JavaScript文件的HTML代码
		/// </summary>
		/// <param name="path"></param>
		/// <param name="inline"></param>
		/// <returns></returns>
		public static string RefJsFileHtml(string path, bool inline)
		{
			string filePath = AppRoot + path.Replace("/", "\\");
			if( inline ) {
				return string.Format("<script type=\"text/javascript\">\r\n{0}\r\n</script>",
                    RetryFile.ReadAllText(filePath, Encoding.UTF8));
			}
			else {
				string version = RetryFile.GetLastWriteTimeUtc(filePath).Ticks.ToString();
				return string.Format("<script type=\"text/javascript\" src=\"{0}?_t={1}\"></script>", path, version);
			}
		}

		/// <summary>
		/// 生成一个引用CSS文件的HTML代码，其中URL包含了文件的最后更新时间。
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string RefCssFileHtml(string path)
		{
			//string filePath = s_root + path.Replace("/", "\\");
			//string version = File.GetLastWriteTimeUtc(filePath).Ticks.ToString();
			//return string.Format("<link type=\"text/css\" rel=\"Stylesheet\" href=\"{0}?_t={1}\" />", path, version);

			return RefCssFileHtml(path, false);
		}

		/// <summary>
		/// 生成引入CSS文件的HTML代码
		/// </summary>
		/// <param name="path"></param>
		/// <param name="inline"></param>
		/// <returns></returns>
		public static string RefCssFileHtml(string path, bool inline)
		{
			string filePath = AppRoot + path.Replace("/", "\\");
			if( inline ) {
				return string.Format("<style type=\"text/css\">\r\n{0}\r\n</style>",
                    RetryFile.ReadAllText(filePath, Encoding.UTF8));
			}
			else {
				string version = RetryFile.GetLastWriteTimeUtc(filePath).Ticks.ToString();
				return string.Format("<link type=\"text/css\" rel=\"Stylesheet\" href=\"{0}?_t={1}\" />", path, version);
			}
		}
	}
}
