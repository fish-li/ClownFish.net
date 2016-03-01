using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.WebTest
{
	[TestClass]
	public class AsyncServiceTest : BaseTest
	{

		[TestMethod]
		public async Task Async_Task_Add()
		{
			string requestText = @"
GET http://www.fish-mvc-demo.com/Ajax/test/TaskService/Add.aspx?a=3&b=2 HTTP/1.1
";
			SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

			string result = await AsyncExecuteService(requestText);
			Assert.AreEqual("105", result);
		}


	}
}
