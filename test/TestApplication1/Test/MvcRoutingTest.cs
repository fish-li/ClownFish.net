using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.WebClient;
using ClownFish.TestApplication1.Common;

namespace ClownFish.TestApplication1.Test
{
	class MvcRoutingTest : TestBase
	{
		[TestMethod("MvcRouting测试：Base64")]
		public void Test1()
		{
			HttpOption option = new HttpOption {
				Url = "http://www.fish-mvc-demo.com/mvc-routing/ns/TestAutoAction/Base64",
				Data = new { input = "Fish Li" }

			};

			string actual = option.Send<string>();
			string expected = "RmlzaCBMaQ==";
			Assert.AreEqual(expected, actual);
		}


		[TestMethod("MvcRouting测试：Md5")]
		public void Test2()
		{
			HttpOption option = new HttpOption {
				Url = "http://www.fish-mvc-demo.com/mvc-routing/ns/TestAutoAction/Md5",
				Data = new { input = "Fish Li" }
			};

			string actual = option.Send<string>();
			string expected = "44D2D9635ED5CDEA2A858CD7A1CC2B0C";
			Assert.AreEqual(expected, actual);
		}


		[TestMethod("MvcRouting测试：Sha1")]
		public void Test3()
		{
			HttpOption option = new HttpOption {
				Url = "http://www.fish-mvc-demo.com/mvc-routing/ns/TestAutoAction/Sha1",
				Data = new { input = "Fish Li" }
			};

			string actual = option.Send<string>();
			string expected = "A6DCC78B685D0CEA701CA90A948B9295F3685FDF";
			Assert.AreEqual(expected, actual);

		}
	}
}
