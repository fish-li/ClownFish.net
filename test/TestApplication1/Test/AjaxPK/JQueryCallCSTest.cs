using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Web;
using ClownFish.Web.Client;
using ClownFish.TestApplication1.Common;
using ClownFish.Base;

namespace ClownFish.TestApplication1.Test.AjaxPK
{
	class JQueryCallCSTest : TestBase
	{
		[TestMethod("AJAX PK：Jquery直接调用 .net 方法, 1 + 2 GET")]
		public async Task Test1()
		{
			HttpOption option = new HttpOption {
				Url = "http://www.fish-mvc-demo.com/ajax/pk/DemoPk/Add.cspx",
				Method = "GET",
				Data = new { a = 1, b = 2 }
			};

			string actual = await option.SendAsync<string>();
			string expected = "13";
			Assert.AreEqual(expected, actual);
		}


		[TestMethod("AJAX PK：Jquery直接调用 .net 方法, 1 + 2 POST")]
		public async Task Test2()
		{
			HttpOption option = new HttpOption {
				Url = "http://www.fish-mvc-demo.com/ajax/pk/DemoPk/Add.cspx",
				Method = "POST",
				Data = new { a = 1, b = 2 }
			};

			string actual = await option.SendAsync<string>();
			string expected = "3";
			Assert.AreEqual(expected, actual);
		}


		[TestMethod("AJAX PK：Jquery直接调用 .net 方法, Save Customer Info")]
		public async Task Test3()
		{
			HttpOption option = new HttpOption {
				Url = "http://www.fish-mvc-demo.com/ajax/pk/DemoPk/AddCustomer.cspx",
				Method = "POST",
				Data = new { Address = "武汉", Age = 20, Email = "test@163.com", Name = "abc", Tel = "12345678" }
			};

			string actual = await option.SendAsync<string>();
			string expected = @"
    <Name>abc</Name>
    <Age>20</Age>
    <Address>武汉</Address>
    <Tel>12345678</Tel>
    <Email>test@163.com</Email>
</Customer>".Trim();
			Assert.IsTrue(actual.EndsWith(expected));
		}


		[TestMethod("AJAX PK：批量输入控件的提交, -1")]
		public async Task Test4()
		{
			await Test4Internal("http://www.fish-mvc-demo.com/ajax/pk/DemoPk/BatchAddCustomer.cspx");
		}


		[TestMethod("AJAX PK：批量输入控件的提交, -2")]
		public async Task Test4b()
		{
			await Test4Internal("http://www.fish-mvc-demo.com/ajax/pk/DemoPk/BatchAddCustomer2.cspx");
		}


		private async Task Test4Internal(string url)
		{
			string input = @"
Address=武汉
Address=上海
Address=武汉
Address=
Address=
Age=20
Age=20
Age=20
Age=
Age=
Email=test1@163.com
Email=
Email=test3@163.com
Email=
Email=
Name=A1
Name=A2
Name=A3
Name=
Name=A5
Tel=12345678
Tel=22222222
Tel=
Tel=
Tel=12345678
batchAddCustomer=保存客户资料";

			HttpOption option = new HttpOption {
				Url = url,
				Method = "POST",
				Data = StringToFormDataCollection(input)
			};

			string actual = await option.SendAsync<string>();
			string expected = @"
    <Customer>
        <Name>A1</Name>
        <Age>20</Age>
        <Address>武汉</Address>
        <Tel>12345678</Tel>
        <Email>test1@163.com</Email>
    </Customer>
    <Customer>
        <Name>A2</Name>
        <Age>20</Age>
        <Address>上海</Address>
        <Tel>22222222</Tel>
        <Email />
    </Customer>
</ArrayOfCustomer>".Trim();
			Assert.IsTrue(actual.EndsWith(expected));
		}


		private FormDataCollection StringToFormDataCollection(string firebugCopyText)
		{
			// 将 FireBug中复制的文本，转变成提交数据

			string[] lines = firebugCopyText.SplitTrim('\r', '\n');
			//if( lines.Length == 0 || lines.Length % 2 != 0 )
			//	throw new ArgumentException("无效的输入数据：firebugCopyText");

			FormDataCollection collection = new FormDataCollection();

			foreach( string line in lines ) {
				string[] nn = line.SplitTrim('=');
				if( nn.Length == 2 && nn[1].Length > 0 )
					collection.AddString(nn[0], nn[1]);
				else
					collection.AddString(nn[0], string.Empty);
			}

			return collection;
		}

	}
}
