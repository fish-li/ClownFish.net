using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;
using ClownFish.Base.TypeExtend;
using ClownFish.Web.Reflection;


namespace ClownFish.Web
{
	internal class RequiresSessionActionHandler : ActionHandler, IRequiresSessionState
	{
	}

	internal class ReadOnlySessionActionHandler : ActionHandler, IRequiresSessionState, IReadOnlySessionState
	{
	}

	/// <summary>
	/// 用于同步操作的 HttpHandler
	/// </summary>
	internal class ActionHandler : IHttpHandler
	{
		internal InvokeInfo InvokeInfo { get; private set; }

		internal ActionExecutor ActionExecutor { get; private set; }

		public void ProcessRequest(HttpContext context)
		{
			// 如果修改了这里的逻辑，请继续修改 TaskAsyncActionHandler，那是一个异步版本的实现！

			this.ActionExecutor = ObjectFactory.New<ActionExecutor>();

			try {
				this.ActionExecutor.ProcessRequest(context, this);
			}
			catch( Exception ex ) {
				if( this.ActionExecutor.ProcessException(ex) == false )
					throw;
			}
			finally {
				this.ActionExecutor.ExecuteEndRequest();
			}
		}

		public bool IsReusable
		{
			get { return false; }
		}

		public static IHttpHandler CreateHandler(InvokeInfo vkInfo)
		{
			SessionMode mode = vkInfo.GetSessionMode();
			
			if( mode == SessionMode.NotSupport )
				return new ActionHandler { InvokeInfo = vkInfo };

			else if( mode == SessionMode.ReadOnly )
				return new ReadOnlySessionActionHandler { InvokeInfo = vkInfo };

			else
				return new RequiresSessionActionHandler { InvokeInfo = vkInfo };
		}
		
	}
}
