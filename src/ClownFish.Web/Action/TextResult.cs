using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ClownFish.Web
{
	/// <summary>
	/// 表示一个纯文本Action的返回结果
	/// </summary>
	public sealed class TextResult : IActionResult
	{
		/// <summary>
		/// 需要输出的数据对象
		/// </summary>
		public object Model { get; private set; }
		/// <summary>
		/// 需要设置的 ContentType 标头
		/// </summary>
		public string ContentType { get; private set; }

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="model">需要输出的数据对象</param>
		public TextResult(object model) : this(model, "text/plain")
		{
		}
		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="model">需要输出的数据对象</param>
		/// <param name="contentType">需要设置的 ContentType 标头</param>
		public TextResult(object model, string contentType)
		{
			if( model == null )
				throw new ArgumentNullException("model");
			if( string.IsNullOrEmpty(contentType) )
				throw new ArgumentNullException("contentType");

			this.Model = model;
			this.ContentType = contentType;
		}

		void IActionResult.Ouput(HttpContext context)
		{
			context.Response.ContentType = this.ContentType;
			context.Response.Write(this.Model.ToString());
		}
	}
}
