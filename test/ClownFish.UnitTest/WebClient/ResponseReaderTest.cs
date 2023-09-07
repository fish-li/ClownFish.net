#pragma warning disable SYSLIB0014 // 类型或成员已过时
using ClownFish.UnitTest.Base;

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
    public void Test_GetEncodingFromString()
    {
        Assert.IsNull(ResponseReader.GetEncodingFromString(null));
        Assert.IsNull(ResponseReader.GetEncodingFromString("xxx"));

        Assert.AreEqual(Encoding.UTF8, ResponseReader.GetEncodingFromString("utf-8"));
        Assert.AreEqual(Encoding.Unicode, ResponseReader.GetEncodingFromString("utf-16"));
        Assert.AreEqual(Encoding.GetEncoding("GB2312"), ResponseReader.GetEncodingFromString("GB2312"));
    }

    [TestMethod]
    public void Test_GetEncodingFromContentType()
    {
        // 规范参考：https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Headers/Content-Type
        // Content-Type: text/html; charset=utf-8
        // Content-Type: multipart/form-data; boundary=something

        /* 规范中的示例，https://tools.ietf.org/html/rfc7231#section-3.1.1.1
            text/html; charset=utf-8
            text/html;charset=utf-8
            text/html;charset=UTF-8
            Text/HTML;Charset="utf-8"
            text/html; charset="utf-8"
         */

        Assert.IsNull(ResponseReader.GetEncodingFromContentType(null));
        Assert.IsNull(ResponseReader.GetEncodingFromContentType("xxxxxx"));

        Assert.IsNull(ResponseReader.GetEncodingFromContentType("text/html"));
        Assert.IsNull(ResponseReader.GetEncodingFromContentType("multipart/form-data; boundary=something"));

        Assert.IsNull(ResponseReader.GetEncodingFromContentType("text/html: charset=utf-8"));
        Assert.IsNull(ResponseReader.GetEncodingFromContentType("text/html; charset=utf-8;"));

        Assert.AreEqual(Encoding.UTF8, ResponseReader.GetEncodingFromContentType("text/html; charset=utf-8"));
        Assert.AreEqual(Encoding.UTF8, ResponseReader.GetEncodingFromContentType("text/html;charset=utf-8"));
        Assert.AreEqual(Encoding.UTF8, ResponseReader.GetEncodingFromContentType("text/html;charset=UTF-8"));
        Assert.AreEqual(Encoding.UTF8, ResponseReader.GetEncodingFromContentType("text/html;CHARset=utf-8"));
        Assert.AreEqual(Encoding.UTF8, ResponseReader.GetEncodingFromContentType("text/html;Charset=\"utf-8\""));
        Assert.AreEqual(Encoding.UTF8, ResponseReader.GetEncodingFromContentType("text/html; charset=\"utf-8\""));

        Assert.AreEqual(Encoding.GetEncoding("GB2312"), ResponseReader.GetEncodingFromContentType("text/html; charset=gb2312"));
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
            string result1 = ResponseReader.ReadResponseAsText(ms1, "text/html; charset=utf8");
            Assert.IsTrue(result1.Contains("中文汉字"));

            string result2 = ResponseReader.ReadResponseAsText(ms1, "text/html");
            Assert.IsTrue(result2.Contains("中文汉字"));

            string result3 = ResponseReader.ReadResponseAsText(ms1, "text/xxx");
            Assert.IsTrue(result3.Contains("中文汉字"));

            string result4 = ResponseReader.ReadResponseAsText(ms1, null);
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
            string result1 = ResponseReader.ReadResponseAsText(ms1, "text/html");
            Assert.IsTrue(result1.Contains("中文汉字"));

            string result2 = ResponseReader.ReadResponseAsText(ms1, "text/plain");
            Assert.IsTrue(result2.Contains("中文汉字"));

            string result4 = ResponseReader.ReadResponseAsText(ms1, null);
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
            string result1 = ResponseReader.ReadResponseAsText(ms1, "text/html; charset=gb2312");
            Assert.IsTrue(result1.Contains("中文汉字"));

            string result2 = ResponseReader.ReadResponseAsText(ms1, "text/html; charset=utf-8");  // 这种情况下会返回乱码
            Assert.IsFalse(result2.Contains("中文汉字"));

            string result4 = ResponseReader.ReadResponseAsText(ms1, null);
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


    [TestMethod]
    public void Test_ConvertResult()
    {
        Assert.AreEqual("abc", ResponseReader.ConvertResult<string>("abc", "xxxxxxx"));
        Assert.AreEqual(null, ResponseReader.ConvertResult<Product2>("", "xxxxxxx"));

        Product2 p0 = Product2.CreateByFixedData();

        string json = p0.ToJson();
        Product2 p1 = ResponseReader.ConvertResult<Product2>(json, "application/json; charset=utf-8");
        Assert.IsTrue(p0.IsEqual(p1));


        string xml = p0.ToXml();
        Product2 p2 = ResponseReader.ConvertResult<Product2>(xml, "application/xml; charset=utf-8");
        Assert.IsTrue(p0.IsEqual(p2));

        Assert.AreEqual(123, ResponseReader.ConvertResult<int>("123", "text/plain; charset=utf-8"));
    }

}
#pragma warning restore SYSLIB0014 // 类型或成员已过时