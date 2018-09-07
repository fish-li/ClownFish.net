using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using ClownFish.Base.Http;
using ClownFish.Base.Xml;
using ClownFish.Web;

namespace ClownFish.Web
{
	/// <summary>
	/// 表示Action的执行结果为XML
	/// </summary>
	public sealed class XmlResult : IActionResult
	{
		/// <summary>
		/// 需要以XML形式输出的数据对象
		/// </summary>
		public object Model { get; private set; }

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="model">需要以XML形式输出的数据对象</param>
		public XmlResult(object model)
		{
			if( model == null )
				throw new ArgumentNullException("model");

			this.Model = model;
		}

		void IActionResult.Ouput(HttpContext context)
		{
			context.Response.ContentType = ResponseContentType.Xml;
			string xml = XmlHelper.XmlSerialize(Model, Encoding.UTF8);
			context.Response.Write(xml);
		}
	}
}
