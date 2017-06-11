using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.WebTest
{
	// 测试：支持可空类型

	[TestClass]
	public class InputNullableTest : BaseTest
	{


		[TestMethod]
		public void Input_int_nullable_Add()
		{
			string requestText = @"
POST http://www.fish-web-demo.com/Ajax/test/DataTypeTest/Input_int_nullable_Add.aspx HTTP/1.1

a=3
";
			string result = ExecuteService(requestText);
			Assert.AreEqual("3", result);
		}


		[TestMethod]
		public void Input_int_nullable_Add2()
		{
			string requestText = @"
POST http://www.fish-web-demo.com/Ajax/test/DataTypeTest/Input_int_nullable_Add.aspx HTTP/1.1

b=5
";
			string result = ExecuteService(requestText);
			Assert.AreEqual("5", result);
		}


		[TestMethod]
		public void Input_int_nullable_Add3()
		{
			string requestText = @"
GET http://www.fish-web-demo.com/Ajax/test/DataTypeTest/Input_int_nullable_Add.aspx HTTP/1.1
";
			string result = ExecuteService(requestText);
			Assert.AreEqual("0", result);
		}



		[TestMethod]
		public void Input_decimal_nullable_Add()
		{
			string requestText = @"
POST http://www.fish-web-demo.com/Ajax/test/DataTypeTest/Input_decimal_nullable_Add.aspx HTTP/1.1

a=3.1
";
			string result = ExecuteService(requestText);
			Assert.AreEqual("3.1", result);
		}


		[TestMethod]
		public void Input_decimal_nullable_Add2()
		{
			string requestText = @"
POST http://www.fish-web-demo.com/Ajax/test/DataTypeTest/Input_decimal_nullable_Add.aspx HTTP/1.1

b=5.2
";
			string result = ExecuteService(requestText);
			Assert.AreEqual("5.2", result);
		}


		[TestMethod]
		public void Input_decimal_nullable_Add3()
		{
			string requestText = @"
GET http://www.fish-web-demo.com/Ajax/test/DataTypeTest/Input_decimal_nullable_Add.aspx HTTP/1.1
";
			string result = ExecuteService(requestText);
			Assert.AreEqual("0", result);
		}


		[TestMethod]
		public void Input_number_nullable_Add()
		{
			string requestText = @"
POST http://www.fish-web-demo.com/Ajax/test/DataTypeTest/Input_number_nullable_Add.aspx?b=3 HTTP/1.1

d=3.5
";
			string result = ExecuteService(requestText);
			Assert.AreEqual("6.5", result);
		}


		[TestMethod]
		public void Input_DateTime_nullable()
		{
			string requestText = @"
GET http://www.fish-web-demo.com/Ajax/test/DataTypeTest/Input_DateTime_nullable.aspx HTTP/1.1
";
			string result = ExecuteService(requestText);
			Assert.AreEqual("00010101", result);
		}

	}
}
