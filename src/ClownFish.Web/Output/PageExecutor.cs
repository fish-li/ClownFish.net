using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Compilation;
using System.Web.UI;
using ClownFish.Base.TypeExtend;
using ClownFish.Web.Reflection;


namespace ClownFish.Web
{
	/// <summary>
	/// 用指定的页面路径以及视图数据呈现结果，最后返回生成的HTML代码。
	/// 页面应从MyPageView&lt;T&gt;继承
	/// </summary>
	public class PageExecutor
	{
		internal static readonly Type MyPageViewOpenType = typeof(MyPageView<>);

		/// <summary>
		/// 用指定的页面路径以及视图数据呈现结果，最后返回生成的HTML代码。
		/// </summary>
		/// <param name="context">HttpContext对象</param>
		/// <param name="pageVirtualPath">Page的虚拟路径</param>
		/// <param name="model">视图数据</param>
		/// <returns>生成的HTML代码</returns>
		public static string Render(HttpContext context, string pageVirtualPath, object model)
		{
			PageExecutor executor = ObjectFactory.New<PageExecutor>();
			return executor.RenderPage(context, pageVirtualPath, model);
		}

		/// <summary>
		/// HttpContext实例引入
		/// </summary>
		protected HttpContext HttpContext { get; private set; }
		/// <summary>
		/// 页面的虚拟路径
		/// </summary>
		protected string PageVirtualPath { get; private set; }
		/// <summary>
		/// 要绑定的数据对象
		/// </summary>
		protected object Model { get; private set; }
		/// <summary>
		/// 已加载的页面实例
		/// </summary>
		protected Page Handler { get; private set; }

		/// <summary>
		/// 用指定的页面路径以及视图数据呈现结果，最后返回生成的HTML代码。
		/// 页面应从MyPageView&lt;T&gt;继承
		/// </summary>
		/// <param name="context">HttpContext对象</param>
		/// <param name="pageVirtualPath">Page的虚拟路径</param>
		/// <param name="model">视图数据</param>
		/// <returns>生成的HTML代码</returns>
		public virtual string RenderPage(HttpContext context, string pageVirtualPath, object model)
		{
			// 扩展点：实现模板替换（页面路由替换），重写方法，修改pageVirtualPath参数

			this.HttpContext = context;
			this.PageVirtualPath = pageVirtualPath;
			this.Model = model;


			this.Handler = GetHandler();

			if( this.Handler == null )
				return null;

			BindModel();

			

			// 捕获页面执行过程中发生的异常
			this.Handler.Error += new EventHandler(HandlerError);

			TextWriter output = GetTextWriter();
			try {
				BeforeExecute();

				this.HttpContext.Server.Execute(this.Handler, output, false);
			}
			catch( HttpException ) {
				// 检查页面执行过程中是否发生过异常。
				Exception ee = GetLastExceptoin();
				if( ee == null )
					throw;
				else
					throw new HttpException(500, ee.Message, ee);
			}
			finally {
				AfterExecute();
			}
			
			return GetWriteText(output);
		}

		/// <summary>
		/// 获取TextWriter实例
		/// </summary>
		/// <returns></returns>
		protected virtual TextWriter GetTextWriter()
		{
			// 扩展点：允许替换默认实现方式

			return new StringWriter();
		}

		/// <summary>
		/// 获取页面渲染的HTML代码
		/// </summary>
		/// <param name="writer"></param>
		/// <returns></returns>
		protected virtual string GetWriteText(TextWriter writer)
		{
			// 扩展点：允许替换默认实现方式

			return writer.ToString();
		}
		/// <summary>
		/// 加载页面，以Page形式返回结果
		/// </summary>
		/// <returns></returns>
		protected virtual Page GetHandler()
		{
			// 扩展点：如果需要实现页面替换逻辑，例如个性化页面覆盖标准产品页面，可以重写这个方法

			Page handler = WebRuntime.Instance.CreateInstanceFromVirtualPath(this.PageVirtualPath);

			if( handler == null )
				throw new InvalidOperationException(
					string.Format("指定的路径 {0} 不是一个有效的页面。", this.PageVirtualPath));

			return handler;
		}
		/// <summary>
		/// 绑定数据对象
		/// </summary>
		protected virtual void BindModel()
		{
			// 扩展点：允许替换默认实现方式

			if( this.Model != null ) {
				MyBasePage page = this.Handler as MyBasePage;
				if( page != null )
					page.SetModel(this.Model);
			}
		}
		/// <summary>
		/// 执行渲染【前】方法
		/// </summary>
		protected virtual void BeforeExecute() 
		{
			// 扩展点： 可以动态加载用户控件，实现页面注入，
		}

		/// <summary>
		/// 执行渲染【后】方法
		/// </summary>
		protected virtual void AfterExecute() { }
		
		private static readonly string STR_PageExecuteExceptionKey = "ClownFish.Web__Page_Execute_Exception";
		/// <summary>
		/// 处理已发生的异常
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void HandlerError(object sender, EventArgs e)
		{
			// 扩展点：允许替换默认实现方式，修改异常的处理方式

			Page handler = (Page)sender;
			HttpContext context = handler.GetType().InvokeMember("Context",
				System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Instance
				| System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public,
				null, handler, null) as HttpContext;

			if( context != null )
				// 临时将异常保存到context
				context.Items[STR_PageExecuteExceptionKey] = context.Error;
		}
		/// <summary>
		/// 获取最近发生的异常信息
		/// </summary>
		/// <returns></returns>
		protected virtual Exception GetLastExceptoin()
		{
			// 扩展点：允许替换默认实现方式，修改异常的处理方式

			return this.HttpContext.Items[STR_PageExecuteExceptionKey] as Exception;
		}

	}
}
