using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.WebTest
{
	// 测试：Action名字自动匹配功能


	[TestClass]
	public class AutoFindActionTest : BaseTest
	{
		[TestMethod]
		public void ActionAutoFind_Base64()
		{
			string requestText = @"
POST http://www.fish-web-demo.com/Ajax/test/ActionAutoFind/submit.aspx HTTP/1.1

input=fish&Base64=OK
";
			string result = ExecuteService(requestText);
			Assert.AreEqual("ZmlzaA==", result);
		}


		[TestMethod]
		public void ActionAutoFind_Md5()
		{
			string requestText = @"
POST http://www.fish-web-demo.com/Ajax/test/ActionAutoFind/submit.aspx HTTP/1.1

input=fish&Md5=OK
";
			string result = ExecuteService(requestText);
			Assert.AreEqual("83E4A96AED96436C621B9809E258B309", result);
		}


		[TestMethod]
		public void ActionAutoFind_Sha1()
		{
			string requestText = @"
POST http://www.fish-web-demo.com/Ajax/test/ActionAutoFind/submit.aspx HTTP/1.1

input=fish&Sha1=OK
";
			string result = ExecuteService(requestText);
			Assert.AreEqual("64875FCCCAAC069FCB3E0E201E7D5B9166641608", result);
		}


	}
}
