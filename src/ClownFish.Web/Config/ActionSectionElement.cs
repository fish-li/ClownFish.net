using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Web.Config
{
	/// <summary>
	/// Action相关的配置节
	/// </summary>
	public sealed class ActionSectionElement : ConfigurationElement
	{
		/// <summary>
		/// JSONP的回调方法的参数名称，
		/// 如果不希望启用JSONP，请设置为 null ，
		/// 默认值："callback" （与 jQuery 保持一致）
		/// </summary>
		[ConfigurationProperty("jsonpCallback", IsRequired = false, DefaultValue = "callback")]
		public string JsonpCallback
		{
			get { return this["jsonpCallback"].ToString(); }
		}

	}
}
