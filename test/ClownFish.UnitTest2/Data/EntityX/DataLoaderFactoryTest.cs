using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.UnitTest.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Data;

namespace ClownFish.UnitTest.Data.EntityX
{
	[TestClass]
	public class DataLoaderFactoryTest : BaseTest
	{
		public class Customer22 
		{
			public int CustomerID { get; set; }
			public string CustomerName { get; set; }
			public string ContactName { get; set; }
			public string Address { get; set; }
			public string PostalCode { get; set; }
			public string Tel { get; set; }
		}


		[TestMethod]
		public void Test_GetLoaderType()
        {
			Type loaderType = DataLoaderFactory.GetLoaderType(typeof(Customer));
			Assert.IsNotNull(loaderType);

			Type entityType = DataLoaderFactory.GetEntityType(loaderType);
			Assert.IsNotNull(entityType);
			Assert.AreEqual(typeof(Customer), entityType);


			Type loaderType2 = DataLoaderFactory.GetLoaderType(typeof(Customer22));
			Assert.IsNotNull(loaderType2);
			Assert.AreEqual(typeof(DefaultDataLoader<>), loaderType2);
		}

		[TestMethod]
		public void Test_GetLoader()
		{			
			// 从代理程序集中获取的类型
			object loader1 = DataLoaderFactory.GetLoader<Customer>();
			Assert.IsNotNull(loader1);
			Console.WriteLine(loader1.GetType().Name);

			// 没有注册的类型
			object loader2 = DataLoaderFactory.GetLoader<NameValue>();
			Assert.IsNotNull(loader2);
			Assert.IsTrue(loader2.GetType() == typeof(DefaultDataLoader<NameValue>));

		}



		[TestMethod]
		public void Test_RegisterInstance()
		{
			// 没有注册的类型
			object loader = DataLoaderFactory.GetLoader<NameValue>();

			// 注册加载器实例
			DataLoaderFactory.RegisterInstance(loader);

			// 尝试获取
			object loader2 = DataLoaderFactory.GetLoader<NameValue>();
			Assert.IsTrue(object.ReferenceEquals(loader, loader2));
			
			// 移除注册
			RemoveRegister(typeof(NameValue));

			// 确认移除
			object loader3 = DataLoaderFactory.GetLoader<NameValue>();
			Assert.IsFalse(object.ReferenceEquals(loader, loader3));
		}


		private void RemoveRegister(Type t)
		{
			Hashtable table = (Hashtable)typeof(DataLoaderFactory).InvokeMember("s_table",
						BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Static,
						null, null, null);

			table.Remove(t);
		}




		[TestMethod]
		public void Test_RegisterType()
		{
			// 没有注册的类型
			object loader = DataLoaderFactory.GetLoader<NameValue>();

			// 注册加载器类型
			DataLoaderFactory.RegisterType(loader.GetType());

			// 尝试获取
			object loader2 = DataLoaderFactory.GetLoader<NameValue>();
			Assert.IsNotNull(loader2);
			Assert.IsTrue(loader2.GetType() == typeof(DefaultDataLoader<NameValue>));


			// 移除注册
			RemoveRegister(typeof(NameValue));
		}

		[TestMethod]
		public void Test_Error()
        {
			MyAssert.IsError<ArgumentNullException>(() => {
				_= DataLoaderFactory.GetEntityType(null);
			});

			MyAssert.IsError<ArgumentNullException>(() => {
				_ = DataLoaderFactory.GetEntityType(null);
			});


			MyAssert.IsError<ArgumentException>(() => {
				_ = DataLoaderFactory.GetEntityType(typeof(Customer22));
			});

			MyAssert.IsError<ArgumentException>(() => {
				_ = DataLoaderFactory.GetEntityType(typeof(BaseTest));
			});


			MyAssert.IsError<ArgumentNullException>(() => {
				DataLoaderFactory.RegisterInstance(null);
			});


			MyAssert.IsError<ArgumentException>(() => {
				DataLoaderFactory.RegisterInstance(typeof(Customer22));
			});
		}
	}
}
