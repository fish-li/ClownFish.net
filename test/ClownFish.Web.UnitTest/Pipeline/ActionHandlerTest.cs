using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ClownFish.Web.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.AspnetMock;

namespace ClownFish.Web.UnitTest.Pipeline
{
	[TestClass]
	public class ActionHandlerTest
	{
		[TestMethod]
		public void Test1()
		{
			InvokeInfo invokeInfo1 = new InvokeInfo();
			IHttpHandler handler1 = ActionHandler.CreateHandler(invokeInfo1);
			Assert.AreEqual(typeof(ActionHandler), handler1.GetType());
		}

		[TestMethod]
		public void Test2()
		{
			InvokeInfo invokeInfo2 = new InvokeInfo();
			invokeInfo2.Action = new ActionDescription(typeof(ActionHandlerTest).GetMethod("Test1"));
			invokeInfo2.Action.GetType().SetValue("SessionMode", 
								invokeInfo2.Action, new SessionModeAttribute(SessionMode.Support));

			IHttpHandler handler2 = ActionHandler.CreateHandler(invokeInfo2);
			Assert.AreEqual(typeof(RequiresSessionActionHandler), handler2.GetType());
		}

		[TestMethod]
		public void Test3()
		{
			InvokeInfo invokeInfo3 = new InvokeInfo();
			invokeInfo3.Controller = new ControllerDescription(typeof(ActionHandlerTest));
			invokeInfo3.Controller.GetType().SetValue("SessionMode", 
				invokeInfo3.Controller, new SessionModeAttribute(SessionMode.ReadOnly));

			IHttpHandler handler3 = ActionHandler.CreateHandler(invokeInfo3);
			Assert.AreEqual(typeof(ReadOnlySessionActionHandler), handler3.GetType());

			Assert.AreEqual(false, handler3.IsReusable);
		}
	}
}
