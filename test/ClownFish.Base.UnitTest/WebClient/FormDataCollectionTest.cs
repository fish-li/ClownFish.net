using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Http;
using ClownFish.Base.WebClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Base.UnitTest.WebClient
{
	[TestClass]
	public class FormDataCollectionTest
	{
		[TestMethod]
		public void Test_FormDataCollection_object_ToString()
		{
			var data = new { a = 1, b = 2, c = "xyz中文汉字" };

			FormDataCollection form = FormDataCollection.Create(data);
			string actual = form.ToString();
			Assert.AreEqual("a=1&b=2&c=xyz%e4%b8%ad%e6%96%87%e6%b1%89%e5%ad%97", actual);
		}


		[TestMethod]
		public void Test_FormDataCollection_IDictionary_ToString()
		{
			Hashtable table = new Hashtable();
			table["a"] = 1;
			table["b"] = 2;
			table["c"] = "xyz中文汉字";

			FormDataCollection form = FormDataCollection.Create(table);
			string actual = form.ToString();
			Assert.AreEqual("a=1&b=2&c=xyz%e4%b8%ad%e6%96%87%e6%b1%89%e5%ad%97", actual);
		}

		[TestMethod]
		public void Test_FormDataCollection_NameValueCollection_ToString()
		{
			NameValueCollection collection = new NameValueCollection();
			collection["a"] = "1";
			collection["b"] = "2";

			// 允许一个名称对应多个值
			collection["c"] = "xyz中文汉字";
			collection.Add("c", "789");


			FormDataCollection form = FormDataCollection.Create(collection);
			string actual = form.ToString();
			Console.WriteLine(actual);
			Assert.AreEqual("a=1&b=2&c=xyz%e4%b8%ad%e6%96%87%e6%b1%89%e5%ad%97&c=789", actual);
		}


		[TestMethod]
		public void Test_FormDataCollection_AddObject()
		{
			FormDataCollection form = new FormDataCollection();
			form.AddObject("a", 1);
			form.AddObject("b", 2);

			form.AddObject("c", "xyz中文汉字");
			form.AddString("c", "789");

			string actual = form.ToString();
			Console.WriteLine(actual);
			Assert.AreEqual("a=1&b=2&c=xyz%e4%b8%ad%e6%96%87%e6%b1%89%e5%ad%97&c=789", actual);
		}

		[TestMethod]
		public void Test_FormDataCollection_AddObject_bytes()
		{
			FormDataCollection form = new FormDataCollection();
			form.AddObject("a", new byte[] { 1, 2, 3, 4, 5 });
			form.AddString("b", "xyz中文汉字");
			
			string actual = null;

			using( MemoryStream ms = new MemoryStream() ) {
				form.WriteToStream(ms, Encoding.UTF8);
				ms.Position = 0;
				byte[] buffer = ms.ToArray();

				actual = Encoding.UTF8.GetString(buffer);
			}

			Assert.IsFalse(form.HasFile);
			Assert.AreEqual("a=AQIDBAU%3d&b=xyz%e4%b8%ad%e6%96%87%e6%b1%89%e5%ad%97", actual);
		}


		[TestMethod]
		public void Test_FormDataCollection_AddObject_File()
		{
			string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test_FormDataCollection_AddObject_bytes.txt");
			File.WriteAllText(filePath, "不能直接重载 -= 运算符", Encoding.UTF8);

			FormDataCollection form = new FormDataCollection();
			form.AddObject("a", new byte[] { 1, 2, 3, 4, 5 });
			form.AddString("b", "xyz中文汉字");
			form.AddObject("c", new FileInfo(filePath));

			form.AddObject("d", new HttpFile {
				FileName = "c:\\test1.dat",
				FileBody = Encoding.UTF8.GetBytes("XML 的输出文件，由编译的源代码文件中的注释填充")
			});



			string md5 = null;

			using(MemoryStream ms = new MemoryStream() ) {
				form.WriteToStream(ms, Encoding.UTF8);
				ms.Position = 0;
				byte[] buffer = ms.ToArray();

				string s = Encoding.UTF8.GetString(buffer);
				Console.WriteLine(s);

				// byte 数组太难写断言，所以就计算 MD5 来比较
				byte[] bb = (new MD5CryptoServiceProvider()).ComputeHash(buffer);
				md5 = BitConverter.ToString(bb).Replace("-", "").ToLower();
			}

			Assert.IsTrue(form.HasFile);
			Assert.AreEqual("fbf3628c4d1415ace6e56084d3edcc18", md5);
		}

	}
}
