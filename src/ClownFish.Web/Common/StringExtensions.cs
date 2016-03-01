using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ClownFish.Web
{
	/// <summary>
	/// 用于UI输出方面的常用字符串扩展
	/// </summary>
	public static class StringExtensions
	{
		/// <summary>
		/// 将字符串转换为 HTML 编码的字符串。
		/// </summary>
		/// <param name="str">要编码的字符串。</param>
		/// <returns>一个已编码的字符串。</returns>
		public static string HtmlEncode(this string str)
		{
			if( string.IsNullOrEmpty(str) )
				return string.Empty;

			return HttpUtility.HtmlEncode(str);
		}

		/// <summary>
		/// 将字符串最小限度地转换为 HTML 编码的字符串。
		/// </summary>
		/// <param name="str">要编码的字符串。</param>
		/// <returns>一个已编码的字符串。</returns>
		public static string HtmlAttributeEncode(this string str)
		{
			if( string.IsNullOrEmpty(str) )
				return string.Empty;

			return HttpUtility.HtmlAttributeEncode(str);
		}

		/// <summary>
		/// 判断二个字符串是否相等，忽略大小写的比较方式。
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool EqualsIgnoreCase(this string a, string b)
		{
			if( a == null )
				throw new ArgumentNullException("a");
			return string.Compare(a, b, StringComparison.OrdinalIgnoreCase) == 0;
		}


		/// <summary>
		/// 以忽略大小写的方式调用 string.EndsWith
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool EndsWithIgnoreCase(this string a, string b)
		{
			if( a == null )
				throw new ArgumentNullException("a");
			return a.EndsWith(b, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// 以忽略大小写的方式调用 string.EndsWith
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool StartsWithIgnoreCase(this string a, string b)
		{
			if( a == null )
				throw new ArgumentNullException("a");
			return a.StartsWith(b, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// 以忽略大小写的方式调用 string.IndexOf
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static int IndexOfIgnoreCase(this string a, string b)
		{
			if( a == null )
				throw new ArgumentNullException("a");
			return a.IndexOf(b, StringComparison.OrdinalIgnoreCase);
		}


		/// <summary>
		/// 等效于 string.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries)
		/// 且为每个拆分后的结果又做了Trim()操作。
		/// </summary>
		/// <param name="str">要拆分的字符串</param>
		/// <param name="separator">分隔符</param>
		/// <returns></returns>
		public static string[] SplitTrim(this string str, params char[] separator)
		{
			if( string.IsNullOrEmpty(str) )
				return null;
			else
				return (from s in str.Split(separator)
						let u = s.Trim()
						where u.Length > 0
						select u).ToArray();
		}


		internal static readonly char[] CommaSeparatorArray = new char[] { ',' };


		/// <summary>
		/// <para>拆分一个字符串行。如：a=1;b=2;c=3;d=4;</para>
		/// <para>此时可以调用: SplitString("a=1;b=2;c=3;d=4;", ';', '=');</para>
		/// <para>说明：对于空字符串，方法也会返回一个空的列表。</para>
		/// </summary>
		/// <param name="line">包含所有项目组成的字符串行</param>
		/// <param name="separator1">每个项目之间的分隔符</param>
		/// <param name="separator2">每个项目内的分隔符</param>
		/// <returns>拆分后的结果列表</returns>
		public static List<NameValue> SplitString(this string line, char separator1, char separator2)
		{
			if( string.IsNullOrEmpty(line) )
				return new List<NameValue>();

			string[] itemArray = line.Split(new char[] { separator1 }, StringSplitOptions.RemoveEmptyEntries);
			List<NameValue> list = new List<NameValue>(itemArray.Length);

			//char[] separator2Array = new char[] { separator2 };

			foreach( string item in itemArray ) {
				//string[] parts = item.Trim().Split(separator2Array, StringSplitOptions.RemoveEmptyEntries);
				//if( parts.Length != 2 )
				//	throw new ArgumentException("要拆分的字符串的格式无效。");

				//list.Add(new NameValue { Name = parts[0], Value = parts[1] });

				// Cookie允许在一个名称下写多个子节点，例如：SRCHUSR=AUTOREDIR=0&GEOVAR=&DOB=20141216; _EDGE_V=1;
				// 如果使用上面的拆分方式，会抛出异常，
				// 所以，调整为，只判断有没有【分隔符】，而不管出现多次

				int p = item.IndexOf(separator2);
				if( p<= 0 )
					throw new ArgumentException("要拆分的字符串的格式无效。");
				
				list.Add(new NameValue { 
					Name = item.Substring(0, p).Trim(), 
					Value = item.Substring(p +1).Trim() });
			}
			return list;
		}


		/// <summary>
		/// 将字符串的首个英文字母大写
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string ToTitleCase(this string text)
		{
			// 重新实现：CultureInfo.CurrentCulture.TextInfo.ToTitleCase
			// 那个方法太复杂了，重新实现一个简单的版本。

			if( text == null || text.Length < 2 )
				return text;

			char c = text[0];
			if( (c >= 'a') && (c <= 'z') )
				return ((char)(c - 32)).ToString() + text.Substring(1);
			else
				return text;
		}

	}


	
}
