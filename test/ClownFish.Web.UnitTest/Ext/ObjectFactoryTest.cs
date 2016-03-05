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
	public class ObjectFactoryTest 
	{
		[TestCleanup]
		public void TestCleanup()
		{
			ExtenderManager.RemoveExtendType(typeof(TypeHookClassExt));
			ExtenderManager.RemoveSubscriber(typeof(EventClassEventSubscriber));
		}

		[TestMethod]
		public void Test1()
		{
			ExtenderManager.RegisterExtendType(typeof(TypeHookClassExt));

			TypeHookClass t1 = ObjectFactory.New<TypeHookClass>();
			int result = t1.Add(2, 3);
			Assert.AreEqual(15, result);		// 确认是调用的扩展类的Add实现
		}


		[TestMethod]
		public void Test2()
		{
			ExtenderManager.RegisterSubscriber(typeof(EventClassEventSubscriber));

			EventClass t2 = ObjectFactory.New<EventClass>();
			int result = t2.Add(2, 3);
			Assert.AreEqual(30, result);		// 结果为30的原因，可参考 EventClassEventSubscriber 的二个事件订阅代码
		}


		[TestMethod]
		public void Test3()
		{
			EventClass3 t3 = ObjectFactory.New<EventClass3>();
			int result = t3.Add(2, 3);
			Assert.AreEqual(5, result);		// 这个类没有事件订阅者，所以结果就是 5
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void Test4()
		{
			// 测试参数为NULL的场景

			Type t = null;
			ObjectFactory.New(t);
		}

		[ExpectedException(typeof(ArgumentException))]
		[TestMethod]
		public void Test5()
		{
			// EventClass2 没有无参的构造函数，所以会抛出异常

			ExtenderManager.RegisterSubscriber(typeof(EventClass2EventSubscriber));
		}
	}
}
