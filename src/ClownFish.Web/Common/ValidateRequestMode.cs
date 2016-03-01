using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Web
{
	/// <summary>
	/// 定义针对请求的安全检查模式
	/// </summary>
	public enum ValidateRequestMode
	{
		/// <summary>
		/// 从web.config中继承设置
		/// </summary>
		Inherits,
		/// <summary>
		/// 打开安全检查
		/// </summary>
		Enable,
		/// <summary>
		/// 关闭安全检查
		/// </summary>
		Disable
	}

}
