using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ClownFish.Web;

namespace ClownFish.Web.UnitTest.Controllers
{
	public class TaskService
	{
		public async Task<int> Add(HttpContext context, int a, int b)
		{
			context.WriteHeader("Add-before-await");

			context.WriteHeader("---------------------------------------------------------");

			int c = await AddSync(a, b);

			context.WriteHeader("Add-after-await-return");

			return c + 99;
		}

		private Task<int> AddSync(int a, int b)
		{
			return Task.Run(() => {
				System.Threading.Thread.Sleep(1000);

				return a + b + 1;	// 故意加错
			});
		}
	}
}
