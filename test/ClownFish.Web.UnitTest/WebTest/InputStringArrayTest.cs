using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.WebTest
{
	public class InputStringArrayTest : BaseTest
	{

		[TestMethod]
		public void Input_string_array()
		{
			string requestText = @"
POST http://www.fish-mvc-demo.com/Ajax/test/DataTypeTest/Input_string_array.aspx HTTP/1.1

array=aa&array=bb&array=cc
";
			string result = ExecuteService(requestText);
			Assert.AreEqual("aa-bb-cc", result);
		}


		[TestMethod]
		public void Input_string_array2()
		{
			string requestText = @"
GET http://www.fish-mvc-demo.com/Ajax/test/DataTypeTest/Input_string_array.aspx?array=aa&array=bb&array=cc HTTP/1.1
";
			string result = ExecuteService(requestText);
			Assert.AreEqual("aa-bb-cc", result);
		}


		[TestMethod]
		public void Input_string_array3()
		{
			string requestText = @"
GET http://www.fish-mvc-demo.com/Ajax/test/DataTypeTest/Input_string_array.aspx?array=aa HTTP/1.1

array=bb&array=cc
";
			// 注意：Action参数 优先采用查询字符串的输入。

			string result = ExecuteService(requestText);
			Assert.AreEqual("aa", result);
		}


		[TestMethod]
		public void Input_string_array4()
		{
			string requestText = @"
GET http://www.fish-mvc-demo.com/Ajax/test/DataTypeTest/Input_string_array.aspx HTTP/1.1
";
			string result = ExecuteService(requestText);
			Assert.AreEqual("NULL", result);
		}



	}
}
