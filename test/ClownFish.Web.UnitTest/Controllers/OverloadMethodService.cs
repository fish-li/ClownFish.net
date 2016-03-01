using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Web;

namespace ClownFish.Web.UnitTest.Controllers
{
	public class OverloadMethodService : BaseController
	{
		// 增加ValidateRequest设置仅仅只是为了单元测试的代码覆盖率，在这里没什么用
		[Action(Verb = "get", ValidateRequest = ValidateRequestMode.Disable)]
		public int Add(int a, int b)
		{
			return a + b;
		}

		[Action(Verb = "post", ValidateRequest = ValidateRequestMode.Enable)]
		public int Add(int a, int b, VoidType x)
		{
			// 说明：Action 要实现方法名重载，需要2个条件：
			// 1、增加一个无效的类型（例如：VoidType）
			// 2、用 Verb 指定不同的请求类型

			return a + b + 10;
		}

		


	}
}
