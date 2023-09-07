using ClownFish.UnitTest.Base;

namespace ClownFish.UnitTest.WebClient;

[TestClass]
	public class RequestWriterTest
	{
		[TestMethod]
		public void Test_Write_Null()
    {
			RequestWriter writer = new RequestWriter();
			writer.Write(null, "xx", SerializeFormat.Text);
			Assert.IsNull(writer.ContentType);


			writer.Write(new MemoryStream(), null, SerializeFormat.Text);
			Assert.IsNull(writer.ContentType);
		}

		[TestMethod]
		public void Test_Write_Text()
		{
			var data = new { a = 1, b = 2, c = "xyz中文汉字" };
			var actual = WriteStream1(data, SerializeFormat.Text);

			Assert.AreEqual("text/plain", actual.ContentType);
			Assert.AreEqual("{ a = 1, b = 2, c = xyz中文汉字 }", actual.Body);
		}
		

		[TestMethod]
		public void Test_Write_Json()
		{
			string json = @"{""a"":1,""b"":2,""c"":""xyz中文汉字""}";
			var data = new { a = 1, b = 2, c = "xyz中文汉字" };

			var actual = WriteStream1(data, SerializeFormat.Json);
			Assert.AreEqual("application/json", actual.ContentType);
			Assert.AreEqual(json, actual.Body);


			var actual2 = WriteStream1(data, SerializeFormat.Json2);
			Assert.AreEqual("application/json", actual2.ContentType);
			Assert.AreEqual(json, actual2.Body);


			var actual3 = WriteStream1(json, SerializeFormat.Json);
			Assert.AreEqual("application/json", actual3.ContentType);
			Assert.AreEqual(json, actual3.Body);
		}

		[TestMethod]
		public void Test_Write_Xml()
		{
			Product2 p = new Product2 { ProductID = 2, ProductName = "abc" };
			string xml = p.ToXml();

			var actual = WriteStream1(p, SerializeFormat.Xml);
			Assert.AreEqual("application/xml", actual.ContentType);
			Assert.AreEqual(xml, actual.Body);


			var actual2 = WriteStream1(xml, SerializeFormat.Xml);
			Assert.AreEqual("application/xml", actual2.ContentType);
			Assert.AreEqual(xml, actual2.Body);
		}

		[TestMethod]
		public void Test_Write_Form()
		{
			string text = "a=1&b=2&c=xyz%e4%b8%ad%e6%96%87%e6%b1%89%e5%ad%97";
			var data = new { a = 1, b = 2, c = "xyz中文汉字" };


			var actual = WriteStream1(data, SerializeFormat.Form);
			Assert.AreEqual("application/x-www-form-urlencoded", actual.ContentType);
			Assert.AreEqual(text, actual.Body);


			var actual2 = WriteStream1(text, SerializeFormat.Form);
			Assert.AreEqual("application/x-www-form-urlencoded", actual2.ContentType);
			Assert.AreEqual(text, actual2.Body);
		}


		[TestMethod]
		public void Test_Write_Form_Text()
		{
			var data = "a=1&b=2&c=xyz%e4%b8%ad%e6%96%87%e6%b1%89%e5%ad%97";
			var actual = WriteStream1(data, SerializeFormat.Form);

			Assert.AreEqual("application/x-www-form-urlencoded", actual.ContentType);
			Assert.AreEqual("a=1&b=2&c=xyz%e4%b8%ad%e6%96%87%e6%b1%89%e5%ad%97", actual.Body);
		}

		[TestMethod]
		public void Test_Write_Empty_Text()
		{
			string data = string.Empty;
			var actual = WriteStream2(data, SerializeFormat.Text);
			Assert.AreEqual(RequestContentType.Text, actual.ContentType);
			Assert.AreEqual(0, actual.Body.Length);
		}


		[TestMethod]
		public void Test_Write_Empty_Bytes()
    {
			byte[] data = Empty.Array<byte>();
			var actual = WriteStream2(data, SerializeFormat.Binary);
			Assert.AreEqual(RequestContentType.Binary, actual.ContentType);
        Assert.AreEqual(0, actual.Body.Length);
    }


		

		[TestMethod]
		public void Test_Write_Binary()
		{
			Guid guid = new Guid("994b07c4-068f-4b76-afad-c457cc5b8473");
			var data = guid.ToByteArray();

			var actual = WriteStream2(data, SerializeFormat.Binary);
			Assert.AreEqual(RequestContentType.Binary, actual.ContentType);
        MyAssert.AreEqual(data, actual.Body);


			var data2 = new MemoryStream(data);
			var actual2 = WriteStream2(data2, SerializeFormat.Binary);
			Assert.AreEqual(RequestContentType.Binary, actual2.ContentType);
			MyAssert.AreEqual(data, actual2.Body);
		}


		[TestMethod]
		public void Test_Write_Direct_Text()
		{
			var data = "xyz中文汉字";
			var actual = WriteStream1(data, SerializeFormat.None);

			Assert.IsNull(actual.ContentType);
			Assert.AreEqual(data, actual.Body);
		}

		[TestMethod]
		public void Test_Write_Direct_Bytes()
		{
			Guid guid = new Guid("994b07c4-068f-4b76-afad-c457cc5b8473");
			byte[] bytes = guid.ToByteArray();

			var actual = WriteStream2(bytes, SerializeFormat.None);

			Assert.IsNull(actual.ContentType);
        MyAssert.AreEqual(bytes, actual.Body);
		}


		[TestMethod]
		public void Test_Write_Direct_Stream()
		{
			Guid guid = new Guid("994b07c4-068f-4b76-afad-c457cc5b8473");
			byte[] bytes = guid.ToByteArray();

			var data = new MemoryStream(bytes);

			var actual = WriteStream2(data, SerializeFormat.None);

			Assert.IsNull(actual.ContentType);
        MyAssert.AreEqual(bytes, actual.Body);
		}


		[TestMethod]
		public void Test_Write_NotSupportedException()
		{
			var data = new {
				a = "11",
				b = 22
			};

			MyAssert.IsError<NotSupportedException>(() => {
				_= WriteStream1(data, SerializeFormat.None);
			});

			MyAssert.IsError<NotSupportedException>(() => {
				_ = WriteStream1(data, SerializeFormat.Binary);
			});
		}

		private (string ContentType, string Body) WriteStream1(object data, SerializeFormat format)
		{
			RequestWriter writer = new RequestWriter();

			using(MemoryStream ms = new MemoryStream() ) {
				writer.Write(ms, data, format);

				ms.Position = 0;
				byte[] bytes = ms.ToArray();

				string contentType = writer.ContentType;
				string body = Encoding.UTF8.GetString(bytes);

				return (contentType, body);
			}
		}

		private (string ContentType, byte[] Body) WriteStream2(object data, SerializeFormat format)
		{
			RequestWriter writer = new RequestWriter();

			using( MemoryStream ms = new MemoryStream() ) {
				writer.Write(ms, data, format);

				ms.Position = 0;
				byte[] bytes = ms.ToArray();

				string contentType = writer.ContentType;

				return (contentType, bytes);
			}
		}
	}
