using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.WebTest
{
	// 测试： Action支持Enum参数数据类型



	[TestClass]
	public class InputEnumTest : BaseTest
	{

		[TestMethod]
		public void Input_Enum()
		{
			string requestText = @"
POST http://www.fish-web-demo.com/Ajax/test/DataTypeTest/Input_Enum.aspx HTTP/1.1

day=Friday";

			string result = ExecuteService(requestText);
			Assert.AreEqual("Friday", result);
		}


		[TestMethod]
		public void Input_Enum2()
		{
			string requestText = @"
POST http://www.fish-web-demo.com/Ajax/test/DataTypeTest/Input_Enum.aspx HTTP/1.1

day=5";

			string result = ExecuteService(requestText);
			Assert.AreEqual("Friday", result);
		}


		[ExpectedException(typeof(ArgumentException))]
		[TestMethod]
		public void Input_Enum3()
		{
			string requestText = @"
POST http://www.fish-web-demo.com/Ajax/test/DataTypeTest/Input_Enum.aspx HTTP/1.1

a=3
";
			string result = ExecuteService(requestText);
			// System.ArgumentException: 未能找到指定的参数值：day
		}

	}
}
