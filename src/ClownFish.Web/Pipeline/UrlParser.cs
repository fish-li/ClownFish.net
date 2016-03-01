using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using ClownFish.Web.Debug404;

namespace ClownFish.Web
{
	/// <summary>
	/// URL解析器，用于从URL中提取基本的UrlActionInfo信息
	/// 注意：这个类型的实例会被静态变量引用，因此要求是线程安全的。
	/// </summary>
	public class UrlParser
	{
		internal static readonly Regex ServiceUrlRegex
			= new Regex(@"/(?<type>\w+)/((?<namespace>[\.\w-]+)[/\.])?(?<name>\w+)/(?<method>\w+)(?<extname>\.[a-zA-Z]+)?", RegexOptions.Compiled);

		// 说明：命名空间中和类名之间可以用 / 和 . 分隔，但是在<system.web>/<httpHandlers>节点中要注意 / 出现的次数与URL中的次数匹配
		//        IIS的集成模式没有这个问题。

		// 补充说明：以上正则表示式当遇到REST风格时，如果在method位置包含了非字符类的文字作为方法的传入参数，匹配后将会造成数据丢失。
		//          因为是采用的 (?<method>\w+) ，除非换成 (?<method>[^\.]+) ，并且在得到数据后做UrlDecode


		/*
可以解析以下格式的URL：（前一个表示包含命名空间的格式）

/service/namespace.Fish.AA.Demo/GetMd5.aspx
/service/namespace.Fish.AA/Demo/GetMd5.aspx
/service/Demo/GetMd5.aspx
/api/aa/Demo/GetMd5
/api/aa.b/Demo/GetMd5?ss=a.b
		*/

		
		/// <summary>
		/// 从指定的请求中提取UrlActionInfo
		/// </summary>
		/// <param name="context"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public virtual UrlActionInfo GetUrlActionInfo(HttpContext context, string path)
		{
			// 扩展点：允许自定义URL解析逻辑

			if( string.IsNullOrEmpty(path) )
				throw new ArgumentNullException("path");

			Match match = ServiceUrlRegex.Match(path);
			if( match.Success == false ) {
				DiagnoseResult diagnoseResult = Http404DebugModule.TryGetDiagnoseResult(context);
				if( diagnoseResult != null )
					diagnoseResult.ErrorMessages.Add("URL解析失败，正则表达式：" + ServiceUrlRegex.ToString());

				return null;
			}

			return new UrlActionInfo {
				UrlType = match.Groups["type"].Value,
				Namesapce = match.Groups["namespace"].Value,
				ClassName = match.Groups["name"].Value,
				MethodName = match.Groups["method"].Value,
				ExtName = match.Groups["extname"].Value
			};
		}


	}
}
