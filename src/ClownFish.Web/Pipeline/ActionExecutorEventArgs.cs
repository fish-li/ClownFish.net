using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ClownFish.Web
{

	/// <summary>
	/// ActionExecutor中所有事件参数的基类
	/// </summary>
	public class ActionEventArgs : System.EventArgs
	{
		/// <summary>
		/// HttpContext实例
		/// </summary>
		public HttpContext HttpContext { get; internal set; }
		/// <summary>
		/// Controller实例
		/// </summary>
		public object ControllerInstance { get; internal set; }
		/// <summary>
		/// Action方法的反射信息（MethodInfo实例）
		/// </summary>
		public MethodInfo ActionMethod { get; internal set; }
	}

	
	/// <summary>
	/// 表示进入CorsCheck阶段的事件参数类型
	/// </summary>
	public sealed class CorsCheckEventArgs : ActionEventArgs
	{
		/// <summary>
		/// Origin请求头信息
		/// </summary>
		public string Origin { get; internal set; }

		/// <summary>
		/// 是否禁止访问，如果在事件中设置为 true ，将禁止本次AJAX的跨域访问。
		/// </summary>
		public bool IsForbidden { get; set; }
	}

	/// <summary>
	/// 表示进入AuthorizeCheck阶段的事件参数类型
	/// </summary>
	public sealed class AuthorizeCheckEventArgs : ActionEventArgs
	{
		/// <summary>
		/// Action方法上的AuthorizeAttribute实例
		/// </summary>
		public AuthorizeAttribute Attribute { get; internal set; }
	}

	/// <summary>
	/// 表示进入EndObtainParameters阶段的事件参数类型
	/// </summary>
	public sealed class EndObtainParametersEventArgs : ActionEventArgs
	{
		/// <summary>
		/// 获取到的参数值数组，可以在事件中修改数组中的元素。
		/// </summary>
		public object[] Parameters { get; internal set; }
	}


	/// <summary>
	/// 表示进入 BeforeExceute  阶段的事件参数类型
	/// </summary>
	public sealed class BeforeExecuteActionEventArgs : ActionEventArgs
	{
		/// <summary>
		/// 获取到的参数值数组
		/// </summary>
		public object[] Parameters { get; internal set; }
	}


	/// <summary>
	/// 表示进入 AfterExecute 阶段的事件参数类型
	/// </summary>
	public sealed class AfterExecuteActionEventArgs : ActionEventArgs
	{
		/// <summary>
		/// 获取到的参数值数组
		/// </summary>
		public object[] Parameters { get; internal set; }
		/// <summary>
		/// 执行结果，如果发生异常，此属性为 null
		/// </summary>
		public object ExecuteResult { get; internal set; }

		/// <summary>
		/// 执行过程中发生的异常，如果没有异常发生，此属性保持为 null
		/// </summary>
		public Exception Exception { get; internal set; }
	}

	/// <summary>
	/// 表示进入结果输出阶段的事件参数类型
	/// </summary>
	public sealed class ProcessResultEventArgs : ActionEventArgs
	{
		/// <summary>
		/// Action的执行结果
		/// 可以在OnOutputResult事件中执行自特定的输出逻辑，并设置ResultHandled=true，来跳过框架的输出过程
		/// </summary>
		public object ExecuteResult { get; internal set; }

		/// <summary>
		/// 表示执行结果已被输出处理，不需要再执行输出过程。
		/// </summary>
		public bool ResultHandled { get; set; }
	}

	/// <summary>
	/// Action 执行过程中发生异常时的事件参数
	/// </summary>
	public sealed class ActionExceptionEventArgs : ActionEventArgs
	{
		/// <summary>
		/// Exception 实例
		/// </summary>
		public Exception Exception { get; internal set; }
		/// <summary>
		/// 可设置为TRUE，表示异常已被处理，将会停止异常传播。
		/// </summary>
		public bool ExceptionHandled { get; set; }

	}
}
