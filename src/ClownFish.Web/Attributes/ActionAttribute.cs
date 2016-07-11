using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClownFish.Base;
using ClownFish.Base.Framework;
using ClownFish.Base.Http;
using ClownFish.Web;
using ClownFish.Web.Reflection;


namespace ClownFish.Web
{
	
	/// <summary>
	/// 将一个方法标记为一个Action
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class ActionAttribute : Attribute
	{
		/// <summary>
		/// 允许哪些访问动词，与web.config中的httpHanlder的配置意义一致。
		/// </summary>
		public string Verb { get; set; }

		private string[] _verbArray;


		/// <summary>
		/// 确定 ASP.NET 是否针对危险值检查来自浏览器的输入。
		/// </summary>
		public ValidateRequestMode ValidateRequest { get; set; }

		/// <summary>
		/// Action结果的序列化方式
		/// </summary>
		public SerializeFormat OutFormat { get; set; }
		

		internal bool AllowExecute(string httpMethod)
		{
			if( string.IsNullOrEmpty(Verb) || Verb == "*" ) {
				return true;
			}
			else {
				if( _verbArray == null )
					_verbArray = Verb.SplitTrim(StringExtensions.CommaSeparatorArray);

				return _verbArray.Contains(httpMethod, StringComparer.OrdinalIgnoreCase);
			}
		}

		internal bool NeedValidateRequest()
		{
			if( this.ValidateRequest == ValidateRequestMode.Enable )
				return true;

			if( this.ValidateRequest == ValidateRequestMode.Disable )
				return false;

			return WebConfig.ValidateRequest;
		}
	}







	
}
