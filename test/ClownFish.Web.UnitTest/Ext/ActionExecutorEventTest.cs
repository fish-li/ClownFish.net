using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.TypeExtend;
using ClownFish.AspnetMock;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.Ext
{
	[TestClass]
	public class ActionExecutorEventTest : BaseTest
	{
		
		[TestMethod]
		public void Test1()
		{
			string requestText = @"
POST http://www.fish-mvc-demo.com/Ajax/test/DataTypeTest/Input_int_Add.aspx HTTP/1.1
Origin: http://www.abc.com

a=3&b=5
";
			Hashtable table = (WebRuntimeExt.Instance as WebRuntimeExt).CallMessage;
			table.Clear();

			string result = ExecuteService(requestText);

			Assert.AreEqual("http://www.abc.com", table["Access-Control-Allow-Origin"]);
			Assert.AreEqual("*", table["Access-Control-Allow-Headers"]);
			Assert.AreEqual("true", table["Access-Control-Allow-Credentials"]);
			Assert.AreEqual("*", table["Access-Control-Allow-Methods"]);

		}
	}
}
