using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Data.UnitTest
{
	[TestClass]
	public abstract class BaseTestWithConnectionScope : BaseTest
	{
		private ConnectionScope _scope;

		[TestInitialize]
		public void TestInitialize()
		{
			_scope = ConnectionScope.Create();
		}

		[TestCleanup]
		public void TestCleanup()
		{
			_scope.Dispose();
		}

	}
}
