using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClownFish.Web
{
	/// <summary>
	/// Action所支持的Session模式
	/// </summary>
	public enum SessionMode
	{
		/// <summary>
		/// 不支持
		/// </summary>
		NotSupport,
		/// <summary>
		/// 全支持
		/// </summary>
		Support,
		/// <summary>
		/// 仅支持读取
		/// </summary>
		ReadOnly
	}


	/// <summary>
	/// 给Action描述Session的支持模式
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
	public sealed class SessionModeAttribute : Attribute
	{
		/// <summary>
		/// 要支持的Session模式
		/// </summary>
		public SessionMode SessionMode { get; private set; }

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="mode"></param>
		public SessionModeAttribute(SessionMode mode)
		{
			this.SessionMode = mode;
		}
	}
}
