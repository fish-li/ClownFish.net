using System.Runtime.InteropServices;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace ClownFish.UnitTest.WebClient;

[TestClass]
public class HttpOptionTest
{
    public static readonly string TestUrl = "http://www.fish-test.com/test1.aspx";

    [TestMethod]
    public void Test()
    {
        HttpOption option = new HttpOption {
            Url = TestUrl
        };

        Assert.AreEqual("GET", option.Method);
        Assert.AreEqual(TestUrl, option.Url);
        Assert.AreEqual(SerializeFormat.Form, option.Format);

        MyAssert.IsError<ArgumentNullException>(()=> {
            option.Method = "";
        });

        option.UserAgent = "u123";
        Assert.AreEqual("u123", option.UserAgent);

        Assert.IsFalse(option.AllowAutoRedirect.HasValue);
        option.AllowAutoRedirect = true;
        Assert.IsTrue(option.AllowAutoRedirect.Value);


        Assert.IsNull(option.Data);
        option.Data = "123";
        Assert.AreEqual("123", option.Data);

        Assert.IsNull(option.Cookie);
        option.Cookie = new System.Net.CookieContainer();
        Assert.IsNotNull(option.Cookie);

        option.SetCookieHeader("cc=xxxx");
        Assert.AreEqual("cc=xxxx", option.Headers["Cookie"]);

        // 下面是一些无聊代码……
        Assert.IsNull(option.Credentials);
        option.Credentials = null;
    }


    [TestMethod]
    public void Test_ReUse()
    {
        HttpOption option = new HttpOption {
            Url = TestUrl
        };

        string html1 = option.GetResult();
        Assert.IsTrue(html1.Contains("Hello Test!"));


        MyAssert.IsError<InvalidOperationException>(() => {
            option.Url = "https://cn.bing.com/";
            string html2 = option.GetResult<string>();
        });
    }

    [TestMethod]
    public void Test_Header()
    {
        HttpOption option = new HttpOption {
            Url = TestUrl
        };

        MyAssert.IsError<ArgumentNullException>(() => {
            option.Headers = null;
        });
        MyAssert.IsError<ArgumentNullException>(() => {
            option.Header = null;
        });

        object h = typeof(HttpOption).InvokeMember("_headers", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField, null, option, null);
        Assert.IsNull(h);

        HttpHeaderCollection h2 = option.Headers;
        Assert.IsNotNull(h2);
        Assert.AreEqual(0, h2.Count);

        option.Header = new {
            a_1 = "11",
            b = 22,
            c = (string)null
        };
        HttpHeaderCollection h3 = option.Headers;
        Assert.IsNotNull(h3);
        Assert.AreEqual(3, h3.Count);
        Assert.AreEqual("11", h3["a-1"]);
        Assert.AreEqual("22", h3["b"]);
        Assert.AreEqual("", h3["c"]);
    }


    [TestMethod]
    public void Test_Url()
    {
        HttpOption option = new HttpOption();

        MyAssert.IsError<ArgumentNullException>(() => {
            option.CheckInput();
        });

        option.Data = new {
            a = 2,
            b = "abc"
        };

        option.Url = "http://www.fish-test.com/test1.aspx";
        Assert.AreEqual("http://www.fish-test.com/test1.aspx?a=2&b=abc", option.GetRequestUrl());

        option.Url = "http://www.fish-test.com/test1.aspx?x=11";
        Assert.AreEqual("http://www.fish-test.com/test1.aspx?x=11&a=2&b=abc", option.GetRequestUrl());


        Assert.IsNull(HttpOption.GetQueryString(null));
        Assert.AreEqual("xxx", HttpOption.GetQueryString("xxx"));

        Assert.IsNull(option.GetPostData());

    }

    private static readonly string s_requestRaw = @"
POST http://www.fish-test.com/api/ns/TestAutoAction/submit.aspx HTTP/1.1
Host: www.fish-test.com
User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; rv:36.0) Gecko/20100101 Firefox/36.0
Accept: */*
Accept-Language: zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3
Accept-Encoding: gzip, deflate
Content-Type: application/x-www-form-urlencoded; charset=UTF-8
X-Requested-With: XMLHttpRequest
Referer: http://www.fish-test.com/Pages/Demo/TestAutoFindAction.htm
Content-Length: 72
Cookie: hasplmlang=_int_; LoginBy=productKey; PageStyle=Style2;
Connection: keep-alive
Pragma: no-cache
Cache-Control: no-cache

input=Fish+Li&Base64=%E8%BD%AC%E6%8D%A2%E6%88%90Base64%E7%BC%96%E7%A0%81
";

    [TestMethod]
    public void Test_FromRawText()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = HttpOption.FromRawText(null);
        });
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = HttpOption.FromRawText("");
        });
                    

        HttpOption option = HttpOption.FromRawText(s_requestRaw);

        Assert.AreEqual("http://www.fish-test.com/api/ns/TestAutoAction/submit.aspx", option.GetRequestUrl());
        Assert.AreEqual("input=Fish+Li&Base64=%E8%BD%AC%E6%8D%A2%E6%88%90Base64%E7%BC%96%E7%A0%81", option.GetPostData().ToString());
        Assert.AreEqual("application/x-www-form-urlencoded", option.Headers["Content-Type"]);

        

        string text2 = option.ToAllText();
        Assert.IsTrue(text2.Contains("POST http://www.fish-test.com/api/ns/TestAutoAction/submit.aspx HTTP/1.1"));
        Assert.IsTrue(text2.Contains("Host: www.fish-test.com"));
        Assert.IsTrue(text2.Contains("User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; rv:36.0) Gecko/20100101 Firefox/36.0"));
        Assert.IsTrue(text2.Contains("Accept: */*"));
        Assert.IsTrue(text2.Contains("Accept-Language: zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3"));
        Assert.IsTrue(text2.Contains("Accept-Encoding: gzip, deflate"));
        Assert.IsTrue(text2.Contains("Content-Type: application/x-www-form-urlencoded"));
        Assert.IsTrue(text2.Contains("X-Requested-With: XMLHttpRequest"));
        Assert.IsTrue(text2.Contains("Referer: http://www.fish-test.com/Pages/Demo/TestAutoFindAction.htm"));
        Assert.IsTrue(text2.Contains("Cookie: hasplmlang=_int_; LoginBy=productKey; PageStyle=Style2;"));
        Assert.IsTrue(text2.Contains("Pragma: no-cache"));
        Assert.IsTrue(text2.Contains("Cache-Control: no-cache"));
        Assert.IsTrue(text2.Contains("input=Fish+Li&Base64=%E8%BD%AC%E6%8D%A2%E6%88%90Base64%E7%BC%96%E7%A0%81"));
    }


    [TestMethod]
    public void Test_ToRawText_TextBody()
    {
        HttpOption option = new HttpOption {
            Method = "POST",
            Url = "http://www.fish-test.com/api/ns/TestAutoAction/submit.aspx",
            Format = SerializeFormat.Form,
            UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:36.0) Gecko/20100101 Firefox/36.0",
            Header = new {
                Accept = "*/*",
                Accept_Language = "zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3",
                X_Requested_With = "XMLHttpRequest"
            },
            Data = new {
                a = "中文汉字",
                b = "<#>&*^%235",
                c = 22
            }
        };

        string text = option.ToRawText();
        Console.WriteLine(text);

        Assert.IsTrue(text.Contains("POST http://www.fish-test.com/api/ns/TestAutoAction/submit.aspx HTTP/1.1"));
        Assert.IsTrue(text.Contains("User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; rv:36.0) Gecko/20100101 Firefox/36.0"));
        Assert.IsTrue(text.Contains("Accept: */*"));
        Assert.IsTrue(text.Contains("Accept-Language: zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3"));
        Assert.IsTrue(text.Contains("X-Requested-With: XMLHttpRequest"));
        Assert.IsTrue(text.Contains("Content-Type: application/x-www-form-urlencoded"));
        Assert.IsTrue(text.Contains("a=%e4%b8%ad%e6%96%87%e6%b1%89%e5%ad%97&b=%3c%23%3e%26*%5e%25235&c=22"));
    }


    [TestMethod]
    public void Test_ToRawText_BinBody()
    {
        byte[] bytes = Guid.NewGuid().ToByteArray();

        HttpOption option = new HttpOption {
            Method = "POST",
            UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:36.0) Gecko/20100101 Firefox/36.0",
            Url = "http://www.fish-test.com/api/ns/TestAutoAction/submit.aspx",
            Format = SerializeFormat.Binary,
            Data = bytes
        };

        string text = option.ToRawText(2);
        Console.WriteLine(text);

        Assert.IsTrue(text.Contains("POST http://www.fish-test.com/api/ns/TestAutoAction/submit.aspx HTTP/1.1"));
        Assert.IsTrue(text.Contains("User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; rv:36.0) Gecko/20100101 Firefox/36.0"));
        Assert.IsTrue(text.Contains("Content-Type: application/octet-stream"));
        Assert.IsTrue(text.Contains("已将二进制数据转成Base64字符串：" + bytes.ToBase64()));
    }

    [TestMethod]
    public void Test_ToRawText_NoBody()
    {
        HttpOption option = new HttpOption {
            Method = "GET",
            Url = "http://www.fish-test.com/api/ns/TestAutoAction/submit.aspx",
            UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:36.0) Gecko/20100101 Firefox/36.0",
            Format = SerializeFormat.Binary,  // 会忽略这个头！
        };

        string text = option.ToRawText();
        Console.WriteLine(text);

        Assert.IsTrue(text.Contains("GET http://www.fish-test.com/api/ns/TestAutoAction/submit.aspx HTTP/1.1"));
        Assert.IsTrue(text.Contains("User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; rv:36.0) Gecko/20100101 Firefox/36.0"));
    }

    [TestMethod]
    public void Test_GET()
    {
        var option = new HttpOption {
            Method = "GET",
            Url = "http://www.abc.com/test.aspx",
            Data = new { a = 1, b = 2, c = "Fish Li" },
            Headers = new Dictionary<string, string>() {
                            { "X-Requested-With", "XMLHttpRequest" },
                            { "User-Agent", "Mozilla/5.0"} }
        };


        Assert.AreEqual("http://www.abc.com/test.aspx?a=1&b=2&c=Fish+Li", option.GetRequestUrl());
        Assert.IsNull(option.GetPostData());

        Assert.AreEqual(2, option.Headers.Count);
        Assert.AreEqual("X-Requested-With", option.Headers[0].Name);
        Assert.AreEqual("XMLHttpRequest", option.Headers[0].Value);

        Assert.AreEqual("User-Agent", option.Headers[1].Name);
        Assert.AreEqual("Mozilla/5.0", option.Headers[1].Value);



        option = new HttpOption {
            Method = "GET",
            Url = "http://www.abc.com/test.aspx?xx=5",
            Data = new { a = 1, b = 2, c = "Fish Li" }
        };
        Assert.AreEqual("http://www.abc.com/test.aspx?xx=5&a=1&b=2&c=Fish+Li", option.GetRequestUrl());
    }


    [TestMethod]
    public void Test_POST()
    {
        var option = new HttpOption {
            Method = "POST",
            Url = "http://www.abc.com/test.aspx",
            Data = new { a = 1, b = 2, c = "Fish Li" },
            Headers = new Dictionary<string, string>() {
                            { "X-Requested-With", "XMLHttpRequest" },
                            { "User-Agent", "Mozilla/5.0"} }
        };


        Assert.AreEqual("http://www.abc.com/test.aspx", option.GetRequestUrl());
        Assert.IsNotNull(option.GetPostData());

        Assert.AreEqual(2, option.Headers.Count);
        Assert.AreEqual("X-Requested-With", option.Headers[0].Name);
        Assert.AreEqual("XMLHttpRequest", option.Headers[0].Value);

        Assert.AreEqual("User-Agent", option.Headers[1].Name);
        Assert.AreEqual("Mozilla/5.0", option.Headers[1].Value);



        option = new HttpOption {
            Method = "POST",
            Url = "http://www.abc.com/test.aspx?xx=5",
            Data = new { a = 1, b = 2, c = "Fish Li" }
        };
        Assert.AreEqual("http://www.abc.com/test.aspx?xx=5", option.GetRequestUrl());
        Assert.IsNotNull(option.GetPostData());
    }


    [TestMethod]
    public void Test_HttpOptionBuilder()
    {
        HttpOption option = new HttpOption();

        string text1 = "http://www.fish-test.com/api/ns/TestAutoAction/submit.aspx";
        string text2 = "POST http://www.fish-test.com/api/ns/TestAutoAction/submit.aspx";
        string text3 = "POST http://www.fish-test.com/api/ns/TestAutoAction/submit.aspx HTTP15";

        var ex1 = MyAssert.IsError<ArgumentException>(() => {
            HttpOptionBuilder.SetRequestLine(option, "http://www.fish-test.com/api/ns/TestAutoAction/submit.aspx");
        });
        Assert.AreEqual($"不能识别的请求文本格式，开始行：[{text1}]", ex1.Message);

        var ex2 = MyAssert.IsError<ArgumentException>(() => {
            HttpOptionBuilder.SetRequestLine(option, "POST http://www.fish-test.com/api/ns/TestAutoAction/submit.aspx");
        });
        Assert.AreEqual($"不能识别的请求文本格式，开始行：[{text2}]", ex2.Message);

        var ex3 = MyAssert.IsError<ArgumentException>(() => {
            HttpOptionBuilder.SetRequestLine(option, "POST http://www.fish-test.com/api/ns/TestAutoAction/submit.aspx HTTP15");
        });
        Assert.AreEqual($"不能识别的请求文本格式，开始行：[{text3}]", ex3.Message);


        string rawText1 = @"
GET http://www.fish-test.com/api/ns/TestAutoAction/submit.aspx HTTP/1.1
Host=www.fish-test.com
";
        var ex4 = MyAssert.IsError<ArgumentException>(() => {
            _ = HttpOption.FromRawText(rawText1);
        });
        Assert.AreEqual($"不能识别的请求文本格式，请求头：[Host=www.fish-test.com]", ex4.Message);

    }


#if NET6_0_OR_GREATER
    [TestMethod]
    public void Test_UnixSocketEndPoint()
    {
        if( RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ) {
            MyAssert.IsError<NotSupportedException>(() => {
                HttpOption httpOption = new HttpOption();
                httpOption.UnixSocketEndPoint = "xxx.sock";
            });
        }
    }


    [TestMethod]
    public void Test_xx()
    {
        var option = new HttpOption();
        option.MockResult = new object();
        Assert.IsNotNull(option.MockResult);

        option.CompletionOption = System.Net.Http.HttpCompletionOption.ResponseHeadersRead;
        Assert.AreEqual(System.Net.Http.HttpCompletionOption.ResponseHeadersRead, option.CompletionOption);
        
        option.MessageHandler = null;
        Assert.IsNull(option.MessageHandler);

        option.AutoDecompressResponse = true;
        Assert.IsTrue(option.AutoDecompressResponse);

        option.OnSetRequest = null;
        Assert.IsNull(option.OnSetRequest);
    }

#endif

    [TestMethod]
    public void Test_GetPostBodyAsString()
    {
        HttpOption option = new HttpOption {
            Method = "POST",
            Url = "http://www.fish-test.com/test1.aspx",
            Data = "abc",
            Format = SerializeFormat.Text
        };

        string text0 = (string)option.InvokeMethod("GetPostBodyAsString", 0);
        Assert.IsNull(text0);

        option.Data = "abc".ToUtf8Bytes();
        option.Format = SerializeFormat.Binary;

        string text1 = (string)option.InvokeMethod("GetPostBodyAsString", 1);
        Assert.AreEqual("##--非文本类数据，长度：(3)--##", text1);


        string text2 = (string)option.InvokeMethod("GetPostBodyAsString", 2);
        Assert.AreEqual("已将二进制数据转成Base64字符串：YWJj", text2);
    }


    [TestMethod]
    public void Test_FillLineAndHeaders_Cookie()
    {
        HttpOption option = new HttpOption {
            Method = "POST",
            Url = "http://www.fish-test.com/test1.aspx",
            Data = "abc",
            Format = SerializeFormat.Text
        };

        CookieContainer cookieContainer = new CookieContainer();
        Uri uri = new Uri(option.Url);
        cookieContainer.Add(uri, new Cookie("name1", "abc"));

        option.Cookie = cookieContainer;

        StringBuilder sb = new StringBuilder();
        option.InvokeMethod("FillLineAndHeaders", sb);

        string text = sb.ToString();
        Console.WriteLine(sb.ToString());

        Assert.IsTrue(text.Contains("Cookie: name1=abc"));
        
    }

#if NETCOREAPP

    [TestMethod]
    public void Test_timeout()
    {
        HttpOption option = new HttpOption {
            Url = "http://127.0.0.1:30000/test1.aspx",
            Timeout = 30
        };

        Exception ex = null;
        try {
            option.Send();
        }
        catch(Exception ex2 ) {
            ex = ex2;
        }

        Assert.IsNotNull(ex);

        string exAll = ex.ToString();
        Console.WriteLine(exAll);


        Assert.IsInstanceOfType(ex, typeof(TaskCanceledException));    

        Assert.IsTrue(exAll.Contains("HTTP call timeout, destination address: http://127.0.0.1:30000/test1.aspx"));
    }
#endif

}
