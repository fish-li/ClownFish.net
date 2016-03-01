using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClownFish.Web.Debug404
{
	/// <summary>
	/// 表示一个404错误发生时测试结果
	/// </summary>
	public sealed class TestResult
	{
		/// <summary>
		/// 描述
		/// </summary>
		public string Text { get; set; }
		/// <summary>
		/// 是否测试通过
		/// </summary>
		public bool IsPass { get; set; }
		/// <summary>
		/// 测试不通过的原因
		/// </summary>
		public string Reason { get; set; }
	}
}
