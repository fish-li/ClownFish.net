using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Web;

namespace ClownFish.Web.UnitTest.Controllers
{

	public class TestModel1
	{
		public int Count { get; set; }

		public double Price { get; set; }

		[HttpValueIgnore]
		public double Total { get; set; }

		public override string ToString()
		{
			return string.Format("Count: {0}, Price: {1}, Total: {2}", Count, Price, Total);
		}
	}

	public class AttributeService : BaseController
	{

		[Action(OutFormat = SerializeFormat.Auto)]		// 由客户端指定返回结果格式
		public TestModel1 GetTotal(TestModel1 model)
		{
			return model;
		}



	}
}
