using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.TypeExtend;
using ClownFish.AspnetMock;
using ClownFish.Web.UnitTest.Controllers;
using ClownFish.Web.UnitTest.Ext;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.WebTest
{
	[TestClass]
	public class AdvancedDataTest : BaseTest
	{

		[TestMethod]
		public void Test1()
		{
			string requestText = @"
POST http://www.fish-web-demo.com/Ajax/test/AdvancedData/TestContextDataAttribute.aspx HTTP/1.1
";

			Hashtable table = (WebRuntimeExt.Instance as WebRuntimeExt).CallMessage;
			table.Clear();

			using( WebContext context = WebContext.FromRawText(requestText) ) {
				context.SetUserName("fish");

				ExecuteService(context);
			}
			
			Assert.AreEqual("POST", table["Request.HttpMethod"]);
			Assert.AreEqual("fish", table["User.Identity.Name"]);
			Assert.AreEqual("/", table["HttpRuntime.AppDomainAppVirtualPath"]);
		}


		[TestMethod]
		public void Test2()
		{
			byte[] bb = new byte[] { 2, 5, 6, 9, 8, 45, 36, 78, 15, 96, 14, 25, 35, 12, 5, 7, 56, 45 };
			string base64 = Convert.ToBase64String(bb);

			StringBuilder sb = new StringBuilder();
			sb.AppendLine("POST http://www.fish-web-demo.com/Ajax/test/AdvancedData/InputByteArray.aspx HTTP/1.1");
			sb.AppendLine("Content-Type: application/x-www-form-urlencoded; charset=UTF-8");
			sb.AppendLine();
			sb.Append("bb=").Append(base64);

			string responseText = ExecuteService(sb.ToString());

			Assert.AreEqual(base64, responseText);
		}

		[TestMethod]
		public void Test3()
		{
			byte[] bb = new byte[] { 2, 5, 6, 9, 8, 45, 36, 78, 15, 96, 14, 25, 35, 12, 5, 7, 56, 45 };
			string base64 = Convert.ToBase64String(bb);

			ByteArrayItem item = new ByteArrayItem { StrValue = "123", ByteArray = bb };

			StringBuilder sb = new StringBuilder();
			sb.AppendLine("POST http://www.fish-web-demo.com/Ajax/test/AdvancedData/InputByteArrayItem.aspx HTTP/1.1");
			sb.AppendLine("Content-Type: application/x-www-form-urlencoded; charset=UTF-8");
			sb.AppendLine();
			sb.Append("strvalue=123&bytearray=").Append(base64);

			string responseText = ExecuteService(sb.ToString());

			Assert.AreEqual(item.ToString(), responseText);
		}


	}
}
