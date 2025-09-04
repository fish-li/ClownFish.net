#pragma warning disable SYSLIB0014 // 类型或成员已过时
using System.Net.Http;
using System.Numerics;
using ClownFish.UnitTest.Base;

#if NETCOREAPP
using ClownFish.WebClient.V2;
using MimeKit;
#endif

namespace ClownFish.UnitTest.WebClient;

[TestClass]
public class ResponseReaderTest
{
    public static readonly string TestUrl = "http://www.fish-test.com/test1.aspx";


    [TestMethod]
    public void Test_As_Text()
    {
        HttpWebRequest request = WebRequest.CreateHttp(TestUrl);
        using( HttpWebResponse response = (HttpWebResponse)request.GetResponse() ) {

            using( ResponseReader reader = new ResponseReader(response) ) {

                HttpResult<string> result = reader.Read<HttpResult<string>>();
                Assert.AreEqual(200, result.StatusCode);
                Assert.IsTrue(result.Result.StartsWith("<!DOCTYPE html>"));
            }
        }
    }



    [TestMethod]
    public void Test_As_Bytes()
    {
        HttpWebRequest request = WebRequest.CreateHttp(TestUrl);
        using( HttpWebResponse response = (HttpWebResponse)request.GetResponse() ) {

            using( ResponseReader reader = new ResponseReader(response) ) {

                HttpResult<byte[]> result = reader.Read<HttpResult<byte[]>>();
                Assert.AreEqual(200, result.StatusCode);

                string html = Encoding.UTF8.GetString(result.Result);
                Assert.IsTrue(html.StartsWith("<!DOCTYPE html>"));
            }
        }
    }
    

    [TestMethod]
    public void Test_As_Stream()
    {
        HttpWebRequest request = WebRequest.CreateHttp(TestUrl);
        using( HttpWebResponse response = (HttpWebResponse)request.GetResponse() ) {

            using( ResponseReader reader = new ResponseReader(response) ) {

                HttpResult<Stream> result = reader.Read<HttpResult<Stream>>();
                Assert.AreEqual(200, result.StatusCode);

                string html = Encoding.UTF8.GetString(result.Result.ToArray());
                Assert.IsTrue(html.StartsWith("<!DOCTYPE html>"));
            }
        }
    }


    [TestMethod]
    public void Test_As_Gzip()
    {
        HttpWebRequest request = WebRequest.CreateHttp(TestUrl + "?x-result-CompressionMode=gzip");
        using( HttpWebResponse response = (HttpWebResponse)request.GetResponse() ) {

            using( ResponseReader reader = new ResponseReader(response, true) ) {

                HttpResult<string> result = reader.Read<HttpResult<string>>();
                Assert.AreEqual(200, result.StatusCode);
                Assert.IsTrue(result.Result.StartsWith("<!DOCTYPE html>"));
                Console.WriteLine(result.Result);
            }
        }
    }

    [TestMethod]
    public void Test_As_Gzip_2()
    {
        HttpWebRequest request = WebRequest.CreateHttp(TestUrl + "?x-result-CompressionMode=gzip");
        using( HttpWebResponse response = (HttpWebResponse)request.GetResponse() ) {

            using( ResponseReader reader = new ResponseReader(response) ) {  // 注意这里没有指定第2个参数

                HttpResult<string> result = reader.Read<HttpResult<string>>();
                Assert.AreEqual(200, result.StatusCode);
                Assert.IsFalse(result.Result.StartsWith("<!DOCTYPE html>"));  // 此时得到的结果是一些乱码
                Console.WriteLine(result.Result);
            }
        }
    }


    
   


    [TestMethod]
    public void Test_GetEncodingFromHtmlHeader()
    {
        string s1 = "xxx<meta http-equiv=\"charset\"  content=\"gb2312\">xx";
        string s2 = "xxx<meta charset=\"gb2312\">xx";
        string s3 = "xxx<meta http-equiv=\"Content-Type\" content=\"text/html; charset=gb2312\"/>xx";

        Assert.AreEqual(Encoding.GetEncoding("GB2312"), ResponseReader.GetEncodingFromHtmlHeader(s1));
        Assert.AreEqual(Encoding.GetEncoding("GB2312"), ResponseReader.GetEncodingFromHtmlHeader(s2));
        Assert.AreEqual(Encoding.GetEncoding("GB2312"), ResponseReader.GetEncodingFromHtmlHeader(s3));
        Assert.IsNull(ResponseReader.GetEncodingFromHtmlHeader(null));
    }


    [TestMethod]
    public void Test_ReadText()
    {
        string text = "<body><h2>中文汉字</h2></body>";
        byte[] b1 = Encoding.UTF8.GetBytes(text);

        using(MemoryStream ms1 = new MemoryStream(b1) ) {
            string result1 = ResponseReader.ReadText(ms1, Encoding.UTF8);
            Assert.IsTrue(result1.Contains("中文汉字"));

            string result2 = ResponseReader.ReadText(ms1, Encoding.GetEncoding("GB2312"));
            Assert.IsFalse(result2.Contains("中文汉字"));
        }


        byte[] b2 = Encoding.GetEncoding("GB2312").GetBytes(text);
        using( MemoryStream ms2 = new MemoryStream(b2) ) {
            string result1 = ResponseReader.ReadText(ms2, Encoding.UTF8);
            Assert.IsFalse(result1.Contains("中文汉字"));

            string result2 = ResponseReader.ReadText(ms2, Encoding.GetEncoding("GB2312"));
            Assert.IsTrue(result2.Contains("中文汉字"));
        }
    }

    [TestMethod]
    public void Test_ReadHtml_NotSetCharset_utf8()
    {
        string text = @"
<!DOCTYPE html>
<html><head>
<title>test</title>
</head>
<body>
<h2>中文汉字</h2>
</body>
</html>";
        Encoding htmlEncoding = null;
        byte[] b1 = Encoding.UTF8.GetBytes(text);

        using( MemoryStream ms1 = new MemoryStream(b1) ) {
            string result1 = ResponseReader.ReadHtml(ms1, Encoding.UTF8, out htmlEncoding);
            Assert.IsTrue(result1.Contains("中文汉字"));
            Assert.IsNull(htmlEncoding);

            string result2 = ResponseReader.ReadHtml(ms1, Encoding.GetEncoding("GB2312"), out htmlEncoding);
            Assert.IsFalse(result2.Contains("中文汉字"));
            Assert.IsNull(htmlEncoding);
        }
    }


    [TestMethod]
    public void Test_ReadHtml_NotSetCharset_gb2312()
    {
        string text = @"
<!DOCTYPE html>
<html><head>
<title>test</title>
</head>
<body>
<h2>中文汉字</h2>
</body>
</html>";
        Encoding htmlEncoding = null;
        byte[] b2 = Encoding.GetEncoding("GB2312").GetBytes(text);

        using( MemoryStream ms2 = new MemoryStream(b2) ) {
            string result1 = ResponseReader.ReadHtml(ms2, Encoding.UTF8, out htmlEncoding);
            Assert.IsFalse(result1.Contains("中文汉字"));
            Assert.IsNull(htmlEncoding);

            string result2 = ResponseReader.ReadHtml(ms2, Encoding.GetEncoding("GB2312"), out htmlEncoding);
            Assert.IsTrue(result2.Contains("中文汉字"));
            Assert.IsNull(htmlEncoding);
        }
    }

    [TestMethod]
    public void Test_ReadHtml_HttpEquiv_Charset_utf8()
    {
        string text = @"
<!DOCTYPE html>
<html><head>
<title>test</title>
<meta http-equiv=""charset"" content=""utf-8"">
</head>
<body>
<h2>中文汉字</h2>
</body>
</html>";
        Encoding htmlEncoding = null;
        byte[] b1 = Encoding.UTF8.GetBytes(text);  // 编码和HTML内容一致，ReadHtml用什么默认编码都无所谓

        using( MemoryStream ms1 = new MemoryStream(b1) ) {
            //string result1 = ResponseReader.ReadHtml(ms1, Encoding.Unicode);
            //Assert.IsTrue(result1.Contains("中文汉字"));

            string result2 = ResponseReader.ReadHtml(ms1, Encoding.GetEncoding("GB2312"), out htmlEncoding);
            Assert.IsTrue(result2.Contains("中文汉字"));
            Assert.AreEqual(Encoding.UTF8, htmlEncoding);

            string result3 = ResponseReader.ReadHtml(ms1, Encoding.ASCII, out htmlEncoding);
            Assert.IsTrue(result3.Contains("中文汉字"));
            Assert.AreEqual(Encoding.UTF8, htmlEncoding);

            string result4 = ResponseReader.ReadHtml(ms1, Encoding.UTF8, out htmlEncoding);
            Assert.IsTrue(result4.Contains("中文汉字"));
            Assert.IsNull(htmlEncoding);  // 没有重新读取

            string result5 = ResponseReader.ReadHtml(ms1, Encoding.GetEncoding("iso-8859-1"), out htmlEncoding);
            Assert.IsTrue(result5.Contains("中文汉字"));
            Assert.AreEqual(Encoding.UTF8, htmlEncoding);
        }
    }

    [TestMethod]
    public void Test_ReadHtml_HttpEquiv_Charset_gb2312()
    {
        string text = @"
<!DOCTYPE html>
<html><head>
<title>test</title>
<meta http-equiv=""charset"" content=""gb2312"">
</head>
<body>
<h2>中文汉字</h2>
</body>
</html>";
        Encoding htmlEncoding = null;
        byte[] b1 = Encoding.GetEncoding("GB2312").GetBytes(text);  // 编码和HTML内容一致，ReadHtml用什么默认编码都无所谓

        using( MemoryStream ms1 = new MemoryStream(b1) ) {
            //string result1 = ResponseReader.ReadHtml(ms1, Encoding.Unicode);
            //Assert.IsTrue(result1.Contains("中文汉字"));

            string result2 = ResponseReader.ReadHtml(ms1, Encoding.GetEncoding("GB2312"), out htmlEncoding);
            Assert.IsTrue(result2.Contains("中文汉字"));
            Assert.IsNull(htmlEncoding);  // 没有重新读取

            string result3 = ResponseReader.ReadHtml(ms1, Encoding.ASCII, out htmlEncoding);
            Assert.IsTrue(result3.Contains("中文汉字"));
            Assert.AreEqual(Encoding.GetEncoding("GB2312"), htmlEncoding);

            string result4 = ResponseReader.ReadHtml(ms1, Encoding.UTF8, out htmlEncoding);
            Assert.IsTrue(result4.Contains("中文汉字"));
            Assert.AreEqual(Encoding.GetEncoding("GB2312"), htmlEncoding);

            string result5 = ResponseReader.ReadHtml(ms1, Encoding.GetEncoding("iso-8859-1"), out htmlEncoding);
            Assert.IsTrue(result5.Contains("中文汉字"));
            Assert.AreEqual(Encoding.GetEncoding("GB2312"), htmlEncoding);

        }
    }

    [TestMethod]
    public void Test_ReadHtml_Charset_utf8()
    {
        string text = @"
<!DOCTYPE html>
<html><head>
<title>test</title>
<meta charset=""utf-8"">
</head>
<body>
<h2>中文汉字</h2>
</body>
</html>";
        Encoding htmlEncoding = null;
        byte[] b1 = Encoding.UTF8.GetBytes(text);  // 编码和HTML内容一致，ReadHtml用什么默认编码都无所谓

        using( MemoryStream ms1 = new MemoryStream(b1) ) {
            //string result1 = ResponseReader.ReadHtml(ms1, Encoding.Unicode);
            //Assert.IsTrue(result1.Contains("中文汉字"));

            string result2 = ResponseReader.ReadHtml(ms1, Encoding.GetEncoding("GB2312"), out htmlEncoding);
            Assert.IsTrue(result2.Contains("中文汉字"));
            Assert.AreEqual(Encoding.UTF8, htmlEncoding);

            string result3 = ResponseReader.ReadHtml(ms1, Encoding.ASCII, out htmlEncoding);
            Assert.IsTrue(result3.Contains("中文汉字"));
            Assert.AreEqual(Encoding.UTF8, htmlEncoding);

            string result4 = ResponseReader.ReadHtml(ms1, Encoding.UTF8, out htmlEncoding);
            Assert.IsTrue(result4.Contains("中文汉字"));
            Assert.IsNull(htmlEncoding);  // 没有重新读取

            string result5 = ResponseReader.ReadHtml(ms1, Encoding.GetEncoding("iso-8859-1"), out htmlEncoding);
            Assert.IsTrue(result5.Contains("中文汉字"));
            Assert.AreEqual(Encoding.UTF8, htmlEncoding);

        }
    }

    [TestMethod]
    public void Test_ReadHtml_Charset_gb2312()
    {
        string text = @"
<!DOCTYPE html>
<html><head>
<title>test</title>
<meta charset=""gb2312"">
</head>
<body>
<h2>中文汉字</h2>
</body>
</html>";
        Encoding htmlEncoding = null;
        byte[] b1 = Encoding.GetEncoding("GB2312").GetBytes(text);  // 编码和HTML内容一致，ReadHtml用什么默认编码都无所谓

        using( MemoryStream ms1 = new MemoryStream(b1) ) {
            //string result1 = ResponseReader.ReadHtml(ms1, Encoding.Unicode);
            //Assert.IsTrue(result1.Contains("中文汉字"));

            string result2 = ResponseReader.ReadHtml(ms1, Encoding.GetEncoding("GB2312"), out htmlEncoding);
            Assert.IsTrue(result2.Contains("中文汉字"));
            Assert.IsNull(htmlEncoding);  // 没有重新读取

            string result3 = ResponseReader.ReadHtml(ms1, Encoding.ASCII, out htmlEncoding);
            Assert.IsTrue(result3.Contains("中文汉字"));
            Assert.AreEqual(Encoding.GetEncoding("GB2312"), htmlEncoding);

            string result4 = ResponseReader.ReadHtml(ms1, Encoding.UTF8, out htmlEncoding);
            Assert.IsTrue(result4.Contains("中文汉字"));
            Assert.AreEqual(Encoding.GetEncoding("GB2312"), htmlEncoding);

            string result5 = ResponseReader.ReadHtml(ms1, Encoding.GetEncoding("iso-8859-1"), out htmlEncoding);
            Assert.IsTrue(result5.Contains("中文汉字"));
            Assert.AreEqual(Encoding.GetEncoding("GB2312"), htmlEncoding);
        }
    }


    [TestMethod]
    public void Test_ReadHtml_HttpEquiv_ContentType_Charset_utf8()
    {
        string text = @"
<!DOCTYPE html>
<html><head>
<title>test</title>
<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8""/>
</head>
<body>
<h2>中文汉字</h2>
</body>
</html>";
        Encoding htmlEncoding = null;
        byte[] b1 = Encoding.UTF8.GetBytes(text);  // 编码和HTML内容一致，ReadHtml用什么默认编码都无所谓

        using( MemoryStream ms1 = new MemoryStream(b1) ) {
            //string result1 = ResponseReader.ReadHtml(ms1, Encoding.Unicode);
            //Assert.IsTrue(result1.Contains("中文汉字"));

            string result2 = ResponseReader.ReadHtml(ms1, Encoding.GetEncoding("GB2312"), out htmlEncoding);
            Assert.IsTrue(result2.Contains("中文汉字"));
            Assert.AreEqual(Encoding.UTF8, htmlEncoding);

            string result3 = ResponseReader.ReadHtml(ms1, Encoding.ASCII, out htmlEncoding);
            Assert.IsTrue(result3.Contains("中文汉字"));
            Assert.AreEqual(Encoding.UTF8, htmlEncoding);

            string result4 = ResponseReader.ReadHtml(ms1, Encoding.UTF8, out htmlEncoding);
            Assert.IsTrue(result4.Contains("中文汉字"));
            Assert.IsNull(htmlEncoding);  // 没有重新读取

            string result5 = ResponseReader.ReadHtml(ms1, Encoding.GetEncoding("iso-8859-1"), out htmlEncoding);
            Assert.IsTrue(result5.Contains("中文汉字"));
            Assert.AreEqual(Encoding.UTF8, htmlEncoding);
        }
    }


    [TestMethod]
    public void Test_ReadHtml_HttpEquiv_ContentType_Charset_gb2312()
    {
        string text = @"
<!DOCTYPE html>
<html><head>
<title>test</title>
<meta http-equiv=""Content-Type"" content=""text/html; charset=gb2312""/>
</head>
<body>
<h2>中文汉字</h2>
</body>
</html>";
        Encoding htmlEncoding = null;
        byte[] b1 = Encoding.GetEncoding("GB2312").GetBytes(text);  // 编码和HTML内容一致，ReadHtml用什么默认编码都无所谓

        using( MemoryStream ms1 = new MemoryStream(b1) ) {
            //string result1 = ResponseReader.ReadHtml(ms1, Encoding.Unicode);
            //Assert.IsTrue(result1.Contains("中文汉字"));

            string result2 = ResponseReader.ReadHtml(ms1, Encoding.GetEncoding("GB2312"), out htmlEncoding);
            Assert.IsTrue(result2.Contains("中文汉字"));
            Assert.IsNull(htmlEncoding);  // 没有重新读取

            string result3 = ResponseReader.ReadHtml(ms1, Encoding.ASCII, out htmlEncoding);
            Assert.IsTrue(result3.Contains("中文汉字"));
            Assert.AreEqual(Encoding.GetEncoding("GB2312"), htmlEncoding);

            string result4 = ResponseReader.ReadHtml(ms1, Encoding.UTF8, out htmlEncoding);
            Assert.IsTrue(result4.Contains("中文汉字"));
            Assert.AreEqual(Encoding.GetEncoding("GB2312"), htmlEncoding);

            string result5 = ResponseReader.ReadHtml(ms1, Encoding.GetEncoding("iso-8859-1"), out htmlEncoding);
            Assert.IsTrue(result5.Contains("中文汉字"));
            Assert.AreEqual(Encoding.GetEncoding("GB2312"), htmlEncoding);
        }
    }


    internal static string ResponseReaderReadResponseAsText(Stream responseStream, string contentType, long maxLimitLen = 0)
    {
        HttpUtils.ParseContentType(contentType, out string mediaType, out Encoding encoding);
        return ResponseReader.ReadResponseAsText(responseStream, mediaType, encoding, maxLimitLen);
    }

    [TestMethod]
    public void Test_GetResponseText_use_http_header()
    {
        string text = @"
<!DOCTYPE html>
<html><head>
<title>test</title>
</head>
<body>
<h2>中文汉字</h2>
</body>
</html>";
        byte[] b1 = Encoding.UTF8.GetBytes(text);

        using( MemoryStream ms1 = new MemoryStream(b1) ) {
            string result1 = ResponseReaderReadResponseAsText(ms1, "text/html; charset=utf8");
            Assert.IsTrue(result1.Contains("中文汉字"));

            string result2 = ResponseReaderReadResponseAsText(ms1, "text/html");
            Assert.IsTrue(result2.Contains("中文汉字"));

            string result3 = ResponseReaderReadResponseAsText(ms1, "text/xxx");
            Assert.IsTrue(result3.Contains("中文汉字"));

            string result4 = ResponseReaderReadResponseAsText(ms1, null);
            Assert.IsTrue(result4.Contains("中文汉字"));
        }
    }


    [TestMethod]
    public void Test_GetResponseText_use_html_header()
    {
        string text = @"
<!DOCTYPE html>
<html><head>
<title>test</title>
<meta http-equiv=""charset"" content=""utf-8"">
</head>
<body>
<h2>中文汉字</h2>
</body>
</html>";

        byte[] b1 = Encoding.UTF8.GetBytes(text);

        using( MemoryStream ms1 = new MemoryStream(b1) ) {
            string result1 = ResponseReaderReadResponseAsText(ms1, "text/html");
            Assert.IsTrue(result1.Contains("中文汉字"));

            string result2 = ResponseReaderReadResponseAsText(ms1, "text/plain");
            Assert.IsTrue(result2.Contains("中文汉字"));

            string result4 = ResponseReaderReadResponseAsText(ms1, null);
            Assert.IsTrue(result4.Contains("中文汉字"));
        }
    }


    [TestMethod]
    public void Test_GetResponseText_use_html_header2()
    {
        string text = @"
<!DOCTYPE html>
<html><head>
<title>test</title>
<meta http-equiv=""charset"" content=""gb2312"">
</head>
<body>
<h2>中文汉字</h2>
</body>
</html>";

        byte[] b1 = Encoding.GetEncoding("GB2312").GetBytes(text);

        using( MemoryStream ms1 = new MemoryStream(b1) ) {
            string result1 = ResponseReaderReadResponseAsText(ms1, "text/html; charset=gb2312");
            Assert.IsTrue(result1.Contains("中文汉字"));

            string result2 = ResponseReaderReadResponseAsText(ms1, "text/html; charset=utf-8");  // 这种情况下会返回乱码
            Assert.IsFalse(result2.Contains("中文汉字"));

            string result4 = ResponseReaderReadResponseAsText(ms1, null);
            Assert.IsFalse(result4.Contains("中文汉字"));
        }
    }


    [TestMethod]
    public void Test_Error()
    {
        MyAssert.IsError<ArgumentNullException>(()=> {
            _ = new ResponseReader(null);
        });
    }


    internal static T ResponseReaderConvertResult<T>(string responseText, string contentType)
    {
        HttpUtils.ParseContentType(contentType, out string mediaType, out Encoding encoding);
        return ResponseReader.ConvertResult<T>(responseText, mediaType, contentType);
    }

    [TestMethod]
    public void Test_ConvertResult()
    {
        Assert.AreEqual("abc", ResponseReaderConvertResult<string>("abc", "xxxxxxx"));
        Assert.AreEqual(null, ResponseReaderConvertResult<Product3>("", "xxxxxxx"));

        Product3 p0 = Product3.CreateByFixedData();

        string json = p0.ToJson();
        Product3 p1 = ResponseReaderConvertResult<Product3>(json, "application/json; charset=utf-8");
        Assert.IsTrue(p0.IsEqual(p1));


        string xml = p0.ToXml();
        Product3 p2 = ResponseReaderConvertResult<Product3>(xml, "application/xml; charset=utf-8");
        Assert.IsTrue(p0.IsEqual(p2));

        Assert.AreEqual(123, ResponseReaderConvertResult<int>("123", "text/plain; charset=utf-8"));

        Assert.AreEqual(123, ResponseReaderConvertResult<int>("123", ""));
    }

    [TestMethod]
    public void Test_ConvertResult_Error()
    {
        string json = Product3.CreateByFixedData().ToJson();

        MyAssert.IsError<NotSupportedException>(() => {
            ResponseReaderConvertResult<Product3>(json, "x; charset=utf-8");
        });

        MyAssert.IsError<NotSupportedException>(() => {
            ResponseReaderConvertResult<Product3>(json, "application/json-seq");   // 没有被【标准化】，暂不支持
        });

        MyAssert.IsError<NotSupportedException>(() => {
            ResponseReaderConvertResult<Product3>(json, "application/jsonl");   // 没有被【标准化】，暂不支持
        });

        MyAssert.IsError<NotSupportedException>(() => {
            ResponseReaderConvertResult<Product3>(json, "application/jsonl ; charset=xxx");
        });
    }

    

#if NETCOREAPP
    [TestMethod]
    public void Test_CheckMaxAllowLen()
    {
        HttpResponseMessage responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        responseMessage.Content = HttpObjectUtils.CreateRequestMessageBody3(SerializeFormat.Text, "中华文明-5000年");   // ContentLength = 20
        HttpWebResponse response = HttpClient2.CreateHttpWebResponse(responseMessage, new Uri(TestUrl), null);

        ResponseReader reader1 = new ResponseReader(response, false, 10);
        MyAssert.IsError<ResponseBodyTooLargeException>(() => {
            reader1.CheckMaxLimitLen();
        });

        ResponseReader reader2 = new ResponseReader(response, false, -10);  // 不检查长度
        Assert.AreEqual(0, reader2.CheckMaxLimitLen());

        ResponseReader reader3 = new ResponseReader(response, false, int.MaxValue);
        Assert.AreEqual(-1L, reader3.CheckMaxLimitLen());    // 长度已检查，然后直接修改内部变量
    }

    [TestMethod]
    public void Test_CheckMaxAllowLen2()
    {
        byte[] data = "中华文明-5000年".ToUtf8Bytes();
        NotLenMemoryStream ms = new NotLenMemoryStream(data);
        HttpResponseMessage responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        responseMessage.Content = HttpObjectUtils.CreateRequestMessageBody1(SerializeFormat.None, ms);   //  没有 Content-Length 头

        HttpWebResponse response = HttpClient2.CreateHttpWebResponse(responseMessage, new Uri(TestUrl), null);

        ResponseReader reader1 = new ResponseReader(response, false, 10);
        Assert.AreEqual(10, reader1.CheckMaxLimitLen());
    }


    private class NotLenMemoryStream : Stream
    {
        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotImplementedException();

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private readonly MemoryStream _ms;

        public NotLenMemoryStream(byte[] data)
        {
            _ms = new MemoryStream(data);
        }

        public override void Flush()
        {

        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _ms.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }


    [TestMethod]
    public void Test_ReadResponseAsBytes_LimitLen()
    {
        byte[] data = "中华文明-5000年".ToUtf8Bytes();

        NotLenMemoryStream ms1 = new NotLenMemoryStream(data);
        byte[] data2 = ResponseReader.ReadResponseAsBytes(ms1, 0);  // 不检查长度
        MyAssert.AreEqual(data, data2);

        NotLenMemoryStream ms2 = new NotLenMemoryStream(data);
        byte[] data3 = ResponseReader.ReadResponseAsBytes(ms2, 100);  // 流的长度没有超过最大值
        MyAssert.AreEqual(data, data3);

        MyAssert.IsError<ResponseBodyTooLargeException>(() => {
            NotLenMemoryStream ms3 = new NotLenMemoryStream(data);
            byte[] data4 = ResponseReader.ReadResponseAsBytes(ms3, 10);  // 流的长度超标
        });       
    }


    [TestMethod]
    public void Test_ReadText_LimitLen()
    {
        string input = "中华文明-5000年；中华文明-5000年；\r\n中华文明-5000年；中华文明-5000年；\r\n中华文明-5000年；";

        NotLenMemoryStream ms1 = new NotLenMemoryStream(input.ToUtf8Bytes());
        string text1 = ResponseReader.ReadText(ms1, null, 0);
        Assert.AreEqual(input, text1);

        NotLenMemoryStream ms2 = new NotLenMemoryStream(input.ToUtf8Bytes());
        string text2 = ResponseReader.ReadText(ms2, null, 100);
        Assert.AreEqual(input, text2);

        MyAssert.IsError<ResponseBodyTooLargeException>(() => {
            NotLenMemoryStream ms3 = new NotLenMemoryStream(input.ToUtf8Bytes());
            string text3 = ResponseReader.ReadText(ms3, null, 10);  // 流的长度超标
        });
    }

#endif

    [TestMethod]
    public void Test_ReturnResultFromTextStream_Text()
    {
        string text = Guid.NewGuid().ToString();
        MemoryStream ms = new MemoryStream(text.GetBytes());

        string result = ResponseReader.ReturnResultFromTextStream<string>(ms, "text/plain; charset=utf-8");
        Assert.AreEqual(text, result);
    }


    [TestMethod]
    public void Test_ReturnResultFromTextStream_Text_Empty()
    {
        string text = "";
        MemoryStream ms = new MemoryStream(text.GetBytes());

        Product3 p = ResponseReader.ReturnResultFromTextStream<Product3>(ms, "application/json; charset=utf-8");
        Assert.IsNull(p);
    }

    [TestMethod]
    public void Test_ReturnResultFromTextStream_Media_Empty()
    {
        string text = "123";
        MemoryStream ms = new MemoryStream(text.GetBytes());

        int value = ResponseReader.ReturnResultFromTextStream<int>(ms, "");
        Assert.AreEqual(123, value);
    }


    [TestMethod]
    public void Test_ReturnResultFromTextStream_Json()
    {
        MemoryStream ms = new MemoryStream(Product3.CreateTestDataList(9).ToJson().GetBytes());

        List<Product3> list = ResponseReader.ReturnResultFromTextStream<List<Product3>>(ms, "application/json; charset=utf-8");
        Assert.AreEqual(9, list.Count);
    }

    [TestMethod]
    public void Test_ReturnResultFromTextStream_NdJson()
    {
        MemoryStream ms = new MemoryStream(Product3.CreateTestDataList(9).ToNdjson().GetBytes());

        List<Product3> list = ResponseReader.ReturnResultFromTextStream<List<Product3>>(ms, "application/x-ndjson; charset=utf-8");
        Assert.AreEqual(9, list.Count);
    }


    [TestMethod]
    public void Test_ReturnResultFromTextStream_xml()
    {
        MemoryStream ms = new MemoryStream(Product3.CreateTestDataList(9).ToXml().GetBytes());

        List<Product3> list = ResponseReader.ReturnResultFromTextStream<List<Product3>>(ms, "application/xml; charset=utf-8");
        Assert.AreEqual(9, list.Count);
    }

    [TestMethod]
    public void Test_ReturnResultFromTextStream_Enum()
    {
        string text = "MySQL";
        MemoryStream ms = new MemoryStream(text.GetBytes());

        DatabaseType result = ResponseReader.ReturnResultFromTextStream<DatabaseType>(ms, "text/plain");
        Assert.AreEqual(DatabaseType.MySQL, result);
    }

    [TestMethod]
    public void Test_ReturnResultFromTextStream_NotSupportedException()
    {
        string text = "MySQL";
        MemoryStream ms = new MemoryStream(text.GetBytes());

        string contentType = "text/abc; charset=utf-8";

        NotSupportedException ex = MyAssert.IsError<NotSupportedException>(() => {
            DatabaseType result = ResponseReader.ReturnResultFromTextStream<DatabaseType>(ms, contentType);
        });

        Assert.IsTrue(ex.Message.Contains(contentType));
        Console.WriteLine(ex.Message);
    }


    [TestMethod]
    public void Test_ReturnObjectFromJsonStream()
    {
        MemoryStream ms = new MemoryStream(Product3.CreateTestDataList(9).ToJson().GetBytes());

        List<Product3> list = ResponseReader.ReturnObjectFromJsonStream<List<Product3>>(ms, null);
        Assert.AreEqual(9, list.Count);
    }


    [TestMethod]
    public void Test_ReturnListFromNdjsonStream()
    {
        MemoryStream ms = new MemoryStream(Product3.CreateTestDataList(10).ToNdjson().GetBytes());

        List<Product3> list = ResponseReader.ReturnListFromNdjsonStream<List<Product3>>(ms, null);
        Assert.AreEqual(10, list.Count);



        MemoryStream emptyMs = new MemoryStream();
        List<Product3> list2 = ResponseReader.ReturnListFromNdjsonStream<List<Product3>>(emptyMs, null);
        Assert.AreEqual(0, list2.Count);
    }

    [TestMethod]
    public void Test_ReturnObjectFromXmlStream()
    {
        MemoryStream ms = new MemoryStream(Product3.CreateTestDataList(11).ToXml().GetBytes());

        List<Product3> list = ResponseReader.ReturnObjectFromXmlStream<List<Product3>>(ms, null);
        Assert.AreEqual(11, list.Count);
    }


    [TestMethod]
    public void Test_ReturnTypeIsList()
    {
        Assert.IsTrue(ResponseReader.ReturnTypeIsList<List<Product3>>());
        Assert.IsTrue(ResponseReader.ReturnTypeIsList<List<int>>());

        Assert.IsFalse(ResponseReader.ReturnTypeIsList<int>());
        Assert.IsFalse(ResponseReader.ReturnTypeIsList<Product3>());
        Assert.IsFalse(ResponseReader.ReturnTypeIsList<Product3[]>());
    }

    [TestMethod]
    public void Test_ReturnTypeIsObject()
    {
        Assert.IsTrue(ResponseReader.ReturnTypeIsObject<List<Product3>>());
        Assert.IsTrue(ResponseReader.ReturnTypeIsObject<Product3[]>());

        Assert.IsTrue(ResponseReader.ReturnTypeIsObject<List<int>>());
        Assert.IsTrue(ResponseReader.ReturnTypeIsObject<int[]>());

        Assert.IsTrue(ResponseReader.ReturnTypeIsObject<Product3>());

        Assert.IsFalse(ResponseReader.ReturnTypeIsObject<object>());

        Assert.IsFalse(ResponseReader.ReturnTypeIsObject<bool>());
        Assert.IsFalse(ResponseReader.ReturnTypeIsObject<int>());
        Assert.IsFalse(ResponseReader.ReturnTypeIsObject<long>());
        Assert.IsFalse(ResponseReader.ReturnTypeIsObject<string>());
        Assert.IsFalse(ResponseReader.ReturnTypeIsObject<DatabaseType>());
        Assert.IsFalse(ResponseReader.ReturnTypeIsObject<Guid>());
        Assert.IsFalse(ResponseReader.ReturnTypeIsObject<DateTime>());
        Assert.IsFalse(ResponseReader.ReturnTypeIsObject<decimal>());
        Assert.IsFalse(ResponseReader.ReturnTypeIsObject<TimeSpan>());

        Assert.IsFalse(ResponseReader.ReturnTypeIsObject<int?>());
    }
}
#pragma warning restore SYSLIB0014 // 类型或成员已过时