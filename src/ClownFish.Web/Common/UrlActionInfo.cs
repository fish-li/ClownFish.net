using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using ClownFish.Web.Reflection;

namespace ClownFish.Web
{
	/// <summary>
	/// 表示从URL中提取到的 Controller，Action
	/// </summary>
	public sealed class UrlActionInfo
	{
		internal UrlActionInfo()
		{
			// 限制仅在框架内部才允许实例化。
			// 目前仅在 MvcRouteHandler 和 ServiceHandlerFactory 中创建实例。
		}

		/// <summary>
		/// Controller 名称
		/// </summary>
		public string Controller { get; internal set; }

		/// <summary>
		/// Action 名称
		/// </summary>
		public string Action { get; internal set; }





		#region 以下属性为 2.0 版本中新增

		/// <summary>
		/// UrlActionInfo的实例将会以此属性做为KEY 存放在 Httpcontext.Item 集合中。
		/// </summary>
		public static readonly string HttpContextItemKey = "ClownFish.Web-UrlActionInfo-Httpcontext-Item-Key";

		/// <summary>
		/// URL中的用于区分不同类型的附加信息，允许为空。
		/// </summary>
		public string UrlType { get; internal set; }

		/// <summary>
		/// URL中的命名空间信息，也有可能是一个命名空间的别名（RestServiceModule），允许为空
		/// </summary>
		public string Namesapce { get; internal set; }

		/// <summary>
		/// URL中的类型名称信息，是一个不完整的类型名称，需要结合Namespace属性一起计算Controller属性
		/// </summary>
		public string ClassName { get; internal set; }

		/// <summary>
		/// URL中的方法名称信息，也有可能是一个参数值（RestServiceModule），因此Action属性需要重新计算
		/// </summary>
		public string MethodName { get; internal set; }

		/// <summary>
		/// URL中的文件扩展名，允许为空。
		/// </summary>
		public string ExtName { get; internal set; }

		/// <summary>
		/// 从RouteTable获取的匹配路由模式
		/// </summary>
		public string RoutePattern { get; internal set; }



		/// <summary>
		/// 从URL中提取到的参数值
		/// </summary>
		public NameValueCollection Params { get; internal set; }



		internal ControllerDescription ControllerDescription { get; set; }
		internal ActionDescription ActionDescription { get; set; }
		


		/// <summary>
		/// 给Params集合增加元素
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public void AddParam(string name, string value)
		{
			if( Params == null )
				Params = new HttpValueCollection();

			Params.Add(name, value);
		}


		internal void SetHttpcontext(HttpContext context)
		{
			if( context == null )
				throw new ArgumentNullException("context");

			if( this.Params != null )
				((HttpValueCollection)this.Params).MakeReadOnly();

			context.Items[HttpContextItemKey] = this;
		}

		#endregion

	}

}
