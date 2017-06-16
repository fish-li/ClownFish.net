using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClownFish.AspnetMock;
using System.Web.Caching;
using ClownFish.Base.Files;

namespace ClownFish.Web.UnitTest.Cache
{
	[TestClass]
	public class FileDependencyManagerTest 
	{
		private static readonly string s_testFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"FileDependencyManagerTest.txt");


		private static string SafeReadFile(string filePath)
		{
			if( File.Exists(filePath) )
				return File.ReadAllText(filePath, Encoding.UTF8);

			return null;
		}

		[TestInitialize]
		public void Init2()
		{
			File.WriteAllText(s_testFilePath, "abc", Encoding.UTF8);

			// 设置 FileDependencyManager.RemovedCallback 的等待时间
			typeof(FileDependencyManager<string>).SetValue("s_WaitFileCloseTimeout", null, 100);
		}

		[TestMethod]
		public void Test_正常的文件缓存依赖行为()
		{
			FileDependencyManager<string> cacheItem = new FileDependencyManager<string>(
					files => SafeReadFile(files[0]),
					s_testFilePath);

			Assert.AreEqual("abc", cacheItem.Result);

			// 修改文件
			File.WriteAllText(s_testFilePath, "Fish Li", Encoding.UTF8);

			// FileDependencyManager.RemovedCallback 有等待时间，所以这里要比那个时间再长一点
			System.Threading.Thread.Sleep(300);

			// 确认能拿到最新的结果
			Assert.AreEqual("Fish Li", cacheItem.Result);

		}



		[ExpectedException(typeof(NotImplementedException))]
		[TestMethod]
		public void Test_CacheResult_NotImplementedException()
		{
			CacheResult<string> result1 = new CacheResult<string>("Fish Li", null);
			Assert.AreEqual("Fish Li", result1.Result);


			CacheResult<string> result2 = new CacheResult<string>("Fish Li", new NotImplementedException());
			Assert.AreEqual("Fish Li", result2.Result);		// 这个断言应该会抛出异常
		}


		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void Test_FileDependencyManager_ctor_ArgumentNullException1()
		{
			FileDependencyManager<string> cacheItem = new FileDependencyManager<string>(
					null,
					s_testFilePath);
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void Test_FileDependencyManager_ctor_ArgumentNullException2()
		{
			FileDependencyManager<string> cacheItem = new FileDependencyManager<string>(
					files => SafeReadFile(files[0]),
					(string[])null);
		}


		[ExpectedException(typeof(NotImplementedException))]
		[TestMethod]
		public void Test_FileDependencyManager_LoadFile()
		{
			FileDependencyManager<string> cacheItem = new FileDependencyManager<string>(
					files => ErrorReadFile(files[0]),
					s_testFilePath);

			string ss = cacheItem.Result;
		}

		private string ErrorReadFile(string filePath)
		{
			throw new NotImplementedException();
		}
	}
}
