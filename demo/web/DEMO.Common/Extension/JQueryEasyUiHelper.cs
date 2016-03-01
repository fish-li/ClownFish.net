using System;
using System.Web;
using System.Text;
using ClownFish;

namespace DEMO.Common
{
	/// <summary>
	///JQueryEasyUiHelper 的摘要说明
	/// </summary>
	public static class JQueryEasyUiHelper
	{

		/// <summary>
		/// 根据分页参数生成与JQuery Easy UI 兼容的分页条代码
		/// </summary>
		/// <param name="pagingInfo">分页参数</param>
		/// <param name="urlParamName">URL中的分页序号参数</param>		
		/// <param name="colspan">td元素要跨越的列数</param>
		/// <returns>分页条代码</returns>
		public static string ToHtml(this PagingInfo pagingInfo, string urlParamName, int colspan)
		{
			string bar = ToHtml(pagingInfo, urlParamName);
			if( string.IsNullOrEmpty(bar) )
				return bar;


			string html = string.Concat(
				string.Format("<tfoot><tr class='GridView_FooterStyle'><td colspan='{0}'>", colspan),
				bar.ToString(),
				"</td></tr></tfoot>");
			return html;
		}



		/// <summary>
		/// 根据分页参数生成与JQuery Easy UI 兼容的分页条代码
		/// </summary>
		/// <param name="pagingInfo">分页参数</param>
		/// <param name="urlParamName">URL中的分页序号参数</param>
		/// <returns>分页条代码</returns>
		public static string ToHtml(this PagingInfo pagingInfo, string urlParamName)
		{
			if( pagingInfo == null )
				throw new ArgumentNullException("pagingInfo");

			if( pagingInfo.PageSize == 0 )
				throw new ArgumentOutOfRangeException("PagingInfo.PageSize is zero.");

			if( string.IsNullOrEmpty(urlParamName) )
				throw new ArgumentNullException("urlParamName");

			HttpContext context = HttpContext.Current;
			if( context == null )
				throw new InvalidProgramException();

			string requestRawUrl = context.Request.RawUrl;


			int pageCount = (int)Math.Ceiling((double)pagingInfo.TotalRecords / (double)pagingInfo.PageSize);

			if( pageCount < 1 )
				return string.Empty;



			StringBuilder sb = new StringBuilder();
			sb.AppendLine("<div class='pagination'>");
			sb.AppendLine("<table cellpadding='0' cellspacing='0' border='0'><tr>");
			sb.AppendLine("<td>{labFirst}</td>");
			sb.AppendLine("<td>{labPrev}</td>");
			sb.AppendLine("<td><div class='pagination-btn-separator'></div></td>");
			sb.AppendLine("<td>&nbsp;页&nbsp;</td>");
			sb.AppendLine("<td>{txtCurrentPage}</td>");
			sb.AppendLine("<td>&nbsp;/{labPageCount}&nbsp;</td>");
			sb.AppendLine("<td><div class='pagination-btn-separator'></div></td>");
			sb.AppendLine("<td>{labNext}</td>");
			sb.AppendLine("<td>{labLast}</td>");
			sb.AppendLine("<td><div class='pagination-btn-separator'></div></td>");
			sb.AppendLine("<td>{labRefresh}</td>");
			sb.AppendLine("</tr></table>");
			sb.AppendLine("<div class='pagination-info'>{labInfo}</div>");
			sb.AppendLine("{script}");
			sb.AppendLine("<div style='clear: both;'></div></div>");

			bool firstEnabled = (pagingInfo.PageIndex > 0);
			bool lastEnabled = (pagingInfo.PageIndex < pageCount - 1);


			MyUrlGenerator generate = new MyUrlGenerator(requestRawUrl);
			generate.Remove("_");	// "_" 是 jquery 加上去的。
			string linkFirst = generate.GetNewUrl(urlParamName, "1");
			string linkLast = generate.GetNewUrl(urlParamName, pageCount.ToString());
			string linkPrevious = generate.GetNewUrl(urlParamName, pagingInfo.PageIndex.ToString());
			string linkNext = generate.GetNewUrl(urlParamName, (pagingInfo.PageIndex + 2).ToString());

			string ctlId = "txt" + Guid.NewGuid().ToString("N");
			string txtCurrentPage = string.Format("<input type='text' class='pageNumber' value=\"{0}\" baseUrl=\"{1}\" max=\"{2}\" autoRedire='true' param=\"{3}\" id='{4}' />",
				(pagingInfo.PageIndex + 1).ToString(),
				HttpUtility.HtmlAttributeEncode(generate.GetNewUrl(null, null, urlParamName)),
				pageCount.ToString(),
				HttpUtility.HtmlAttributeEncode(urlParamName),
				ctlId);

			string script = string.Format("<script type=\"text/javascript\">$(function(){{ SetPageNumberTextbox('{0}'); }});</script>", ctlId);

			string labFirst = GenerateButton("pagination-first", linkFirst, null, firstEnabled, "转到第一页");
			string labPrev = GenerateButton("pagination-prev", linkPrevious, null, firstEnabled, "转到前一页");
			string labNext = GenerateButton("pagination-next", linkNext, null, lastEnabled, "转到后一页");
			string labLast = GenerateButton("pagination-last", linkLast, null, lastEnabled, "转到最后一页");
			string labPageCount = pageCount.ToString();
			string labRefresh = GenerateButton("pagination-load", requestRawUrl, "刷新", true, "刷新当前页");
			int endRecord = ((pagingInfo.PageIndex + 1) * pagingInfo.PageSize);
			string labInfo = string.Format("显示范围：{0} 到 {1} ，  共 {2} 条记录",
				(pagingInfo.PageIndex * pagingInfo.PageSize + 1), ((endRecord < pagingInfo.TotalRecords) ? endRecord : pagingInfo.TotalRecords), pagingInfo.TotalRecords);

			sb.Replace("{labFirst}", labFirst).Replace("{labPrev}", labPrev).Replace("{txtCurrentPage}", txtCurrentPage);
			sb.Replace("{labPageCount}", labPageCount).Replace("{labNext}", labNext).Replace("{labLast}", labLast);
			sb.Replace("{labRefresh}", labRefresh).Replace("{labInfo}", labInfo).Replace("{script}", script);

			return sb.ToString();
		}

		private static string GenerateButton(string icon, string href, string text, bool enabled, string tipText)
		{
			if( enabled == false ) {
				href = "javascript:;";
				tipText = string.Empty;
			}

			if( string.IsNullOrEmpty(text) )
				return string.Format(string.Concat(
					"<a icon=\"{0}\" href=\"{1}\" class='l-btn l-btn-plain {2}' title=\"{3}\" autoRedire=\"{4}\">",
					"<span class='l-btn-left'><span class='l-btn-text'>",
					"<span class='l-btn-empty {0}'>&nbsp;</span></span></span></a>"),
					icon, href, (enabled ? string.Empty : "l-btn-disabled"), 
					tipText, enabled.ToString().ToLower());

			else
				return string.Format(string.Concat(
					"<a icon=\"{0}\" href=\"{1}\" class='l-btn l-btn-plain' style='float: left;' title=\"{3}\" autoRedire=\"{4}\">",
					"<span class='l-btn-left'>",
					"<span class='l-btn-text {0}' style='padding-left: 20px;'>{2}</span></span></a>"),
					icon, href, text, tipText, enabled.ToString().ToLower());
		}

	}
}