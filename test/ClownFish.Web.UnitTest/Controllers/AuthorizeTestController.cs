using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DEMO.Controllers;

namespace ClownFish.Web.UnitTest.Controllers
{
	// 测试：[Authorize]，以及继承AuthorizeAttribute的自定义的授权检查
	// 测试：[PageUrl]


	public class AuthorizeTestController : BaseController
	{
		[PageUrl(Url = "/Pages/Demo/Authorize/Everyone.aspx")]
		public string ShowPublicPage()
		{
			return "Hello ClownFish.net";
		}


		[Authorize]
		[PageUrl(Url = "/Pages/Demo/Authorize/LoginUser.aspx")]
		public string ShowLoginUserPage()
		{
			// 仅当：当前用户是已登录用户时，才允许访问这个地址

			return "Hello ClownFish.net";
		}


		[CheckRight(RightNo = "23")]
		[PageUrl(Url = "/Pages/Demo/Authorize/RightNo23.aspx")]
		public object TestCheckRightAttrribute()
		{
			// 仅当：当前用户已登录，并且拥有 “23” 号权限时，才允许访问这个地址
			return "Hello ClownFish.net";
		}


		[Authorize(Users = "fish")]
		[PageUrl(Url = "/Pages/Demo/Authorize/Fish.aspx")]
		public object ShowFishPage()
		{
			// 仅当当前用户是 fish 时，才允许访问这个地址

			return "Hello Fish Li";
		}


	}


	[Authorize]
	public class AuthorizeTest2Controller : BaseController
	{
		// 测试在Controller中加[Authorize] ，此时，所有方法都要求已登录用户才能访问
		// 事实上，还可以指定用户名，或者使用[CheckRight]来指定特殊的制授权验证，这里就不再测试了，因为和Action的检查逻辑一样。


		[PageUrl(Url = "/Pages/Demo/Authorize/Add.aspx")]
		public int Add(int a, int b)
		{
			return a + b + 20;
		}

	}
}
