using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Data.UnitTest
{
	[TestClass]
	public abstract class BaseTest
	{
		[TestInitialize]
		public void RestCPQueryParamIndex()
		{
			// 将CPQuery的内部参数序号计数器重置为零，便于做SQL语句的断言
			typeof(CPQuery).InvokeMember("s_index",
								BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static,
								null, null, new object[] { 0 });
		}



		public void AssertLastExecuteSQL(string text)
		{
			Assert.AreEqual(text, ClownFishDataEventSubscriber.LastExecuteSQL);
		}
	}
}
