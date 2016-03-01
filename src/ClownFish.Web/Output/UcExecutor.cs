using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web;
using System.IO;
using ClownFish.Base.TypeExtend;

namespace ClownFish.Web
{
	/// <summary>
	/// 用指定的用户控件以及视图数据呈现结果，最后返回生成的HTML代码。
	/// 用户控件应从MyUserControlView&lt;T&gt;继承
	/// </summary>
	public class UcExecutor
	{
		/// <summary>
		/// 用指定的用户控件以及视图数据呈现结果，最后返回生成的HTML代码。
		/// 用户控件应从MyUserControlView&lt;T&gt;继承
		/// </summary>
		/// <param name="ucVirtualPath">用户控件的虚拟路径</param>
		/// <param name="model">视图数据</param>
		/// <returns>生成的HTML代码</returns>
		public static string Render(string ucVirtualPath, object model)
		{
			UcExecutor executor = ObjectFactory.New<UcExecutor>();
			return executor.RenderUserControl(ucVirtualPath, model);
		}
		/// <summary>
		/// 包含用户控件的Page实例
		/// </summary>
		protected Page Page { get; private set; }
		/// <summary>
		/// 已加载的用户控件
		/// </summary>
		protected Control Control { get; private set; }
		/// <summary>
		/// 用户控件的虚拟路径
		/// </summary>
		protected string UserControlVirtualPath { get; private set; }
		/// <summary>
		/// 要绑定的数据对象
		/// </summary>
		protected object Model { get; private set; }


		/// <summary>
		/// 用指定的用户控件以及视图数据呈现结果，最后返回生成的HTML代码。
		/// 用户控件应从MyUserControlView&lt;T&gt;继承
		/// </summary>
		/// <param name="ucVirtualPath">用户控件的虚拟路径</param>
		/// <param name="model">视图数据</param>
		/// <returns>生成的HTML代码</returns>
		public virtual string RenderUserControl(string ucVirtualPath, object model)
		{
			// 扩展点：实现模板替换（页面路由替换），重写方法，修改ucVirtualPath参数

			if( string.IsNullOrEmpty(ucVirtualPath) )
				throw new ArgumentNullException("ucVirtualPath");

			this.UserControlVirtualPath = ucVirtualPath;
			this.Model = model;

			this.Page = new Page();
			this.Control = LoadControl();

			BindModel();

			// 将用户控件放在Page容器中。
			this.Page.Controls.Add(this.Control);

			TextWriter output = GetTextWriter();

			try {
				BeforeExecute();

				Execute(output);
			}
			finally {
				AfterExecute();
			}

			return GetWriteText(output);
		}

		/// <summary>
		/// 加载用户控件
		/// </summary>
		/// <returns></returns>
		protected virtual Control LoadControl()
		{
			// 扩展点：可实现动态模板加载：根据属性UserControlVirtualPath判断有没有其它的模板

			Control ctl = this.Page.LoadControl(this.UserControlVirtualPath);
			if( ctl == null )
				throw new InvalidOperationException(
					string.Format("指定的用户控件 {0} 没有找到。", this.UserControlVirtualPath));

			return ctl;
		}

		/// <summary>
		/// 绑定数据对象
		/// </summary>
		protected virtual void BindModel()
		{
			// 扩展点：允许替换默认实现方式，增加一些额外的数据绑定逻辑

			if( this.Model != null ) {
				MyBaseUserControl myctl = this.Control as MyBaseUserControl;
				if( myctl != null )
					myctl.SetModel(this.Model);
			}
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
		/// 获取用户控件渲染的HTML代码
		/// </summary>
		/// <param name="writer"></param>
		/// <returns></returns>
		protected virtual string GetWriteText(TextWriter writer)
		{
			// 扩展点：允许替换默认实现方式

			return writer.ToString();
		}

		/// <summary>
		/// 执行控件渲染过程
		/// </summary>
		/// <param name="output"></param>
		protected virtual void Execute(TextWriter output)
		{
			// 扩展点：允许替换默认实现方式，增加一些额外的HTML片段

			HtmlTextWriter write = new HtmlTextWriter(output, string.Empty);

			this.Page.RenderControl(write);

			// 用下面的方法也可以的。
			//HttpContext.Current.Server.Execute(this.Page, output, false);
		}
		/// <summary>
		/// 执行渲染【前】方法
		/// </summary>
		protected virtual void BeforeExecute() { }
		/// <summary>
		/// 执行渲染【后】方法
		/// </summary>
		protected virtual void AfterExecute() { }


	}


}
