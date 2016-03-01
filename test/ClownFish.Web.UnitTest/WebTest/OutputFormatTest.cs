using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.WebTest
{
	[TestClass]
	public class OutputFormatTest : BaseTest
	{
		[TestMethod]
		public void Result_Format_Xml()
		{
			string requestText = @"
POST http://www.fish-mvc-demo.com/Ajax/test/Attribute/GetTotal.aspx HTTP/1.1
X-Result-Format: XML

Count=2&Price=3.1&Total=100.23
";
			string result = ExecuteService(requestText);

			string expect = "<Count>2</Count><Price>3.1</Price><Total>0</Total>";

			// 因为 Total 属性有 HttpValueIgnoreAttribute 标记，所以它应该不能接收数据。
			Assert.IsTrue(result.Replace("\r\n", "").Replace(" ", "").IndexOf(expect) > 0);
		}




		[TestMethod]
		public void Result_Format_JSON()
		{
			string requestText = @"
POST http://www.fish-mvc-demo.com/Ajax/test/Attribute/GetTotal.aspx HTTP/1.1
X-Result-Format: JSON

Count=2&Price=3.1&Total=100.23
";
			string result = ExecuteService(requestText);

			string expect = "{\"Count\":2,\"Price\":3.1,\"Total\":0.0}";

			// 因为 Total 属性有 HttpValueIgnoreAttribute 标记，所以它应该不能接收数据。
			Assert.AreEqual(expect, result);
		}



		[TestMethod]
		public void Result_Format_JSON2()
		{
			string requestText = @"
POST http://www.fish-mvc-demo.com/Ajax/test/Attribute/GetTotal.aspx HTTP/1.1
X-Result-Format: JSON2

Count=2&Price=3.1&Total=100.23
";
			string result = ExecuteService(requestText);

			string expect = "{\"$type\":\"ClownFish.Web.UnitTest.Controllers.TestModel1, ClownFish.Web.UnitTest\",\"Count\":2,\"Price\":3.1,\"Total\":0.0}";

			// 因为 Total 属性有 HttpValueIgnoreAttribute 标记，所以它应该不能接收数据。
			Assert.AreEqual(expect, result);
		}

		[TestMethod]
		public void Result_Format_FORM()
		{
			string requestText = @"
POST http://www.fish-mvc-demo.com/Ajax/test/Attribute/GetTotal.aspx HTTP/1.1
X-Result-Format: FORM

Count=2&Price=3.1&Total=100.23
";
			string result = ExecuteService(requestText);

			string expect = "Count=2&Price=3.1&Total=0";

			// 因为 Total 属性有 HttpValueIgnoreAttribute 标记，所以它应该不能接收数据。
			Assert.AreEqual(expect, result);
		}


		[TestMethod]
		public void Result_Format_TEXT()
		{
			string requestText = @"
POST http://www.fish-mvc-demo.com/Ajax/test/Attribute/GetTotal.aspx HTTP/1.1
X-Result-Format: TEXT

Count=2&Price=3.1&Total=100.23
";
			string result = ExecuteService(requestText);

			string expect = "Count: 2, Price: 3.1, Total: 0";

			// 因为 Total 属性有 HttpValueIgnoreAttribute 标记，所以它应该不能接收数据。
			Assert.AreEqual(expect, result);
		}



	}
}
