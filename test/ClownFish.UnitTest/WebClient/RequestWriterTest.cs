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

        Assert.AreEqual("text/plain; charset=utf-8", actual.ContentType);
        Assert.AreEqual("{ a = 1, b = 2, c = xyz中文汉字 }", actual.Body);
    }


    [TestMethod]
    public void Test_Write_Json()
    {
        string json = @"{""a"":1,""b"":2,""c"":""xyz中文汉字""}";
        var data = new { a = 1, b = 2, c = "xyz中文汉字" };

        var actual = WriteStream1(data, SerializeFormat.Json);
        Assert.AreEqual("application/json; charset=utf-8", actual.ContentType);
        Assert.AreEqual(json, actual.Body);


        var actual2 = WriteStream1(data, SerializeFormat.Json2);
        Assert.AreEqual("application/json; charset=utf-8", actual2.ContentType);
        Assert.AreEqual(json, actual2.Body);


        var actual3 = WriteStream1(json, SerializeFormat.Json);
        Assert.AreEqual("application/json; charset=utf-8", actual3.ContentType);
        Assert.AreEqual(json, actual3.Body);
    }

    [TestMethod]
    public void Test_Write_Xml()
    {
        Product3 p = new Product3 { ProductID = 2, ProductName = "abc" };
        string xml = p.ToXml();

        var actual = WriteStream1(p, SerializeFormat.Xml);
        Assert.AreEqual("application/xml; charset=utf-8", actual.ContentType);
        Assert.AreEqual(xml, actual.Body);


        var actual2 = WriteStream1(xml, SerializeFormat.Xml);
        Assert.AreEqual("application/xml; charset=utf-8", actual2.ContentType);
        Assert.AreEqual(xml, actual2.Body);
    }

    [TestMethod]
    public void Test_Write_Form()
    {
        string text = "a=1&b=2&c=xyz%e4%b8%ad%e6%96%87%e6%b1%89%e5%ad%97";
        var data = new { a = 1, b = 2, c = "xyz中文汉字" };


        var actual = WriteStream1(data, SerializeFormat.Form);
        Assert.AreEqual("application/x-www-form-urlencoded; charset=utf-8", actual.ContentType);
        Assert.AreEqual(text, actual.Body);


        var actual2 = WriteStream1(text, SerializeFormat.Form);
        Assert.AreEqual("application/x-www-form-urlencoded; charset=utf-8", actual2.ContentType);
        Assert.AreEqual(text, actual2.Body);
    }


    [TestMethod]
    public void Test_Write_Form_Text()
    {
        var data = "a=1&b=2&c=xyz%e4%b8%ad%e6%96%87%e6%b1%89%e5%ad%97";
        var actual = WriteStream1(data, SerializeFormat.Form);

        Assert.AreEqual("application/x-www-form-urlencoded; charset=utf-8", actual.ContentType);
        Assert.AreEqual("a=1&b=2&c=xyz%e4%b8%ad%e6%96%87%e6%b1%89%e5%ad%97", actual.Body);
    }

    [TestMethod]
    public void Test_Write_Empty_Text()
    {
        string data = string.Empty;
        var actual = WriteStream2(data, SerializeFormat.Text);
        Assert.AreEqual("text/plain; charset=utf-8", actual.ContentType);
        Assert.AreEqual(0, actual.Body.Length);
    }


    [TestMethod]
    public void Test_Write_Empty_Bytes()
    {
        byte[] data = Empty.Array<byte>();
        var actual = WriteStream2(data, SerializeFormat.Binary);
        Assert.AreEqual("application/octet-stream", actual.ContentType);
        Assert.AreEqual(0, actual.Body.Length);
    }




    [TestMethod]
    public void Test_Write_Binary()
    {
        Guid guid = new Guid("994b07c4-068f-4b76-afad-c457cc5b8473");
        var data = guid.ToByteArray();

        var actual = WriteStream2(data, SerializeFormat.Binary);
        Assert.AreEqual("application/octet-stream", actual.ContentType);
        MyAssert.AreEqual(data, actual.Body);


        var data2 = new MemoryStream(data);
        var actual2 = WriteStream2(data2, SerializeFormat.Binary);
        Assert.AreEqual("application/octet-stream", actual2.ContentType);
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
            _ = WriteStream1(data, SerializeFormat.None);
        });

        MyAssert.IsError<NotSupportedException>(() => {
            _ = WriteStream1(data, SerializeFormat.Binary);
        });
    }

    private (string ContentType, string Body) WriteStream1(object data, SerializeFormat format)
    {
        RequestWriter writer = new RequestWriter();

        using( MemoryStream ms = new MemoryStream() ) {
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


    [TestMethod]
    public void Test_Gzip_Text_2048()
    {
        using MemoryStream ms = new MemoryStream();
        string text = new string('中', 5000);

        RequestWriter writer = new RequestWriter();
        writer.Write(ms, text, SerializeFormat.Text, true);

        Assert.AreEqual("text/plain; charset=utf-8", writer.ContentType);
        Assert.IsTrue(writer.IsGzip);

        byte[] bytes = ms.ToArray().UnGzip();
        string text2 = Encoding.UTF8.GetString(bytes);
        Assert.AreEqual(text, text2);
    }

    [TestMethod]
    public void Test_Gzip_Text_512()
    {
        using MemoryStream ms = new MemoryStream();
        string text = new string('中', 512);

        RequestWriter writer = new RequestWriter();
        writer.Write(ms, text, SerializeFormat.Text, true);

        Assert.AreEqual("text/plain; charset=utf-8", writer.ContentType);
        Assert.IsFalse(writer.IsGzip);

        byte[] bytes = ms.ToArray();
        string text2 = Encoding.UTF8.GetString(bytes);
        Assert.AreEqual(text, text2);
    }

    [TestMethod]
    public void Test_Gzip_false_Text_2048()
    {
        using MemoryStream ms = new MemoryStream();
        string text = new string('中', 2048);

        RequestWriter writer = new RequestWriter();
        writer.Write(ms, text, SerializeFormat.Text, false);

        Assert.AreEqual("text/plain; charset=utf-8", writer.ContentType);
        Assert.IsFalse(writer.IsGzip);

        byte[] bytes = ms.ToArray();
        string text2 = Encoding.UTF8.GetString(bytes);
        Assert.AreEqual(text, text2);
    }


    [TestMethod]
    public void Test_Gzip_Json_2048()
    {
        using MemoryStream ms = new MemoryStream();
        NameValue data = new NameValue {Name = "abc", Value = new string('中', 2048) };

        RequestWriter writer = new RequestWriter();
        writer.Write(ms, data, SerializeFormat.Json, true);

        Assert.AreEqual("application/json; charset=utf-8", writer.ContentType);
        Assert.IsTrue(writer.IsGzip);

        byte[] bytes = ms.ToArray().UnGzip();
        string text2 = Encoding.UTF8.GetString(bytes);

        NameValue data2 = text2.FromJson<NameValue>();
        Assert.AreEqual(data2.Value, data.Value);
    }

    [TestMethod]
    public void Test_Gzip_Json_512()
    {
        using MemoryStream ms = new MemoryStream();
        NameValue data = new NameValue { Name = "abc", Value = new string('中', 512) };

        RequestWriter writer = new RequestWriter();
        writer.Write(ms, data, SerializeFormat.Json, true);

        Assert.AreEqual("application/json; charset=utf-8", writer.ContentType);
        Assert.IsFalse(writer.IsGzip);

        byte[] bytes = ms.ToArray();
        string text2 = Encoding.UTF8.GetString(bytes);

        NameValue data2 = text2.FromJson<NameValue>();
        Assert.AreEqual(data2.Value, data.Value);
    }

    [TestMethod]
    public void Test_Gzip_Json_Text_2048()
    {
        using MemoryStream ms = new MemoryStream();
        NameValue data = new NameValue { Name = "abc", Value = new string('中', 2048) };

        RequestWriter writer = new RequestWriter();
        writer.Write(ms, data, SerializeFormat.Json, false);

        Assert.AreEqual("application/json; charset=utf-8", writer.ContentType);
        Assert.IsFalse(writer.IsGzip);

        byte[] bytes = ms.ToArray();
        string text2 = Encoding.UTF8.GetString(bytes);

        NameValue data2 = text2.FromJson<NameValue>();
        Assert.AreEqual(data2.Value, data.Value);
    }


    [TestMethod]
    public void Test_Gzip_Json2_2048()
    {
        using MemoryStream ms = new MemoryStream();
        NameValue data = new NameValue { Name = "abc", Value = new string('中', 2048) };

        RequestWriter writer = new RequestWriter();
        writer.Write(ms, data, SerializeFormat.Json2, true);

        Assert.AreEqual("application/json; charset=utf-8", writer.ContentType);
        Assert.IsTrue(writer.IsGzip);

        byte[] bytes = ms.ToArray().UnGzip();
        string text2 = Encoding.UTF8.GetString(bytes);

        NameValue data2 = text2.FromJson<NameValue>();
        Assert.AreEqual(data2.Value, data.Value);
    }

    [TestMethod]
    public void Test_Gzip_Json2_512()
    {
        using MemoryStream ms = new MemoryStream();
        NameValue data = new NameValue { Name = "abc", Value = new string('中', 512) };

        RequestWriter writer = new RequestWriter();
        writer.Write(ms, data, SerializeFormat.Json2, true);

        Assert.AreEqual("application/json; charset=utf-8", writer.ContentType);
        Assert.IsFalse(writer.IsGzip);

        byte[] bytes = ms.ToArray();
        string text2 = Encoding.UTF8.GetString(bytes);

        NameValue data2 = text2.FromJson<NameValue>();
        Assert.AreEqual(data2.Value, data.Value);
    }

    [TestMethod]
    public void Test_Gzip_Json2_Text_2048()
    {
        using MemoryStream ms = new MemoryStream();
        NameValue data = new NameValue { Name = "abc", Value = new string('中', 2048) };

        RequestWriter writer = new RequestWriter();
        writer.Write(ms, data, SerializeFormat.Json2, false);

        Assert.AreEqual("application/json; charset=utf-8", writer.ContentType);
        Assert.IsFalse(writer.IsGzip);

        byte[] bytes = ms.ToArray();
        string text2 = Encoding.UTF8.GetString(bytes);

        NameValue data2 = text2.FromJson<NameValue>();
        Assert.AreEqual(data2.Value, data.Value);
    }



    [TestMethod]
    public void Test_Gzip_Xml_2048()
    {
        using MemoryStream ms = new MemoryStream();
        NameValue data = new NameValue { Name = "abc", Value = new string('中', 2048) };

        RequestWriter writer = new RequestWriter();
        writer.Write(ms, data, SerializeFormat.Xml, true);

        Assert.AreEqual("application/xml; charset=utf-8", writer.ContentType);
        Assert.IsTrue(writer.IsGzip);

        byte[] bytes = ms.ToArray().UnGzip();
        string text2 = Encoding.UTF8.GetString(bytes);

        NameValue data2 = text2.FromXml<NameValue>();
        Assert.AreEqual(data2.Value, data.Value);
    }

    [TestMethod]
    public void Test_Gzip_Xml_512()
    {
        using MemoryStream ms = new MemoryStream();
        NameValue data = new NameValue { Name = "abc", Value = new string('中', 512) };

        RequestWriter writer = new RequestWriter();
        writer.Write(ms, data, SerializeFormat.Xml, true);

        Assert.AreEqual("application/xml; charset=utf-8", writer.ContentType);
        Assert.IsFalse(writer.IsGzip);

        byte[] bytes = ms.ToArray();
        string text2 = Encoding.UTF8.GetString(bytes);

        NameValue data2 = text2.FromXml<NameValue>();
        Assert.AreEqual(data2.Value, data.Value);
    }

    [TestMethod]
    public void Test_Gzip_Xml_Text_2048()
    {
        using MemoryStream ms = new MemoryStream();
        NameValue data = new NameValue { Name = "abc", Value = new string('中', 2048) };

        RequestWriter writer = new RequestWriter();
        writer.Write(ms, data, SerializeFormat.Xml, false);

        Assert.AreEqual("application/xml; charset=utf-8", writer.ContentType);
        Assert.IsFalse(writer.IsGzip);

        byte[] bytes = ms.ToArray();
        string text2 = Encoding.UTF8.GetString(bytes);

        NameValue data2 = text2.FromXml<NameValue>();
        Assert.AreEqual(data2.Value, data.Value);
    }

    [TestMethod]
    public void Test_WriteAsJsonLinesFormat_string()
    {
        List<Product3> list = ResponseReaderTest.CreateTestDataList(9);
        string text = list.ToMultiLineJson();

        MemoryStream ms = new MemoryStream();

        RequestWriter writer = new RequestWriter();
        writer.Write(ms, text, SerializeFormat.JsonLines, false);

        Assert.AreEqual("application/x-ndjson", writer.ContentType);
        Assert.IsFalse(writer.IsGzip);

        string text2 = ms.ToArray().ToUtf8String();
        Assert.AreEqual(text, text2 );
    }

    [TestMethod]
    public void Test_WriteAsJsonLinesFormat_string_gzip()
    {
        List<Product3> list = ResponseReaderTest.CreateTestDataList(100);
        string text = list.ToMultiLineJson();

        MemoryStream ms = new MemoryStream();

        RequestWriter writer = new RequestWriter();
        writer.Write(ms, text, SerializeFormat.JsonLines, true);

        Assert.AreEqual("application/x-ndjson", writer.ContentType);
        Assert.IsTrue(writer.IsGzip);

        string text2 = ms.ToArray().UnGzip().ToUtf8String();
        Assert.AreEqual(text, text2);
    }

    [TestMethod]
    public void Test_WriteAsJsonLinesFormat_list()
    {
        List<Product3> list = ResponseReaderTest.CreateTestDataList(9);

        MemoryStream ms = new MemoryStream();

        RequestWriter writer = new RequestWriter();
        writer.Write(ms, list, SerializeFormat.JsonLines, false);

        Assert.AreEqual("application/x-ndjson", writer.ContentType);
        Assert.IsFalse(writer.IsGzip);

        string text2 = ms.ToArray().ToUtf8String();
        string text = list.ToMultiLineJson();
        Assert.AreEqual(text, text2);
    }

    [TestMethod]
    public void Test_WriteAsJsonLinesFormat_list_gzip()
    {
        List<Product3> list = ResponseReaderTest.CreateTestDataList(100);

        MemoryStream ms = new MemoryStream();

        RequestWriter writer = new RequestWriter();
        writer.Write(ms, list, SerializeFormat.JsonLines, true);

        Assert.AreEqual("application/x-ndjson", writer.ContentType);
        Assert.IsTrue(writer.IsGzip);

        string text2 = ms.ToArray().UnGzip().ToUtf8String();
        string text = list.ToMultiLineJson();
        Assert.AreEqual(text, text2);
    }


    [TestMethod]
    public void Test_WriteAsJsonLinesFormat_list_empty()
    {
        List<Product3> list = new List<Product3>();

        MemoryStream ms = new MemoryStream();

        RequestWriter writer = new RequestWriter();
        writer.Write(ms, list, SerializeFormat.JsonLines, true);

        Assert.AreEqual("application/x-ndjson", writer.ContentType);
        Assert.IsFalse(writer.IsGzip);

        Assert.IsTrue(ms.ToArray().Length == 0);
    }


    [TestMethod]
    public void Test_WriteAsJsonLinesFormat_notlist()
    {
        Product3 p = new Product3();

        MemoryStream ms = new MemoryStream();

        RequestWriter writer = new RequestWriter();

        ArgumentException ex = MyAssert.IsError<ArgumentException>(() => {
            writer.Write(ms, p, SerializeFormat.JsonLines, true);
        });

        Assert.IsTrue(ex.Message.Contains("HttpOption.Data"));

        
    }
}
