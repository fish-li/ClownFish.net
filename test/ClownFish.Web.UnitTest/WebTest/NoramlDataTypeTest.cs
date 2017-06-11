using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.WebTest
{
	[TestClass]
	public class NoramlDataTypeTest : BaseTest
	{

		[TestMethod]
		public void Input_string_ToUpper()
		{
			string requestText = @"
GET http://www.fish-web-demo.com/Ajax/test/DataTypeTest/Input_string_ToUpper.aspx?input=fish HTTP/1.1
";
			string result = ExecuteService(requestText);
			Assert.AreEqual("FISH", result);
		}


		[TestMethod]
		public void Input_string_ToUpper2()
		{
			string requestText = @"
GET http://www.fish-web-demo.com/Ajax/test/DataTypeTest/Input_string_ToUpper.aspx?input= HTTP/1.1
";
			string result = ExecuteService(requestText);
			Assert.AreEqual(string.Empty, result);
		}

		[TestMethod]
		public void Input_string_ToUpper3()
		{
			string requestText = @"
GET http://www.fish-web-demo.com/Ajax/test/DataTypeTest/Input_string_ToUpper.aspx HTTP/1.1
";
			string result = ExecuteService(requestText);
			Assert.AreEqual("input is empty.".ToUpper(), result);
		}


		[TestMethod]
		public void Input_int_Add()
		{
			string requestText = @"
POST http://www.fish-web-demo.com/Ajax/test/DataTypeTest/Input_int_Add.aspx HTTP/1.1

a=3&b=5
";
			string result = ExecuteService(requestText);
			Assert.AreEqual("8", result);
		}


		[ExpectedException(typeof(ArgumentException))]
		[TestMethod]
		public void Input_int_Add2()
		{
			string requestText = @"
POST http://www.fish-web-demo.com/Ajax/test/DataTypeTest/Input_int_Add.aspx HTTP/1.1

a=3
";
			string result = ExecuteService(requestText);
			// System.ArgumentException: 未能找到指定的参数值：b
		}



		[TestMethod]
		public void Input_decimal_Add()
		{
			string requestText = @"
POST http://www.fish-web-demo.com/Ajax/test/DataTypeTest/Input_decimal_Add.aspx HTTP/1.1

a=3.1&b=5.2
";
			string result = ExecuteService(requestText);
			Assert.AreEqual("8.3", result);
		}


		[TestMethod]
		public void Input_number_Add()
		{
			string requestText = @"
POST http://www.fish-web-demo.com/Ajax/test/DataTypeTest/Input_number_Add.aspx HTTP/1.1

a=2&b=3&c=3.5&d=4.7
";
			string result = ExecuteService(requestText);
			Assert.AreEqual("13.2", result);
		}




		[TestMethod]
		public void Input_DateTime()
		{
			string requestText = @"
POST http://www.fish-web-demo.com/Ajax/test/DataTypeTest/Input_DateTime.aspx HTTP/1.1

dt=2015-05-03
";
			string result = ExecuteService(requestText);
			Assert.AreEqual("20150503", result);
		}






		[TestMethod]
		public void Input_bool()
		{
			string requestText = @"
POST http://www.fish-web-demo.com/Ajax/test/DataTypeTest/Input_bool.aspx HTTP/1.1

b=true";

			string result = ExecuteService(requestText);
			Assert.AreEqual("true", result);
		}


		[TestMethod]
		public void Input_bool2()
		{
			string requestText = @"
POST http://www.fish-web-demo.com/Ajax/test/DataTypeTest/Input_bool.aspx HTTP/1.1

b=false";

			string result = ExecuteService(requestText);
			Assert.AreEqual("false", result);
		}


		[ExpectedException(typeof(InvalidCastException))]
		[TestMethod]
		public void Input_bool3()
		{
			string requestText = @"
POST http://www.fish-web-demo.com/Ajax/test/DataTypeTest/Input_bool.aspx HTTP/1.1

b=1";

			string result = ExecuteService(requestText);
		}






	}
}
