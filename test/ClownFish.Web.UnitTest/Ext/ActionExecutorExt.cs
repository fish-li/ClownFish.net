using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.TypeExtend;

namespace ClownFish.Web.UnitTest.Ext
{
	public class ActionExecutorExt : EventSubscriber<ActionExecutor>
	{
		// 定义成静态变量是为了便于在外面获取数据
		public static List<string> EventMessages = new List<string>();

		public override void SubscribeEvent(ActionExecutor instance)
		{
			EventMessages.Clear();

			instance.BeginRequest += instance_OnBeginRequest;
			instance.BeforeExecuteAction += instance_BeforeExecuteAction;
			instance.AfterExecuteAction += instance_AfterExecuteAction;
			instance.AuthorizeRequest += instance_OnAuthorizeRequest;
			instance.CorsCheck += instance_OnCorsCheck;
			instance.EndObtainParameters += instance_EndObtainParameters;
			instance.ProcessResult += instance_OnOutputResult;
		}

		void instance_OnOutputResult(object sender, ProcessResultEventArgs e)
		{
			EventMessages.Add("ProcessResult");
		}

		void instance_EndObtainParameters(object sender, EndObtainParametersEventArgs e)
		{
			EventMessages.Add("EndObtainParameters");
		}

		void instance_OnCorsCheck(object sender, CorsCheckEventArgs e)
		{
			EventMessages.Add("CorsCheck");
		}

		void instance_OnBeginRequest(object sender, ActionEventArgs e)
		{
			EventMessages.Add("OnBeginRequest");
		}

		void instance_OnAuthorizeRequest(object sender, AuthorizeCheckEventArgs e)
		{
			EventMessages.Add("AuthorizeRequest");
		}

		void instance_AfterExecuteAction(object sender, AfterExecuteActionEventArgs e)
		{
			EventMessages.Add("AfterExecuteAction");
		}

		void instance_BeforeExecuteAction(object sender, BeforeExecuteActionEventArgs e)
		{
			EventMessages.Add("BeforeExecuteAction");
		}
	}
}
