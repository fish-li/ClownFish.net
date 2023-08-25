using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Base.Http;
using ClownFish.Base.WebClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.WebClient
{
	[TestClass]
	public class FormDataCollectionTest
	{
		[TestMethod]
		public void Test_Add()
        {
			FormDataCollection form = new FormDataCollection();
			form.AddString("a", "11");
			form.AddString("b", null);
			form.AddObject("c", 22);
			form.AddObject("d", null);

			MyAssert.IsError<ArgumentNullException>(() => {
				form.AddString("", "22");
			});

			MyAssert.IsError<ArgumentNullException>(() => {
				form.AddObject("", 33);
			});

			string text = form.ToString();
			Assert.AreEqual("a=11&b=&c=22&d=", text);

			MyAssert.IsError<ArgumentNullException>(() => {
				form.WriteToStream(null, Encoding.UTF8);
			});

			MyAssert.IsError<ArgumentNullException>(() => {
				form.WriteToStream(new MemoryStream(), null);
			});

			using(MemoryStream ms = new MemoryStream() ) {
				form.WriteToStream(ms, Encoding.UTF8);

				string text2 = Encoding.UTF8.GetString(ms.ToArray());
				Assert.AreEqual("a=11&b=&c=22&d=", text);
			}
		}

		[TestMethod]
		public void Test_ToString()
		{
			var data = new { a = 1, b = 2, c = "xyz中文汉字" };

			FormDataCollection form = FormDataCollection.Create(data);
			string actual = form.ToString();
			Assert.AreEqual("a=1&b=2&c=xyz%e4%b8%ad%e6%96%87%e6%b1%89%e5%ad%97", actual);

			MyAssert.IsError<InvalidOperationException>(()=> {
				_ = form.GetMultipartContentType();
			});
		}


		[TestMethod]
		public void Test_IDictionary()
		{
			Hashtable table = new Hashtable();
			table["a"] = 1;
			table["b"] = 2;
			table["c"] = "xyz中文汉字";

			FormDataCollection form = FormDataCollection.Create(table);
			string actual = form.ToString();
            //Console.WriteLine(actual);

            //Assert.AreEqual("a=1&b=2&c=xyz%e4%b8%ad%e6%96%87%e6%b1%89%e5%ad%97", actual);

            // 没法保证3个key的先后次序
            Assert.IsTrue(actual.IndexOf("a=1") >= 0);
            Assert.IsTrue(actual.IndexOf("b=2") >= 0);
            Assert.IsTrue(actual.IndexOf("c=xyz%e4%b8%ad%e6%96%87%e6%b1%89%e5%ad%97") >= 0);
        }

		[TestMethod]
		public void Test_NameValueCollection()
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
		public void Test_AddObject()
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
		public void Test_AddObject_bytes()
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
		public void Test_AddObject_File_Error()
		{
			MyAssert.IsError<ArgumentNullException>(() => {
				_ = HttpFile.CreateFromFileInfo(null);
			});

			MyAssert.IsError<ArgumentNullException>(() => {
				HttpFile file = new HttpFile();
				file.Validate();
			});

			MyAssert.IsError<ArgumentNullException>(() => {
				HttpFile file = new HttpFile();
				file.FileName = "xx.dat";
				file.Validate();
			});


			FormDataCollection form = new FormDataCollection();

			MyAssert.IsError<ArgumentNullException>(() => {
				form.AddFile(null, new HttpFile {
					FileName = "c:\\xx.dat",
					FileBody = new Byte[] { 2, 3, 5 }
				});
			});

			MyAssert.IsError<ArgumentNullException>(() => {
				form.AddFile("file0", (HttpFile)null);
			});

			MyAssert.IsError<ArgumentNullException>(() => {
				form.AddFile(null, new FileInfo("ClownFish.App.config"));
			});

			MyAssert.IsError<ArgumentNullException>(() => {
				form.AddFile("file0", (FileInfo)null);
			});


			MyAssert.IsError<ArgumentNullException>(() => {
				form.AddObject("x", new HttpFile {
					FileName = "c:\\xx.dat",
					FileBody = null
				});
			});
		}


		private static readonly string s_fileText = "POST的请求包中，form-data中媒体文件标识，应包含有 filename、filelength、content-type等信息";

		[TestMethod]
		public void Test_AddObject_File1()
		{
			string filePath = Path.Combine(AppDomain.CurrentDomain.GetTempPath(), "Test_FormDataCollection_AddObject_bytes.txt");
			byte[] bb = Encoding.UTF8.GetBytes(s_fileText);
			RetryFile.WriteAllBytes(filePath, bb);


			FormDataCollection form = new FormDataCollection();
			form.AddObject("a", new byte[] { 1, 2, 3, 4, 5 });
			form.AddString("b", "xyz中文汉字");
			
			form.AddObject("file1", new FileInfo(filePath));

			string md5 = GetFormDataCollectionMd5(form);

			Assert.IsTrue(form.HasFile);
			Assert.AreEqual("a482f39f658fa5a8fcc7dcfb7fb32301", md5);
		}

		private string GetFormDataCollectionMd5(FormDataCollection form)
        {
			using( MemoryStream ms = new MemoryStream() ) {
				form.WriteToStream(ms, Encoding.UTF8);

				ms.Position = 0;
				byte[] buffer = ms.ToArray();

				string s = Encoding.UTF8.GetString(buffer);
				Console.WriteLine(s);

				// byte 数组太难写断言，所以就计算 MD5 来比较
				byte[] bb = MD5.Create().ComputeHash(buffer);
				return bb.ToHexString().ToLower();
			}
		}

		[TestMethod]
		public void Test_AddObject_File2()
		{
			FormDataCollection form = new FormDataCollection();
			form.AddObject("a", new byte[] { 1, 2, 3, 4, 5 });
			form.AddString("b", "xyz中文汉字");

			form.AddObject("file1", new HttpFile {
				FileName = @"c:\xxx\Test_FormDataCollection_AddObject_bytes.txt",
				FileBody = Encoding.UTF8.GetBytes(s_fileText)
			});

			string md5 = GetFormDataCollectionMd5(form);

			Assert.IsTrue(form.HasFile);
			Assert.AreEqual("a482f39f658fa5a8fcc7dcfb7fb32301", md5);
		}


		[TestMethod]
		public void Test_AddObject_File3()
		{
			FormDataCollection form = new FormDataCollection();
			form.AddObject("a", new byte[] { 1, 2, 3, 4, 5 });
			form.AddString("b", "xyz中文汉字");

			form.AddObject("file1", new HttpFile {
				FileName = @"c:\xxx\Test_FormDataCollection_AddObject_bytes.txt",
                BodyStream = new MemoryStream(Encoding.UTF8.GetBytes(s_fileText))
			});

			string md5 = GetFormDataCollectionMd5(form);

			Assert.IsTrue(form.HasFile);
			Assert.AreEqual("a482f39f658fa5a8fcc7dcfb7fb32301", md5);
		}

		[TestMethod]
		public void Test_AddObject_File4()
		{
			dynamic data = new ExpandoObject();
			data.a = new byte[] { 1, 2, 3, 4, 5 };
			data.b = "xyz中文汉字";
			data.file1 = new HttpFile {
				FileName = @"c:\xxx\Test_FormDataCollection_AddObject_bytes.txt",
                BodyStream = new MemoryStream(Encoding.UTF8.GetBytes(s_fileText))
			};


			FormDataCollection form = FormDataCollection.Create(data);

			string md5 = GetFormDataCollectionMd5(form);

			Assert.IsTrue(form.HasFile);
			Assert.AreEqual("a482f39f658fa5a8fcc7dcfb7fb32301", md5);
		}

		[TestMethod]
		public void Test_GetQueryString()
        {
			var data = new {
				a = 11,
				b = "abc",
				c = (string)null,
				d = new string[] { "22", null, "33" }
			};

			string text = FormDataCollection.GetQueryString(data);
			Assert.AreEqual("a=11&b=abc&c=&d=22&d=&d=33", text);

			FormDataCollection form1 = FormDataCollection.Create(data);
			FormDataCollection form2 = FormDataCollection.Create(form1);

			Assert.IsTrue(object.ReferenceEquals(form1, form2));
		}

		[TestMethod]
		public void Test_Create_Error()
        {
			MyAssert.IsError<ArgumentNullException>(() => {
				_ = FormDataCollection.Create((object)null);
			});

			MyAssert.IsError<ArgumentException>(() => {
				_ = FormDataCollection.Create(123);
			});

			MyAssert.IsError<ArgumentException>(() => {
				_ = FormDataCollection.Create("xxx");
			});
		}
	}
}
