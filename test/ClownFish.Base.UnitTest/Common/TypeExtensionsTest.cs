using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.WebClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Base.UnitTest.Common
{
	[TestClass]
	public class TypeExtensionsTest
	{
		[TestMethod]
		public void Test_IsCommonValueType()
		{
			Assert.IsTrue(typeof(DateTime).IsCommonValueType());
			Assert.IsTrue(typeof(Guid).IsCommonValueType());
			Assert.IsTrue(typeof(decimal).IsCommonValueType());
			Assert.IsTrue(typeof(DayOfWeek).IsCommonValueType());

		}

		[TestMethod]
		public void Test_GetRealType()
		{
			Assert.AreEqual(typeof(int), typeof(int).GetRealType());
			Assert.AreEqual(typeof(int), typeof(int?).GetRealType());

			Assert.AreNotEqual(typeof(int), typeof(long?).GetRealType());
		}

		[TestMethod]
		public void Test_IsNullableType()
		{
			Assert.IsFalse(typeof(DateTime).IsNullableType());
			Assert.IsTrue(typeof(DateTime?).IsNullableType());

			Assert.IsFalse(typeof(Guid).IsNullableType());			
			Assert.IsTrue(typeof(Guid?).IsNullableType());
		}


		[TestMethod]
		public void Test_IsCompatible()
		{
			Assert.IsTrue(typeof(MemoryStream).IsCompatible(typeof(Stream)));
			Assert.IsTrue(typeof(MemoryStream).IsCompatible(typeof(IDisposable)));

			Assert.IsFalse(typeof(FileStream).IsCompatible(typeof(MemoryStream)));
		}


		[TestMethod]
		public void Test_HasReturn()
		{
			Assert.IsTrue(typeof(TypeExtensions).GetMethod("HasReturn").HasReturn());
			Assert.IsTrue(typeof(TypeExtensions).GetMethod("IsTaskMethod").HasReturn());

			Assert.IsFalse(typeof(TestHelper).GetMethod("TryThrowException").HasReturn());
		}


		[TestMethod]
		public void Test_IsTaskMethod()
		{
			Assert.IsTrue(typeof(HttpClient).GetMethod("SetRequestDataAsync").IsTaskMethod());
			Assert.IsTrue(typeof(HttpClient).GetMethod("GetResponseAsync").IsTaskMethod());
			Assert.IsTrue(typeof(HttpClient).GetMethod("SendAsync").IsTaskMethod());
		}


		[TestMethod]
		public void Test_GetTaskMethodResultType()
		{
			Assert.AreEqual(typeof(HttpWebResponse), 
							typeof(HttpClient).GetMethod("GetResponseAsync").GetTaskMethodResultType());

			// return Task
			Assert.AreEqual(null,
							typeof(HttpClient).GetMethod("SetRequestDataAsync").GetTaskMethodResultType());

			// return Task<T>
			Assert.IsTrue(typeof(HttpClient).GetMethod("SendAsync").GetTaskMethodResultType().IsGenericParameter);

		}

	}
}
