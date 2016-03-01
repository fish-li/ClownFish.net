using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.WebTest
{
	// 测试： Action支持GUID的参数数据类型

	[TestClass]
	public class InputGuidTest : BaseTest
	{

		[TestMethod]
		public void Input_Guid()
		{
			Guid g = Guid.NewGuid();

			string requestText = @"
POST http://www.fish-mvc-demo.com/Ajax/test/DataTypeTest/Input_Guid.aspx HTTP/1.1

guid=" + g.ToString();
			string result = ExecuteService(requestText);
			Assert.AreEqual(g.ToString("N"), result);
		}


		[TestMethod]
		public void Input_Guid_nullable()
		{
			string requestText = @"
GET http://www.fish-mvc-demo.com/Ajax/test/DataTypeTest/Input_Guid_nullable.aspx HTTP/1.1
";
			string result = ExecuteService(requestText);
			Assert.AreEqual(Guid.Empty.ToString("N"), result);
		}


	}
}
