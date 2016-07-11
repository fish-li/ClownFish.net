using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.WebClient;
using ClownFish.TestApplication1.Common;

namespace ClownFish.TestApplication1.Test
{
	public class AuthorizeTest : TestBase
	{
		[TestMethod("权限测试：访问公共页面")]
		public async Task TestAllUser()
		{
			HttpOption option = new HttpOption {
				Url = "http://www.fish-mvc-demo.com/Pages/Demo/TestAuthorize/AllUser.aspx",
				Method = "GET"
			};

			string actual = await option.SendAsync<string>();

			Assert.IsTrue(actual.IndexOf("此页面可由所有用户访问。") > 0);
		}


		[TestMethod("权限测试：访问已登录用户才能访问的页面")]
		public async Task TestLoginUser()
		{
			HttpClientEventSubscriber.ShareCookie = new CookieContainer();

			HttpOption option = new HttpOption {
				Url = "http://www.fish-mvc-demo.com/Pages/Demo/TestAuthorize/LoginUser.aspx",
				Method = "GET"
			};

			int stateCode = await option.GetStatusCode();
			Assert.AreEqual(403, stateCode);

			// 登录
			await Login("fish11");

			// 登录后再试一次
			stateCode = await option.GetStatusCode();
			Assert.AreEqual(200, stateCode);


			// 注销用户
			await Logout();


			HttpClientEventSubscriber.ShareCookie = null;
		}



		[TestMethod("权限测试：访问[Fish]才能访问的页面")]
		public async Task TestFishUser()
		{
			HttpClientEventSubscriber.ShareCookie = new CookieContainer();

			HttpOption option = new HttpOption {
				Url = "http://www.fish-mvc-demo.com/Pages/Demo/TestAuthorize/Fish.aspx",
				Method = "GET"
			};

			int stateCode = await option.GetStatusCode();
			Assert.AreEqual(403, stateCode);


			// 登录
			await Login("fish11");


			// 登录后再试一次
			stateCode = await option.GetStatusCode();
			Assert.AreEqual(403, stateCode);


			// 用Fish登录后再试一次
			await Login("Fish");

			stateCode = await option.GetStatusCode();
			Assert.AreEqual(200, stateCode);


			// 注销用户
			await Logout();

			HttpClientEventSubscriber.ShareCookie = null;
		}


		private async Task Login(string username, string rightNo = null)
		{
			HttpOption option2 = new HttpOption {
				Url = "http://www.fish-mvc-demo.com/user/Login.aspx",
				Method = "POST",
				Data = new { username = username, rightNo = rightNo }
			};
			int stateCode2 = await option2.GetStatusCode();
			Assert.AreEqual(200, stateCode2);
		}

		private async Task Logout()
		{
			HttpOption option9 = new HttpOption {
				Url = "http://www.fish-mvc-demo.com/user/Logout.aspx",
				Method = "POST",
				Data = new { Logout = "注销" }
			};
			int stateCode9 = await option9.GetStatusCode();
			Assert.AreEqual(200, stateCode9);

		}



		[TestMethod("权限测试：访问[RightNo=23]才能访问的页面")]
		public async Task TestRightNo23()
		{
			HttpClientEventSubscriber.ShareCookie = new CookieContainer();

			HttpOption option = new HttpOption {
				Url = "http://www.fish-mvc-demo.com/Pages/Demo/TestAuthorize/RightNo23.aspx",
				Method = "GET"
			};

			int stateCode = await option.GetStatusCode();
			Assert.AreEqual(403, stateCode);


			// 登录
			await Login("Fish");


			// 登录后再试一次
			stateCode = await option.GetStatusCode();
			Assert.AreEqual(403, stateCode);


			// 用Fish登录后再试一次
			await Login("Fish", "23");


			stateCode = await option.GetStatusCode();
			Assert.AreEqual(200, stateCode);


			// 注销用户
			await Logout();

			HttpClientEventSubscriber.ShareCookie = null;
		}

	}
}
