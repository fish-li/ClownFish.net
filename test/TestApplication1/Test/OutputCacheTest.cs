using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Web.Client;
using ClownFish.TestApplication1.Common;

namespace ClownFish.TestApplication1.Test
{
	public class OutputCacheTest : TestBase
	{
		[TestMethod("OutputCache测试")]
		public async Task TestSubmitName()
		{
			HttpOption option = new HttpOption {
				Url = "http://www.fish-mvc-demo.com/Pages/Demo/TestOutputCache.aspx",
				Method = "GET"
			};

			string firstText = await option.SendAsync<string>();

			await Task.Run(() => System.Threading.Thread.Sleep(100));

			string secondText = await option.SendAsync<string>();
			Assert.AreEqual(firstText, secondText);

			await Task.Run(() => System.Threading.Thread.Sleep(100));

			string text3 = await option.SendAsync<string>();
			Assert.AreEqual(firstText, text3);

			await Task.Run(() => System.Threading.Thread.Sleep(3000));

			string text4 = await option.SendAsync<string>();
			Assert.AreNotEqual(firstText, text4);
			
		}
	}
}
