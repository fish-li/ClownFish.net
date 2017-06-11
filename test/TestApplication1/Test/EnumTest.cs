using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.TestApplication1.Common;
using ClownFish.Base.WebClient;

namespace ClownFish.TestApplication1.Test
{
	public class EnumTest : TestBase
	{
		[TestMethod("Enum测试：提交枚举名称")]
		public async Task TestSubmitName()
		{
			HttpOption option = new HttpOption {
				Url = "http://www.fish-web-demo.com/api/ns/Demo1/TestEnum.aspx?submit=submit",
				Data = new { week = "Thursday" },
				Method = "GET"
			};

			string actual = await option.SendAsync<string>();
			string expected = "Thursday";

			Assert.AreEqual(expected, actual);
		}


		[TestMethod("Enum测试：提交数字")]
		public async Task TestSubmitNumber()
		{
			HttpOption option = new HttpOption {
				Url = "http://www.fish-web-demo.com/api/ns/Demo1/TestEnum.aspx?submit=submit",
				Data = new { week = "2" },
				Method = "GET"
			};

			string actual = await option.SendAsync<string>();
			string expected = "Tuesday";

			Assert.AreEqual(expected, actual);
		}
	}
}
