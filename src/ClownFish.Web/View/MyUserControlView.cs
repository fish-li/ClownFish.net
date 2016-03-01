using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;


namespace ClownFish.Web
{
	/// <summary>
	/// 基于用户控件的视图基类，这类用户控件只是用于呈现数据。
	/// </summary>
	/// <typeparam name="TModel">传递给用户控件呈现时所需的数据实体对象类型</typeparam>
	public class MyUserControlView<TModel> : MyBaseUserControl
	{
		/// <summary>
		/// 用于页面呈现时所需的数据实体对象
		/// </summary>
		public TModel Model { get; private set; }

		/// <summary>
		/// 为当前实例指定可绑定的数据对象
		/// </summary>
		/// <param name="model">可绑定的数据对象</param>
		public override void SetModel(object model)
		{
			try {
				this.Model = (TModel)model;
			}
			catch( Exception ex ) {
				throw new ArgumentException("参数model与目标类型不匹配。", ex);
			}
		}
	}



}
