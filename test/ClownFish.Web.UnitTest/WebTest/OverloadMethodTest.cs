using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.WebTest
{
	[TestClass]
	public class OverloadMethodTest : BaseTest
	{

		[TestMethod]
		public void Add_get()
		{
			string requestText = @"
GET http://www.fish-mvc-demo.com/Ajax/test/OverloadMethod/Add.aspx?a=2&b=3 HTTP/1.1
";
			string result = ExecuteService(requestText);
			Assert.AreEqual("5", result);
		}



		[TestMethod]
		public void Add_post()
		{
			string requestText = @"
POST http://www.fish-mvc-demo.com/Ajax/test/OverloadMethod/Add.aspx HTTP/1.1

a=2&b=3
";
			string result = ExecuteService(requestText);
			Assert.AreEqual("15", result);
		}






	}
}
