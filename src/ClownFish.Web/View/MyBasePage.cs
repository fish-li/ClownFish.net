using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.CodeDom;

namespace ClownFish.Web
{
	/// <summary>
	/// 一个基于“System.Web.UI.Page”的类
	/// </summary>
	[FileLevelControlBuilder(typeof(ViewPageControlBuilder))]
	public class MyBasePage : System.Web.UI.Page
	{
		/// <summary>
		/// 为当前页面设置可绑定的数据对象
		/// </summary>
		/// <param name="model"></param>
		public virtual void SetModel(object model)
		{
		}
	}


	internal sealed class ViewPageControlBuilder : FileLevelPageControlBuilder
	{
		public string PageBaseType
		{
			get;
			set;
		}

		public override void ProcessGeneratedCode(
			CodeCompileUnit codeCompileUnit,
			CodeTypeDeclaration baseType,
			CodeTypeDeclaration derivedType,
			CodeMemberMethod buildMethod,
			CodeMemberMethod dataBindingMethod)
		{

			// 如果分析器找到一个有效的类型，就使用它。
			if( PageBaseType != null ) {
				derivedType.BaseTypes[0] = new CodeTypeReference(PageBaseType);
			}
		}
	}



	



}
