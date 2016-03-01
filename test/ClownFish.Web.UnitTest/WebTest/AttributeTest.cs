using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.WebTest
{
	[TestClass]
	public class AttributeTest : BaseTest
	{

		[TestMethod]
		public void Test_HttpValueIgnoreAttribute()
		{
			string requestText = @"
POST http://www.fish-mvc-demo.com/Ajax/test/Attribute/GetTotal.aspx HTTP/1.1
X-Result-Format: FORM

Count=2&Price=3.1&Total=100.23
";
			string result = ExecuteService(requestText);

			// 因为 Total 属性有 HttpValueIgnoreAttribute 标记，所以它应该不能接收数据。
			Assert.AreEqual("Count=2&Price=3.1&Total=0", result);
		}


		






	}
}
