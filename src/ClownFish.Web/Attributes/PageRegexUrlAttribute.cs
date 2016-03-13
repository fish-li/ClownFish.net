using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ClownFish.Web
{

	// 第一种用法：
	// [PageRegexUrl(Url = @"/page/{id}/{year}-{month}-{day}.aspx")]
	// 上例中 {占位符} 将替换成正则表达式中的 \w+


	// 第二种用法：
	// 针对一些常用场景，数字，日期，GUID的匹配，可以使用下面的写法：
	// /page/{id:int}/{day:date}/{g:guid}.aspx
	// 上例中，id, day, g 分别表示三个变量名称，int, date, guid表示三种类型（注意类型名称的大小写）
	// int 将替换成：   \d+
	// date 将替换成：  \d{4}-\d{1,2}-\d{1,2} ，这个并不是精确的日期匹配模式，但是容易理解
	// guid 将替换成：  [a-fA-F0-9\-]{36}     ，这个并不是精确的GUID匹配模式，但看起来短小清晰，所以不要纠结！
	// 补充说明：匹配模式越精确，对于不能匹配之后发生的404现象也会越来越多，所以【宽松】模式也是有必要的。


	// 第三种用法：
	// 如果{占位符}不能满足需要，例如：希望匹配GUID中的横线，还可以直接使用正则表达式，例如下面的写法
	// [PageRegexUrl(Url = @"/m/(?<guid>[^/]{36})\.aspx")]


	/// <summary>
	/// 指示Url是一个正则表达式
	/// 注意：需要在正则表达式的模式中指出分组名称，用于给Action对应名称的参数赋值。
	/// 简单用法：[PageRegexUrl(Url = @"/page/{id}/{year}-{month}-{day}.aspx")]
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public sealed class PageRegexUrlAttribute : PageUrlAttribute
	{
		/// <summary>
		/// 用于替换{占位符}转变成标准正则表达式的Regex
		/// </summary>
		private static readonly Regex s_regex = new Regex(@"{((?<name>\w+)(\:(?<type>int|date|guid))?)}", RegexOptions.Compiled);


		// 简单的模式替换，由于不能处理数据类型 {g:guid}，所以不使用了。
		// 放在这里做个纪念，不要删除！
		//private static readonly Regex s_regex = new Regex(@"{(\w+)}", RegexOptions.Compiled);
		//string newString = s_regex.Replace(this.Url, @"(?<$1>\w+)");

		internal Regex GetRegex()
		{
			if( string.IsNullOrEmpty(this.Url) )
				return null;

			if( this.Url.IndexOf('{') > 0 ) {
				// 替换{占位符}，例如：/page/{id}/{year}-{month}-{day}.aspx
				// 将变成：/page/(?<id>\w+)/(?<year>\w+)-(?<month>\w+)-(?<day>\w+).aspx

				string newString = s_regex.Replace(this.Url, EvalReplace);

				return new Regex(newString, RegexOptions.Compiled | RegexOptions.IgnoreCase);
			}
			else {
				return new Regex(this.Url, RegexOptions.Compiled | RegexOptions.IgnoreCase);
			}
		}

		private string EvalReplace(Match match)
		{
			string type = match.Groups["type"].Value;
			string name = match.Groups["name"].Value;

			if( string.IsNullOrEmpty(type) ) {
				return string.Format(@"(?<{0}>\w+)", name);
			}
			else {
				switch( type ) {		// 注意：数据类型名称区分大小写！
					case "int":
						return string.Format(@"(?<{0}>\d+)", name);

					case "date":
						return string.Format(@"(?<{0}>\d{{4}}-\d{{1,2}}-\d{{1,2}})", name);

					case "guid":
						return string.Format(@"(?<{0}>[a-fA-F0-9\-]{{36}})", name);

					default:
						throw new NotSupportedException("PageRegexUrlAttribute不支持未知的占位符数据类型：" + type);
				}
			}
		}

	}
}
