using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.TypeExtend;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Base.UnitTest.TypeExtend
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
			// BaseObject 有继承类，还有事件订阅者

			BaseObject baseObject = ObjectFactory.New<BaseObject>();

			Assert.AreEqual(BaseObjectExt.ExecuteResult, baseObject.Execute());


			baseObject = ObjectFactory.New<BaseObject>();
			Assert.AreEqual("012", baseObject.EventExecute("0"));
		}
	}




	
}
