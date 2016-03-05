using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.TypeExtend;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.Ext
{
	[TestClass]
	public class ObjectFactoryTest2
	{
		[TestInitialize]
		public void Init()
		{
			ExtenderManager.RegisterExtendType(typeof(BaseObjectExt));
			ExtenderManager.RegisterSubscriber(typeof(BaseObjectEventSubscriber));
		}

		[TestCleanup]
		public void TestCleanup()
		{
			ExtenderManager.RemoveExtendType(typeof(BaseObjectExt));
			ExtenderManager.RemoveSubscriber(typeof(BaseObjectEventSubscriber));
		}


		[TestMethod]
		public void Execute()
		{
			BaseObject baseObject = ObjectFactory.New<BaseObject>();

			Assert.AreEqual(BaseObjectExt.ExecuteResult, baseObject.Execute());


			baseObject = ObjectFactory.New<BaseObject>();
			Assert.AreEqual("012", baseObject.EventExecute("0"));

			//Assert.Throws<ArgumentNullException>(() => ObjectFactory.New(null));
		}
	}




	public class ExecuteEventArgs : EventArgs
	{
		public string Data { get; set; }
	}
	public class BaseObject : BaseEventObject		// 定义二个事件，允许外部扩展（订阅事件）
	{
		public event EventHandler<ExecuteEventArgs> BeforExecute;
		public event EventHandler<ExecuteEventArgs> AfterExecute;
		public virtual string Execute()
		{
			return "BaseObject";
		}

		public string EventExecute(string data)
        {
            ExecuteEventArgs executeEventArgs = new ExecuteEventArgs {
                Data = data
            };
            EventHandler<ExecuteEventArgs> beforExecute = this.BeforExecute;
			if( beforExecute != null )
				beforExecute.Invoke(this,executeEventArgs);

             

            EventHandler<ExecuteEventArgs> afterExecute = this.AfterExecute;

			if( afterExecute != null )
				afterExecute.Invoke(this, executeEventArgs);

            return executeEventArgs.Data;
        }
	}



	public class BaseObjectExt : BaseObject		// 扩展BaseObject，采用类型继承方式
	{
		internal static readonly string ExecuteResult = "ExtendObject";

		public override string Execute()
		{
			return ExecuteResult;
		}
	}


	public class BaseObjectEventSubscriber : EventSubscriber<BaseObject>
	{
		#region Overrides of EventSubscriber<BaseObject>

		/// <summary>
		/// 订阅事件
		/// </summary>
		/// <param name="instance"></param>
		public override void SubscribeEvent(BaseObject instance)
		{
			instance.BeforExecute += this.BeforExecute;
			instance.AfterExecute += this.AfterExecute;

		}
		
		private void BeforExecute(object sender, ExecuteEventArgs eventArgs)
		{
			eventArgs.Data += "1";
		}
		private void AfterExecute(object sender, ExecuteEventArgs eventArgs)
		{
			eventArgs.Data += "2";
		}
		#endregion
	}

	public abstract class AbstractEventObject : EventSubscriber<BaseObjectExt>
	{

	}
}
