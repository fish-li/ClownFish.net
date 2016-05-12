using System;

namespace DEMO.Common
{

	/// <summary>
	/// 表示创建一个 html 的 option 所需的数据
	/// </summary>
	public sealed class HtmlOptionItem
	{
		/// <summary>
		/// Text
		/// </summary>
		public string Text { get; set; }
		/// <summary>
		/// Value
		/// </summary>
		public string Value { get; set; }
		/// <summary>
		/// Selected
		/// </summary>
		public bool Selected { get; set; }

	}
}