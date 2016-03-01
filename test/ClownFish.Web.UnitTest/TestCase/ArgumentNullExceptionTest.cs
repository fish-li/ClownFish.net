using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.TestCase
{
	[TestClass]
	public class ArgumentNullExceptionTest
	{
		// 这个测试类用于验证各种参数为NULL，抛出 ArgumentNullException 的场景


		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void Test_ContextDataAttribute()
		{
			ContextDataAttribute a = new ContextDataAttribute(null);
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void Test_ContextDataAttribute2()
		{
			ContextDataAttribute a = new ContextDataAttribute(string.Empty);
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void Test_ExceptionHelper1()
		{
			ExceptionHelper.Throw403Exception(null);
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void Test_ExceptionHelper2()
		{
			ExceptionHelper.Throw404Exception(null);
		}


		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void Test_NamespaceMapAttribute()
		{
			NamespaceMapAttribute a = new NamespaceMapAttribute(null, "abc");
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void Test_NamespaceMapAttribute2()
		{
			NamespaceMapAttribute a = new NamespaceMapAttribute("aaaaaaaaaaaaa", null);
		}

		[ExpectedException(typeof(ArgumentException))]
		[TestMethod]
		public void Test_NamespaceMapAttribute3()
		{
			NamespaceMapAttribute a = new NamespaceMapAttribute("aaaaaaaaaaaaa", "a.b");
		}
	}
}
