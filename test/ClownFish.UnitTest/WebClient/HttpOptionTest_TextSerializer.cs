namespace ClownFish.UnitTest.WebClient;
[TestClass]
public class HttpOptionTest_TextSerializer
{
    internal static void SetTestHeader(HttpOption http)
    {
        http.Headers.Add("x-aa", "aa");
        http.Headers.Add("x-bb", "bb");

        CookieContainer cookieContainer = new CookieContainer();
        Uri uri = new Uri(http.Url);
        cookieContainer.Add(uri, new Cookie("name1", "abc"));
        cookieContainer.Add(uri, new Cookie("name2", "xyz"));
        http.Cookie = cookieContainer;
    }

    [TestMethod]
    public void Test_GET()
    {
        HttpOption http = new HttpOption {
            Method = "GET",
            Url = "http://www.fish-test.com/show-body.aspx?a=2&b=3",
        };
        SetTestHeader(http);

        string rawText = http.ToRawText(2);
        //Console.WriteLine(rawText);

        string expectedRaw = @"
GET http://www.fish-test.com/show-body.aspx?a=2&b=3 HTTP/1.1
x-aa: aa
x-bb: bb
Cookie: name1=abc; name2=xyz
".Trim();

        Assert.AreEqual(expectedRaw, rawText.Trim());

        HttpOption http2 = HttpOption.FromRawText(rawText);
        Assert.AreEqual("GET", http2.Method);
        Assert.AreEqual("http://www.fish-test.com/show-body.aspx?a=2&b=3", http2.Url);
        Assert.AreEqual(3, http2.Headers.Count);
        Assert.AreEqual("aa", http2.Headers["x-aa"]);
        Assert.AreEqual("bb", http2.Headers["x-bb"]);
        Assert.AreEqual("name1=abc; name2=xyz", http2.Headers["Cookie"]);
        Assert.IsNull(http2.Headers["Content-Type"]);
        Assert.IsTrue(http2.Format == SerializeFormat.None);
        Assert.IsNull(http2.Data);

        string rawText2 = http2.ToRawText(2);
        Assert.AreEqual(expectedRaw, rawText2.Trim());


        string rawText3 = (http as ITextSerializer).ToText();
        Assert.AreEqual(expectedRaw, rawText3.Trim());

        HttpOption http3 = new HttpOption();
        (http3 as ITextSerializer).LoadData(rawText3);
        Assert.AreEqual(expectedRaw, http3.ToRawText(2).Trim());
    }



    [TestMethod]
    public void Test_Text()
    {
        HttpOption http = new HttpOption {
            Method = "POST",
            Url = "http://www.fish-test.com/show-body.aspx?a=2&b=3",
            Data = "中华文明-5000年",
            Format = SerializeFormat.Text,
        };
        SetTestHeader(http);

        string rawText = http.ToRawText(2);
        //Console.WriteLine(rawText);

        string expectedRaw = @"
POST http://www.fish-test.com/show-body.aspx?a=2&b=3 HTTP/1.1
x-aa: aa
x-bb: bb
Cookie: name1=abc; name2=xyz
Content-Type: text/plain; charset=utf-8

中华文明-5000年".Trim();

        Assert.AreEqual(expectedRaw, rawText);

        HttpOption http2 = HttpOption.FromRawText(rawText);
        Assert.AreEqual("POST", http2.Method);
        Assert.AreEqual("http://www.fish-test.com/show-body.aspx?a=2&b=3", http2.Url);
        Assert.AreEqual(4, http2.Headers.Count);
        Assert.AreEqual("aa", http2.Headers["x-aa"]);
        Assert.AreEqual("bb", http2.Headers["x-bb"]);
        Assert.AreEqual("name1=abc; name2=xyz", http2.Headers["Cookie"]);
        Assert.AreEqual("text/plain; charset=utf-8", http2.Headers["Content-Type"]);
        Assert.IsTrue(http2.Format == SerializeFormat.None);
        Assert.AreEqual(@"中华文明-5000年", http2.Data);

        string rawText2 = http2.ToRawText(2);
        Assert.AreEqual(expectedRaw, rawText2);


        string rawText3 = (http as ITextSerializer).ToText();
        Assert.AreEqual(expectedRaw, rawText3.Trim());

        HttpOption http3 = new HttpOption();
        (http3 as ITextSerializer).LoadData(rawText3);
        Assert.AreEqual(expectedRaw, http3.ToRawText(2).Trim());
    }

    [TestMethod]
    public void Test_Json()
    {
        NameValue data = new NameValue { Name = "abc", Value = new string('中', 5) };

        HttpOption http = new HttpOption {
            Method = "POST",
            Url = "http://www.fish-test.com/show-body.aspx?a=2&b=3",
            Data = data,
            Format = SerializeFormat.Json,
        };
        SetTestHeader(http);

        string rawText = http.ToRawText(2);
        //Console.WriteLine(rawText);

        string expectedRaw = @"
POST http://www.fish-test.com/show-body.aspx?a=2&b=3 HTTP/1.1
x-aa: aa
x-bb: bb
Cookie: name1=abc; name2=xyz
Content-Type: application/json; charset=utf-8

{""Name"":""abc"",""Value"":""中中中中中""}".Trim();

        Assert.AreEqual(expectedRaw, rawText);

        HttpOption http2 = HttpOption.FromRawText(rawText);
        Assert.AreEqual("POST", http2.Method);
        Assert.AreEqual("http://www.fish-test.com/show-body.aspx?a=2&b=3", http2.Url);
        Assert.AreEqual(4, http2.Headers.Count);
        Assert.AreEqual("aa", http2.Headers["x-aa"]);
        Assert.AreEqual("bb", http2.Headers["x-bb"]);
        Assert.AreEqual("name1=abc; name2=xyz", http2.Headers["Cookie"]);
        Assert.AreEqual("application/json; charset=utf-8", http2.Headers["Content-Type"]);
        Assert.IsTrue(http2.Format == SerializeFormat.None);
        Assert.AreEqual(@"{""Name"":""abc"",""Value"":""中中中中中""}", http2.Data);

        string rawText2 = http2.ToRawText(2);
        Assert.AreEqual(expectedRaw, rawText2);


        string rawText3 = (http as ITextSerializer).ToText();
        Assert.AreEqual(expectedRaw, rawText3.Trim());

        HttpOption http3 = new HttpOption();
        (http3 as ITextSerializer).LoadData(rawText3);
        Assert.AreEqual(expectedRaw, http3.ToRawText(2).Trim());
    }


    [TestMethod]
    public void Test_Bytes()
    {
        HttpOption http = new HttpOption {
            Method = "POST",
            Url = "http://www.fish-test.com/show-body.aspx?a=2&b=3",
            Data = "中华文明-5000年".GetBytes(),
            Format = SerializeFormat.Binary,
        };
        SetTestHeader(http);

        string rawText = http.ToRawText(2);
        //Console.WriteLine(rawText);

        string expectedRaw = @"
POST http://www.fish-test.com/show-body.aspx?a=2&b=3 HTTP/1.1
x-aa: aa
x-bb: bb
Cookie: name1=abc; name2=xyz
Content-Type: application/octet-stream
[BODY-IS-BIN]: 1

5Lit5Y2O5paH5piOLTUwMDDlubQ=".Trim();

        Assert.AreEqual(expectedRaw, rawText);

        HttpOption http2 = HttpOption.FromRawText(rawText);
        Assert.AreEqual("POST", http2.Method);
        Assert.AreEqual("http://www.fish-test.com/show-body.aspx?a=2&b=3", http2.Url);
        Assert.AreEqual(4, http2.Headers.Count);
        Assert.AreEqual("aa", http2.Headers["x-aa"]);
        Assert.AreEqual("bb", http2.Headers["x-bb"]);
        Assert.AreEqual("name1=abc; name2=xyz", http2.Headers["Cookie"]);
        Assert.AreEqual("application/octet-stream", http2.Headers["Content-Type"]);
        Assert.IsTrue(http2.Format == SerializeFormat.None);
        Assert.IsTrue(http2.Data.GetType() == typeof(byte[]));
        Assert.AreEqual(@"中华文明-5000年", (http2.Data as byte[]).ToUtf8String());

        string rawText2 = http2.ToRawText(2);
        Assert.AreEqual(expectedRaw, rawText2);


        string rawText3 = (http as ITextSerializer).ToText();
        Assert.AreEqual(expectedRaw, rawText3.Trim());

        HttpOption http3 = new HttpOption();
        (http3 as ITextSerializer).LoadData(rawText3);
        Assert.AreEqual(expectedRaw, http3.ToRawText(2).Trim());
    }

    [TestMethod]
    public void Test_Bytes_ErrorData()
    {
        HttpOption http = new HttpOption {
            Method = "POST",
            Url = "http://www.fish-test.com/show-body.aspx?a=2&b=3",
            Data = "中华文明-5000年".GetBytes(),
            Format = SerializeFormat.Binary,
        };
        SetTestHeader(http);

        string rawText = http.ToRawText(1);
        //Console.WriteLine(rawText);

        string expectedRaw = @"
POST http://www.fish-test.com/show-body.aspx?a=2&b=3 HTTP/1.1
x-aa: aa
x-bb: bb
Cookie: name1=abc; name2=xyz
Content-Type: application/octet-stream
[BODY-IS-BIN]: 1

##--NOT TEXT DATA, Length:(20)--##".Trim();

        Assert.AreEqual(expectedRaw, rawText);

        HttpOption http2 = HttpOption.FromRawText(rawText);
        Assert.AreEqual("POST", http2.Method);
        Assert.AreEqual("http://www.fish-test.com/show-body.aspx?a=2&b=3", http2.Url);
        Assert.AreEqual(4, http2.Headers.Count);
        Assert.AreEqual("aa", http2.Headers["x-aa"]);
        Assert.AreEqual("bb", http2.Headers["x-bb"]);
        Assert.AreEqual("name1=abc; name2=xyz", http2.Headers["Cookie"]);
        Assert.AreEqual("application/octet-stream", http2.Headers["Content-Type"]);

        Assert.IsTrue(http2.Format == SerializeFormat.None);
        Assert.IsTrue(http2.Data.GetType() == typeof(string));
        Assert.AreEqual("##--NOT TEXT DATA, Length:(20)--##", http2.Data.ToString());
    }

    [TestMethod]
    public void Test_Bytes_2()    // 这是最通用的存储方式，Format=None, Data=byte[], Headers["Content-Type"] = "xxx"
    {
        HttpOption http = new HttpOption {
            Method = "POST",
            Url = "http://www.fish-test.com/show-body.aspx?a=2&b=3",
            Data = "中华文明-5000年".GetBytes(),
            Format = SerializeFormat.None,
        };
        HttpOptionTest_TextSerializer.SetTestHeader(http);
        http.Headers["Content-Type"] = "text/plain; charset=gb2312";    // 故意 写个“错误”的charset

        string rawText = http.ToRawText(2);

        string expectedRaw = @"
POST http://www.fish-test.com/show-body.aspx?a=2&b=3 HTTP/1.1
x-aa: aa
x-bb: bb
Content-Type: text/plain; charset=gb2312
Cookie: name1=abc; name2=xyz
[BODY-IS-BIN]: 1

5Lit5Y2O5paH5piOLTUwMDDlubQ=".Trim();    // 日志中使用 base64 编码

        Assert.AreEqual(expectedRaw, rawText);

        HttpOption http2 = HttpOption.FromRawText(rawText);
        Assert.AreEqual("POST", http2.Method);
        Assert.AreEqual("http://www.fish-test.com/show-body.aspx?a=2&b=3", http2.Url);
        Assert.AreEqual(4, http2.Headers.Count);
        Assert.AreEqual("aa", http2.Headers["x-aa"]);
        Assert.AreEqual("bb", http2.Headers["x-bb"]);
        Assert.AreEqual("name1=abc; name2=xyz", http2.Headers["Cookie"]);
        Assert.AreEqual("text/plain; charset=utf-8", http2.Headers["Content-Type"]);  // charset 已纠正
        Assert.IsTrue(http2.Format == SerializeFormat.None);
        Assert.IsTrue(http2.Data.GetType() == typeof(byte[]));
        Assert.AreEqual(@"中华文明-5000年", (http2.Data as byte[]).ToUtf8String());   // 反序列化可以 “正确还原”

        string rawText2 = http2.ToRawText(2);
        Assert.AreEqual(expectedRaw.Replace("charset=gb2312", "charset=utf-8"), rawText2);


        string rawText3 = (http as ITextSerializer).ToText();
        Assert.AreEqual(expectedRaw, rawText3.Trim());

        HttpOption http3 = new HttpOption();
        (http3 as ITextSerializer).LoadData(rawText3);
        Assert.AreEqual(expectedRaw.Replace("charset=gb2312", "charset=utf-8"), http3.ToRawText(2).Trim());
    }


    [TestMethod]
    public void Test_Gzip()
    {
        HttpOption http = new HttpOption {
            Method = "POST",
            Url = "http://www.fish-test.com/show-body.aspx?a=2&b=3",
            Data = "中华文明-5000年".ToUtf8Bytes().ToGzip(),
            Format = SerializeFormat.None,
        };
        HttpOptionTest_TextSerializer.SetTestHeader(http);
        http.Headers["Content-Type"] = "text/plain";
        http.Headers["Content-Encoding"] = "gzip";

        string rawText = http.ToRawText(2);

        HttpOption http2 = HttpOption.FromRawText(rawText);

        Assert.IsTrue(http2.Data.GetType() == typeof(byte[]));
        MyAssert.AreEqual(http.Data, http2.Data);
        Assert.AreEqual("中华文明-5000年", (http2.Data as byte[]).UnGzip().ToUtf8String());   // 反序列化可以 “正确还原”

        string expectedRaw = $@"
POST http://www.fish-test.com/show-body.aspx?a=2&b=3 HTTP/1.1
x-aa: aa
x-bb: bb
Content-Type: text/plain
Content-Encoding: gzip
Cookie: name1=abc; name2=xyz
[BODY-IS-BIN]: 1

{"中华文明-5000年".ToUtf8Bytes().ToGzip().ToBase64()}".Trim();    // 二进制数据只能使用 base64 编码

        Assert.AreEqual(expectedRaw, rawText);

        Assert.AreEqual("POST", http2.Method);
        Assert.AreEqual("http://www.fish-test.com/show-body.aspx?a=2&b=3", http2.Url);
        Assert.AreEqual(5, http2.Headers.Count);
        Assert.AreEqual("aa", http2.Headers["x-aa"]);
        Assert.AreEqual("bb", http2.Headers["x-bb"]);
        Assert.AreEqual("name1=abc; name2=xyz", http2.Headers["Cookie"]);
        Assert.AreEqual("text/plain", http2.Headers["Content-Type"]);
        Assert.AreEqual("gzip", http2.Headers["Content-Encoding"]);
        Assert.IsTrue(http2.Format == SerializeFormat.None);

        string rawText2 = http2.ToRawText(2);
        Assert.AreEqual(expectedRaw, rawText2);



        string rawText3 = (http as ITextSerializer).ToText();
        Assert.AreEqual(expectedRaw, rawText3.Trim());

        HttpOption http3 = new HttpOption();
        (http3 as ITextSerializer).LoadData(rawText3);
        Assert.AreEqual(expectedRaw, http3.ToRawText(2).Trim());
    }



    [TestMethod]
    public void Test_Form()
    {
        HttpOption http = new HttpOption {
            Method = "POST",
            Url = "http://www.fish-test.com/show-body.aspx?a=2&b=3",
            Data = new {
                x = 11,
                y = 12,
                z = "abc"
            },
            Format = SerializeFormat.Form,
        };
        SetTestHeader(http);

        string rawText = http.ToRawText(2);
        //Console.WriteLine(rawText);

        string expectedRaw = @"
POST http://www.fish-test.com/show-body.aspx?a=2&b=3 HTTP/1.1
x-aa: aa
x-bb: bb
Cookie: name1=abc; name2=xyz
Content-Type: application/x-www-form-urlencoded; charset=utf-8

x=11&y=12&z=abc".Trim();

        Assert.AreEqual(expectedRaw, rawText);

        HttpOption http2 = HttpOption.FromRawText(rawText);
        Assert.AreEqual("POST", http2.Method);
        Assert.AreEqual("http://www.fish-test.com/show-body.aspx?a=2&b=3", http2.Url);
        Assert.AreEqual(4, http2.Headers.Count);
        Assert.AreEqual("aa", http2.Headers["x-aa"]);
        Assert.AreEqual("bb", http2.Headers["x-bb"]);
        Assert.AreEqual("name1=abc; name2=xyz", http2.Headers["Cookie"]);
        Assert.AreEqual("application/x-www-form-urlencoded; charset=utf-8", http2.Headers["Content-Type"]);
        Assert.IsTrue(http2.Format == SerializeFormat.None);
        Assert.AreEqual(@"x=11&y=12&z=abc", http2.Data);

        string rawText2 = http2.ToRawText(2);
        Assert.AreEqual(expectedRaw, rawText2);


        string rawText3 = (http as ITextSerializer).ToText();
        Assert.AreEqual(expectedRaw, rawText3.Trim());

        HttpOption http3 = new HttpOption();
        (http3 as ITextSerializer).LoadData(rawText3);
        Assert.AreEqual(expectedRaw, http3.ToRawText(2).Trim());
    }


    [TestMethod]
    public void Test_Multipart()
    {
        HttpOption http = new HttpOption {
            Method = "POST",
            Url = "http://www.fish-test.com/show-body.aspx?a=2&b=3",
            Data = new {
                x = 11,
                y = 12,
                z = "abc",
                f1 = new HttpFile {
                    FileName = @"d:/aa/bb.txt",
                    FileBody = "中华文明-5000年".GetBytes()
                }
            },
            Format = SerializeFormat.Multipart,
        };
        SetTestHeader(http);

        string rawText = http.ToRawText(2);
        //Console.WriteLine(rawText);

        string expectedRaw = @"
POST http://www.fish-test.com/show-body.aspx?a=2&b=3 HTTP/1.1
x-aa: aa
x-bb: bb
Cookie: name1=abc; name2=xyz
Content-Type: multipart/form-data; boundary=2c7ad4d5617d449992786e4d5d4a75ed
[BODY-IS-BIN]: 1

DQotLTJjN2FkNGQ1NjE3ZDQ0OTk5Mjc4NmU0ZDVkNGE3NWVkDQpDb250ZW50LURpc3Bvc2l0aW9uOiBmb3JtLWRhdGE7IG5hbWU9IngiDQoNCjExDQotLTJjN2FkNGQ1NjE3ZDQ0OTk5Mjc4NmU0ZDVkNGE3NWVkDQpDb250ZW50LURpc3Bvc2l0aW9uOiBmb3JtLWRhdGE7IG5hbWU9InkiDQoNCjEyDQotLTJjN2FkNGQ1NjE3ZDQ0OTk5Mjc4NmU0ZDVkNGE3NWVkDQpDb250ZW50LURpc3Bvc2l0aW9uOiBmb3JtLWRhdGE7IG5hbWU9InoiDQoNCmFiYw0KLS0yYzdhZDRkNTYxN2Q0NDk5OTI3ODZlNGQ1ZDRhNzVlZA0KQ29udGVudC1EaXNwb3NpdGlvbjogZm9ybS1kYXRhOyBuYW1lPSJmMSI7IGZpbGVuYW1lPSJiYi50eHQiDQpDb250ZW50LVR5cGU6IGFwcGxpY2F0aW9uL29jdGV0LXN0cmVhbQ0KDQrkuK3ljY7mlofmmI4tNTAwMOW5tA0KLS0yYzdhZDRkNTYxN2Q0NDk5OTI3ODZlNGQ1ZDRhNzVlZC0tDQo=
".Trim();

        Assert.AreEqual(expectedRaw, rawText);

        HttpOption http2 = HttpOption.FromRawText(rawText);
        Assert.AreEqual("POST", http2.Method);
        Assert.AreEqual("http://www.fish-test.com/show-body.aspx?a=2&b=3", http2.Url);
        Assert.AreEqual(4, http2.Headers.Count);
        Assert.AreEqual("aa", http2.Headers["x-aa"]);
        Assert.AreEqual("bb", http2.Headers["x-bb"]);
        Assert.AreEqual("name1=abc; name2=xyz", http2.Headers["Cookie"]);
        Assert.AreEqual("multipart/form-data; boundary=2c7ad4d5617d449992786e4d5d4a75ed", http2.Headers["Content-Type"]);
        Assert.IsTrue(http2.Format == SerializeFormat.None);
        Assert.IsTrue(http2.Data.GetType() == typeof(byte[]));

        string rawText2 = http2.ToRawText(2);
        Assert.AreEqual(expectedRaw, rawText2);


        string rawText3 = (http as ITextSerializer).ToText();
        Assert.AreEqual(expectedRaw, rawText3.Trim());

        HttpOption http3 = new HttpOption();
        (http3 as ITextSerializer).LoadData(rawText3);
        Assert.AreEqual(expectedRaw, http3.ToRawText(2).Trim());
    }
}
