using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Web.Client;
using ClownFish.TestApplication1.Common;

namespace ClownFish.TestApplication1.Test
{
	class AutoFindActionTest : TestBase
	{
		[TestMethod("AutoFindAction测试：Base64")]
		public async Task TestAutoFindAction1()
		{
			HttpOption option = new HttpOption {
				Url = "http://www.fish-mvc-demo.com/Ajax/ns/TestAutoAction/submit.aspx",
				Method = "POST",
				Data = new { Base64 = "yes", input = "Fish Li" }
			};

			string actual = await option.SendAsync<string>();
			string expected = "RmlzaCBMaQ==";
			Assert.AreEqual(expected, actual);
		}


		[TestMethod("AutoFindAction测试：md5")]
		public async Task TestAutoFindAction2()
		{
			HttpOption option = new HttpOption {
				Url = "http://www.fish-mvc-demo.com/Ajax/ns/TestAutoAction/submit.aspx",
				Method = "POST",
				Data = new { Md5 = "yes", input = "Fish Li" }
			};

			string actual = await option.SendAsync<string>();
			string expected = "44D2D9635ED5CDEA2A858CD7A1CC2B0C";
			Assert.AreEqual(expected, actual);
		}


		[TestMethod("AutoFindAction测试：sha1")]
		public async Task TestAutoFindAction3()
		{
			HttpOption option = new HttpOption {
				Url = "http://www.fish-mvc-demo.com/Ajax/ns/TestAutoAction/submit.aspx",
				Method = "POST",
				Data = new { Sha1 = "yes", input = "Fish Li" }
			};

			string actual = await option.SendAsync<string>();
			string expected = "A6DCC78B685D0CEA701CA90A948B9295F3685FDF";
			Assert.AreEqual(expected, actual);

		}
	}
}
