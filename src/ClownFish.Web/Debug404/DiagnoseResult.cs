using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ClownFish.Web.Debug404
{
	/// <summary>
	/// 404错误的诊断结果
	/// </summary>
	public sealed class DiagnoseResult
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		public DiagnoseResult()
		{
			ErrorMessages = new List<string>();
		}
		/// <summary>
		/// 所有错误消息描述。
		/// </summary>
		public List<string> ErrorMessages { get; set; }

		/// <summary>
		/// 从当前请求URL解析出来的UrlActionInfo实例
		/// </summary>
		public UrlActionInfo UrlActionInfo { get; set; }
		/// <summary>
		/// 所有包含Action的程序集列表
		/// </summary>
		public List<string> AssemblyList { get; set; }

		/// <summary>
		/// 所有PageUrlAttribute的测试结果
		/// </summary>
		public List<TestResult> PageUrlTestResult { get; set; }
		/// <summary>
		/// 所有PageRegexUrlAttribute的测试结果
		/// </summary>
		public List<TestResult> PageRegexUrlTestResult { get; set; }


		/// <summary>
		/// 所有NamespaceMapAttribute的查找结果
		/// </summary>
		public List<TestResult> NamespaceMapTestResult { get; set; }
		/// <summary>
		/// 所有路由记录的测试结果
		/// </summary>
		public List<TestResult> RouteTestResult { get; set; }
		/// <summary>
		/// 所有Controller类型的匹配结果
		/// </summary>
		public List<TestResult> ControllerTestResult { get; set; }
		/// <summary>
		/// 可以匹配的Controller类型
		/// </summary>
		public string ControllerType { get; set; }
		/// <summary>
		/// 匹配Controller类型的所有Action测试结果
		/// </summary>
		public List<TestResult> ActionTestResult { get; set; }


	}
}
