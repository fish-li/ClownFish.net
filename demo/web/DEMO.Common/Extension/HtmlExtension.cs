using System;
using System.Collections.Generic;
using System.Text;
using ClownFish.Web;
using System.IO;
using ClownFish;
using System.Web;


namespace DEMO.Common
{

	public static class HtmlExtension
	{
		public static string ToHtml(this HtmlOptionItem item)
		{
			if( item == null )
				return string.Empty;

			return item.Text.ToHtmlOption(item.Value, item.Selected);
		}

		public static string ToHtmlOption(this string text)
		{
			return ToHtmlOption(text, text, false);
		}

		public static string ToHtmlOption(this string text, bool selected)
		{
			return ToHtmlOption(text, text, selected);
		}

		public static string ToHtmlOption(this string text, string value)
		{
			return ToHtmlOption(text, value, false);
		}

		public static string ToHtmlOption(this string text, string value, bool selected)
		{
			return string.Format("<option value=\"{0}\" {1}>{2}</option>\r\n", value.HtmlAttributeEncode(), (selected ? "selected='selected'" : string.Empty), text.HtmlEncode());
		}


		public static string ToHtml(this List<HtmlOptionItem> list)
		{
			if( list == null || list.Count == 0 )
				return string.Empty;


			StringBuilder sb = new StringBuilder();
			foreach( var item in list )
				sb.Append(item.ToHtml());

			return sb.ToString();
		}

		public static string ToCheckBox(this bool value, string id, string name, bool disabled)
		{
			return string.Format("<input type='checkbox' {0} {1} {2} {3} />",
				(id == null ? string.Empty : string.Format("id=\"{0}\"", id.HtmlAttributeEncode())),
				(name == null ? string.Empty : string.Format("name=\"{0}\"", name.HtmlAttributeEncode())),
				(value ? "checked='checked'" : string.Empty),
				(disabled ? "disabled='disabled'" : string.Empty)
				);
		}

		public static string PaginationBar(this PagingInfo pagingInfo, int colspan)
		{
			return pagingInfo.ToHtml("page", colspan);
		}

		public static string PaginationBar(this PagingInfo pagingInfo)
		{
			return pagingInfo.ToHtml("page");
		}

		/// <summary>
		/// 让某个下拉框拥有选择后自动刷新功能，这要求每个option的value是一个URL
		/// </summary>
		public static string DropDownListAutoRedir(string ctlId)
		{
			return string.Format(@"javascript:setTimeout('window.location.href = document.getElementById(\'{0}\').value', 0)", ctlId);
		}


	}
}