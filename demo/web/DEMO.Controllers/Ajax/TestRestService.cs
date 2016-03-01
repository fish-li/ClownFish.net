using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClownFish.Web;

namespace DEMO.Controllers.Ajax
{
	public class TestRestService
	{
		[Action]
		public string Get(string input)
		{
			TestAutoActionService service = new TestAutoActionService();
			return service.Base64(input);
		}

		[Action]
		public string Post(string input)
		{
			TestAutoActionService service = new TestAutoActionService();
			return service.Md5(input);
		}


		[Action]
		public string Delete(string input)
		{
			TestAutoActionService service = new TestAutoActionService();
			return service.Sha1(input);
		}
	}
}
