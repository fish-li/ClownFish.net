using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;
using ClownFish.Base.TypeExtend;
using ClownFish.Web.Reflection;

namespace ClownFish.Web
{

	internal class TaskAsyncRequiresSessionActionHandler : TaskAsyncActionHandler, IRequiresSessionState
	{
	}

	internal class TaskAsyncReadOnlySessionActionHandler : TaskAsyncActionHandler, IRequiresSessionState, IReadOnlySessionState
	{
	}

	/// <summary>
	/// 支持异步的 HttpHandler
	/// </summary>
	internal class TaskAsyncActionHandler : HttpTaskAsyncHandler
	{
		internal InvokeInfo InvokeInfo { get; private set; }

		internal ActionExecutor ActionExecutor { get; private set; }


		public override bool IsReusable
		{
			get { return false; }
		}


		public static IHttpHandler CreateHandler(InvokeInfo vkInfo)
		{
			SessionMode mode = vkInfo.GetSessionMode();

			if( mode == SessionMode.NotSupport )
				return new TaskAsyncActionHandler { InvokeInfo = vkInfo };

			else if( mode == SessionMode.ReadOnly )
				return new TaskAsyncReadOnlySessionActionHandler { InvokeInfo = vkInfo };

			else
				return new TaskAsyncRequiresSessionActionHandler { InvokeInfo = vkInfo };
		}

		
		public async override Task ProcessRequestAsync(HttpContext context)
		{
			// 如果修改了这里的逻辑，请继续修改 ActionHandler，那是一个同步版本的实现！

			this.ActionExecutor = ObjectFactory.New<ActionExecutor>();

			try {
				await this.ActionExecutor.ProcessRequestAsync(context, this);
			}
			catch( Exception ex ) {
				if( this.ActionExecutor.ProcessException(ex) == false )
					throw;
			}
			finally {
				this.ActionExecutor.ExecuteEndRequest();
			}
		}
	}
}
