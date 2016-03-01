using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using ClownFish.Base.Common;
using ClownFish.Web;

namespace ClownFish.Web.Serializer
{
	/// <summary>
	/// 用于创建Action参数提供者的工厂
	/// </summary>
	public class ActionParametersProviderFactory
	{
		//说明：
		//这个工厂不想设计成字典形式（根据 ContentType 来查找匹配的IActionParametersProvider类型），
		//是因为 ContentType 就这么几种，不可能会有较多变化，
		//所以，直接 3 个 if　应该可以满足绝大多数的使用场景，它比字典查找要轻量很多，
		
		//如果对于某种　ContentType 的　Provider　不能满足需求，可以重写对应的虚方法来重写，
		//如果这里所列表的　ContentType　的类型确实也不能满足需求，还可以重写CreateProvider


		// 这个工厂没有数据成员，每次都是返回新实例，因此没有线程安全问题，
		// 所以这里采用单例模式保存 ActionParametersProviderFactory 或者它的继承类型的实例

		private static readonly LazyObject<ActionParametersProviderFactory> s_instance
											= new LazyObject<ActionParametersProviderFactory>();

		/// <summary>
		/// ActionParametersProviderFactory的实例
		/// </summary>
		internal static ActionParametersProviderFactory Instance
		{
			get { return s_instance.Instance; }
		}



		/// <summary>
		/// 根据HttpContext创建一个匹配的IActionParametersProvider实例
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public virtual IActionParametersProvider CreateProvider(HttpContext context)
		{
			// 扩展点：允许自定义 IActionParametersProvider从HTTP请求中解析数据

			if( context == null )
				throw new ArgumentNullException("context");


			string contentType = context.Request.ContentType;

			if( string.IsNullOrEmpty(contentType)
				|| contentType.IndexOfIgnoreCase("application/x-www-form-urlencoded") >= 0 )
				return CreateFormProvider(context);

			if( contentType.IndexOfIgnoreCase("application/json") >= 0 ) 
				return CreateJsonProvider(context);


			if( contentType.IndexOfIgnoreCase("application/xml") >= 0 )
				return CreateXmlProvider(context);


			IActionParametersProvider provider = CreateCustomizeProvider(context);
			if( provider != null )
				return provider;


			// 默认还是表单的 key = vlaue格式。
			// "multipart/form-data", "text/plain" 也会到这里
			return CreateDefaultProvider(context);
		}

		
		/// <summary>
		/// 获取自定义的IActionParametersProvider实现
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		protected virtual IActionParametersProvider CreateCustomizeProvider(HttpContext context)
		{
			// 默认不返回结果，
			// 如果需要支持FORM, JSON, XML之外的数据格式，可以重写这个方法。

			//例如：
			//if( contentType.IndexOfIgnoreCase("application/bin") >= 0 )
			//	return XXXXXXXXXXXXXXXX();

			return null;
		}


		/// <summary>
		/// 创建一个可用于解析 FORM表单 的ActionParametersProvider
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		protected virtual IActionParametersProvider CreateFormProvider(HttpContext context)
		{
			// 扩展点：允许替换框架的默认实现

			return new FormDataProvider();
		}
		/// <summary>
		/// 当FORM, JSON, XML判断失败时，创建 默认 的ActionParametersProvider
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		protected virtual IActionParametersProvider CreateDefaultProvider(HttpContext context)
		{
			// 扩展点：允许替换框架的默认实现

			return new FormDataProvider();
		}

		/// <summary>
		/// 创建一个可用于解析 JSON 的ActionParametersProvider
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		protected virtual IActionParametersProvider CreateJsonProvider(HttpContext context)
		{
			// 扩展点：允许替换框架的默认实现

			return new JsonnetDataProvider();
			//return new JsonDataProvider();
		}

		/// <summary>
		/// 创建一个可用于解析 XML 的ActionParametersProvider
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		protected virtual IActionParametersProvider CreateXmlProvider(HttpContext context)
		{
			// 扩展点：允许替换框架的默认实现

			return new XmlDataProvider();
		}
	}
}
