using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Http;
using ClownFish.Web;

namespace DEMO.Controllers.Services
{

	[Authorize(Users = "fish")]
	public class TestCorsService
	{

		[Action(OutFormat = SerializeFormat.Json)]
		public object Add(int a, int b)
		{
			return new {
				result = a + b
			};
		}

		[Action(OutFormat = SerializeFormat.Json)]
		public object Test2(string input)
		{
			TestAutoActionService service = new TestAutoActionService();

			return new {
				md5 = service.Md5(input),
				sha1 = service.Sha1(input)
			};
		}


		


	}
}
