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
	public class ExtenderManagerTest
	{
		[TestMethod]
		public void Test_Register_Remove_ExtendType()
		{
			TypeHookClass c1 = ObjectFactory.New<TypeHookClass>();
			Assert.AreEqual(5, c1.Add(2, 3));


			ExtenderManager.RegisterExtendType(typeof(TypeHookClassExt));

			TypeHookClass c2 = ObjectFactory.New<TypeHookClass>();
			Assert.AreEqual(15, c2.Add(2, 3));


			ExtenderManager.RemoveExtendType(typeof(TypeHookClassExt));

			TypeHookClass c3 = ObjectFactory.New<TypeHookClass>();
			Assert.AreEqual(5, c3.Add(2, 3));
		}


		[TestMethod]
		public void Test_Register_Remove_EventSubscriber()
		{
			EventClass c1 = ObjectFactory.New<EventClass>();
			Assert.AreEqual(5, c1.Add(2, 3));


			ExtenderManager.RegisterSubscriber(typeof(EventClassEventSubscriber));

			EventClass c2 = ObjectFactory.New<EventClass>();
			Assert.AreEqual(30, c2.Add(2, 3));


			ExtenderManager.RemoveSubscriber(typeof(EventClassEventSubscriber));

			EventClass c3 = ObjectFactory.New<EventClass>();
			Assert.AreEqual(5, c3.Add(2, 3));
		}
	}
}
