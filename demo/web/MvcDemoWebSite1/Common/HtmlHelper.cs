using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using DEMO.Common;
using DEMO.Model;

namespace MvcDemoWebSite1.Common
{
	/// <summary>
	/// 网站相关的一些HTML扩展方法
	/// </summary>
	public static class HtmlHelper
	{
		/// <summary>
		/// 生成“商品分类”下拉框中的 option 列表
		/// </summary>
		public static string ToHtmlOptions(this List<Category> list, int currentCategoryId,
			string categoryUrlkey, bool addEmptyItem, params string[] deleteUrlKeys)
		{
			if( list == null )
				throw new ArgumentNullException("list");
			if( string.IsNullOrEmpty(categoryUrlkey) )
				throw new ArgumentNullException("categoryUrlkey");

			HttpContext context = HttpContext.Current;
			if( context == null )
				throw new InvalidProgramException();

			string requestRawUrl = context.Request.RawUrl;


			StringBuilder sb = new StringBuilder();

			MyUrlGenerator generate = new MyUrlGenerator(requestRawUrl);
			generate.Remove(categoryUrlkey).Remove(deleteUrlKeys);

			if( addEmptyItem )
				sb.Append("---请选择---".ToHtmlOption(generate.ToString()));

			foreach( DEMO.Model.Category category in list ) {
				string href = generate.GetNewUrl(categoryUrlkey, category.CategoryID.ToString());

				sb.Append(category.CategoryName.ToHtmlOption(href, (category.CategoryID == currentCategoryId)));
			}

			return sb.ToString();
		}

		private static readonly string[] UnitArray = new string[] { "个", "件", "只", "双" };

		/// <summary>
		/// 生成“商品单位”下拉框中的 option 列表
		/// </summary>
		public static string GetUnitHtmlOptions(string unit)
		{
			int exist = 0;
			StringBuilder sb = new StringBuilder();

			foreach( string str in UnitArray ) {
				if( str == unit ) {
					exist++;
					sb.Append(str.ToHtmlOption(true));
				}
				else
					sb.Append(str.ToHtmlOption());
			}

			if( exist == 0 && string.IsNullOrEmpty(unit) == false )
				sb.Append(unit.ToHtmlOption(true));

			return sb.ToString();
		}


	}
}