using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Base.UnitTest.Common
{
	[TestClass]
	public class DataTimeExtensionsTest
	{
		[TestMethod]
		public void Test_日期转字符串_长格式()
		{
			DateTime dt = new DateTime(2016, 1, 2, 11, 22, 33);
			Assert.AreEqual("2016-01-02 11:22:33", dt.ToTimeString());
		}

		[TestMethod]
		public void Test_日期转字符串_短格式()
		{
			DateTime dt = new DateTime(2016, 1, 2, 11, 22, 33);
			Assert.AreEqual("2016-01-02", dt.ToDateString());
		}
	}
}
