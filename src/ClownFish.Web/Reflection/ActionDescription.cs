using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using ClownFish.Base.Common;
using ClownFish.Base.Reflection;

namespace ClownFish.Web.Reflection
{
	/// <summary>
	/// 用于描述【能从URL中提取Controller，Action】的Action
	/// </summary>
	internal sealed class ActionDescription : BaseDescription
	{
		// 创建一个ActionProcessor的实例，用于返回默认的ActionAttriubte实例
		private static LazyObject<ActionHelper> s_actionProcessorInstance = new LazyObject<ActionHelper>();

		public ControllerDescription PageController { get; set; } //为PageAction保留
		public MethodInfo MethodInfo { get; private set; }
		public ActionAttribute Attr { get; private set; }
		public ParameterInfo[] Parameters { get; private set; }
		public bool HasReturn { get; private set; }


		public ActionDescription(MethodInfo m)
			: base(m)
		{
			this.MethodInfo = m;			
			this.Parameters = m.GetParameters();

			ActionAttribute attr = m.GetMyAttribute<ActionAttribute>(false);	// ?? new ActionAttribute();
			if( attr == null ) 
				attr = s_actionProcessorInstance.Instance.CreateDefaultActionAttribute();

			this.Attr = attr;
			

			if( m.IsTaskMethod() )
				this.HasReturn = m.GetTaskMethodResultType() != null;
			else
				this.HasReturn = m.ReturnType != ReflectionHelper.VoidType;
		}
	}

	/// <summary>
	/// 用于描述【正则表达式URL】的Action
	/// </summary>
	internal sealed class RegexActionDescription
	{
		public Regex Regex { get; set; }

		public ActionDescription ActionDescription { get; set; }
	}


	/// <summary>
	/// 用于描述【与固定URL匹配】的Action
	/// </summary>
	internal sealed class MatchActionDescription
	{
		public Match Match { get; set; }

		public ActionDescription ActionDescription { get; set; }
	}


}
