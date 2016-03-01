using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.Log.Serializer;

namespace ClownFish.Log.UnitTest
{
	[TestClass]
	public class WriterFactoryTest: TestBase
	{
		// 这个测试类主要是验证返回值为 NULL 的场景

		private bool _eabled;

		[TestInitialize]
		public void Init()
		{
			_eabled = WriterFactory.Config.Enable;
		}

		[TestCleanup]
		public void Cleanup()
		{
			WriterFactory.Config.Enable = _eabled;
		}

		[TestMethod]
		public void Test_IsSupport()
		{
			WriterFactory.Config.Enable = false;

			bool result = WriterFactory.IsSupport(typeof(int));
			Assert.AreEqual(false, result);
		}


		[TestMethod]
		public void Test_CreateWriters()
		{
			WriterFactory.Config.Enable = false;

			var result = WriterFactory.CreateWriters(typeof(int));
			Assert.AreEqual(null, result);


			WriterFactory.Config.Enable = true;

			var result2 = WriterFactory.CreateWriters(typeof(int));
			Assert.AreEqual(null, result2);
		}


		[TestMethod]
		public void Test_GetWriters()
		{
			WriterFactory.Config.Enable = false;

			var result = WriterFactory.GetWriters(typeof(int));
			Assert.AreEqual(null, result);


			WriterFactory.Config.Enable = true;

			var result2 = WriterFactory.GetWriters(typeof(int));
			Assert.AreEqual(null, result2);
		}
	}
}
