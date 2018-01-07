using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data.UnitTest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Data.UnitTest
{
	[TestClass]
	public class EntityProxyFactoryTest : BaseTest
	{
		[TestMethod]
		public void Test_EntityProxyFactory_Register()
		{
			Product product = Entity.BeginEdit<Product>();
			Type t1 = product.GetType();

			Assert.AreEqual(typeof(Product), t1.BaseType);


			// 覆盖初始化时的注册，结果其实是一样的
			EntityProxyFactory.Register(t1);

			Product product2 = Entity.BeginEdit<Product>();
			Type t2 = product2.GetType();

			Assert.AreEqual(typeof(Product), t2.BaseType);
			Assert.AreEqual(t1, t2);
		}
	}
}
