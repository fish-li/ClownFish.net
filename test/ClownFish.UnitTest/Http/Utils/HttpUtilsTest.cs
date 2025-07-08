using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Http.Utils;
[TestClass]
public class HttpUtilsTest
{
    [TestMethod]
    public void Test_RequestHasBody()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = HttpUtils.RequestHasBody(null);
        });

        Assert.IsTrue(HttpUtils.RequestHasBody("POST"));
        Assert.IsTrue(HttpUtils.RequestHasBody("PUT"));
        Assert.IsTrue(HttpUtils.RequestHasBody("PATCH"));

        Assert.IsFalse(HttpUtils.RequestHasBody("GET"));
        Assert.IsFalse(HttpUtils.RequestHasBody("DELETE"));
        Assert.IsFalse(HttpUtils.RequestHasBody("QUERY"));
    }


    [TestMethod]
    public void Test_CanWriteResponseBody()
    {
        Assert.IsTrue(HttpUtils.CanWriteResponseBody("GET", 200));
        Assert.IsFalse(HttpUtils.CanWriteResponseBody("GET", 204));
        Assert.IsFalse(HttpUtils.CanWriteResponseBody("GET", 205));
        Assert.IsTrue(HttpUtils.CanWriteResponseBody("GET", 301));
        Assert.IsTrue(HttpUtils.CanWriteResponseBody("GET", 302));
        Assert.IsFalse(HttpUtils.CanWriteResponseBody("GET", 304));
        Assert.IsTrue(HttpUtils.CanWriteResponseBody("GET", 401));
        Assert.IsTrue(HttpUtils.CanWriteResponseBody("GET", 404));
        Assert.IsTrue(HttpUtils.CanWriteResponseBody("GET", 500));

        Assert.IsTrue(HttpUtils.CanWriteResponseBody("POST", 200));
        Assert.IsFalse(HttpUtils.CanWriteResponseBody("POST", 204));
        Assert.IsFalse(HttpUtils.CanWriteResponseBody("POST", 205));
        Assert.IsTrue(HttpUtils.CanWriteResponseBody("POST", 301));
        Assert.IsTrue(HttpUtils.CanWriteResponseBody("POST", 302));
        Assert.IsFalse(HttpUtils.CanWriteResponseBody("POST", 304));
        Assert.IsTrue(HttpUtils.CanWriteResponseBody("POST", 401));
        Assert.IsTrue(HttpUtils.CanWriteResponseBody("POST", 404));
        Assert.IsTrue(HttpUtils.CanWriteResponseBody("POST", 500));

        Assert.IsFalse(HttpUtils.CanWriteResponseBody("HEAD", 200));
        Assert.IsFalse(HttpUtils.CanWriteResponseBody("HEAD", 204));
        Assert.IsFalse(HttpUtils.CanWriteResponseBody("HEAD", 205));
        Assert.IsFalse(HttpUtils.CanWriteResponseBody("HEAD", 301));
        Assert.IsFalse(HttpUtils.CanWriteResponseBody("HEAD", 302));
        Assert.IsFalse(HttpUtils.CanWriteResponseBody("HEAD", 304));
        Assert.IsFalse(HttpUtils.CanWriteResponseBody("HEAD", 401));
        Assert.IsFalse(HttpUtils.CanWriteResponseBody("HEAD", 404));
        Assert.IsFalse(HttpUtils.CanWriteResponseBody("HEAD", 500));
    }

    [TestMethod]
    public void Test_RequestBodyIsText()
    {
        Assert.IsFalse(HttpUtils.RequestBodyIsText(""));

        Assert.IsTrue(HttpUtils.RequestBodyIsText("text/plain"));
        Assert.IsTrue(HttpUtils.RequestBodyIsText("text/css"));
        Assert.IsTrue(HttpUtils.RequestBodyIsText("application/json"));
        Assert.IsTrue(HttpUtils.RequestBodyIsText("application/xml"));
        Assert.IsTrue(HttpUtils.RequestBodyIsText("application/x-www-form-urlencoded"));

        Assert.IsFalse(HttpUtils.RequestBodyIsText("multipart/form-data"));
        Assert.IsFalse(HttpUtils.RequestBodyIsText("application/octet-stream"));
    }

    [TestMethod]
    public void Test_ResponseBodyIsText()
    {
        Assert.IsFalse(HttpUtils.ResponseBodyIsText(""));

        Assert.IsTrue(HttpUtils.ResponseBodyIsText("text/plain"));
        Assert.IsTrue(HttpUtils.ResponseBodyIsText("text/css"));
        Assert.IsTrue(HttpUtils.ResponseBodyIsText("application/json"));
        Assert.IsTrue(HttpUtils.ResponseBodyIsText("application/xml"));

        Assert.IsTrue(HttpUtils.ResponseBodyIsText("application/problem+json"));
        Assert.IsTrue(HttpUtils.ResponseBodyIsText("application/x-ndjson"));

        Assert.IsFalse(HttpUtils.ResponseBodyIsText("application/x-www-form-urlencoded"));  // Response根本不使用这个类型
        Assert.IsFalse(HttpUtils.ResponseBodyIsText("multipart/form-data"));
        Assert.IsFalse(HttpUtils.ResponseBodyIsText("application/octet-stream"));
    }

    [TestMethod]
    public void Test_GetStatusReasonPhrase()
    {
        StringBuilder sb = new StringBuilder();
        for( int i = 99; i <= 999; i++ ) {
            string text = HttpUtils.GetStatusReasonPhrase(i);
            sb.AppendLineRN($"{i}: {text}");
            Assert.IsTrue(text.Length > 0);
        }

        string all = sb.ToString();
        Console.WriteLine(all);

        // 这里就抽几个做断言
        Assert.IsTrue(all.Contains("200: OK"));
        Assert.IsTrue(all.Contains("500: Internal Server Error"));
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

        Assert.AreEqual(0, HttpUtils.ParseContentType(null, out string media, out Encoding encoding));

        Assert.AreEqual(1, HttpUtils.ParseContentType("xxxxxx", out string media1, out Encoding encoding1));
        Assert.AreEqual("xxxxxx", media1);
        Assert.IsNull(encoding1);

        Assert.AreEqual(1, HttpUtils.ParseContentType("text/html", out string media2, out Encoding encoding2));
        Assert.AreEqual("text/html", media2);
        Assert.IsNull(encoding2);

        Assert.AreEqual(1, HttpUtils.ParseContentType("multipart/form-data; boundary=something", out string media3, out Encoding encoding3));
        Assert.AreEqual("multipart/form-data", media3);
        Assert.IsNull(encoding3);

        // 中间的分隔符错误
        Assert.AreEqual(1, HttpUtils.ParseContentType("text/html: charset=utf-8", out string media4, out Encoding encoding4));
        Assert.AreEqual("text/html: charset=utf-8", media4);
        Assert.IsNull(encoding4);


        Assert.AreEqual(2, HttpUtils.ParseContentType("text/html; charset=utf-8", out string media5, out Encoding encoding5));
        Assert.AreEqual("text/html", media5);
        Assert.IsTrue(encoding5 == Encoding.UTF8);


        Assert.AreEqual(2, HttpUtils.ParseContentType("text/html;charset=utf-8", out string media6, out Encoding encoding6));
        Assert.AreEqual("text/html", media6);
        Assert.IsTrue(encoding6 == Encoding.UTF8);

        Assert.AreEqual(2, HttpUtils.ParseContentType("text/html;charset=UTF-8", out string media7, out Encoding encoding7));
        Assert.AreEqual("text/html", media7);
        Assert.IsTrue(encoding7 == Encoding.UTF8);

        Assert.AreEqual(2, HttpUtils.ParseContentType("text/html; CHARset=utf-8", out string media8, out Encoding encoding8));
        Assert.AreEqual("text/html", media8);
        Assert.IsTrue(encoding8 == Encoding.UTF8);

        Assert.AreEqual(2, HttpUtils.ParseContentType("text/html; charset=gb2312", out string media9, out Encoding encoding9));
        Assert.AreEqual("text/html", media9);
        Assert.IsTrue(encoding9 == Encoding.GetEncoding("GB2312"));


        Assert.AreEqual(2, HttpUtils.ParseContentType("text/html;Charset=\"utf-8\"", out string media10, out Encoding encoding10));
        Assert.AreEqual("text/html", media10);
        Assert.IsTrue(encoding10 == Encoding.UTF8);

        Assert.AreEqual(2, HttpUtils.ParseContentType("text/html; charset=\"utf-8\"", out string media11, out Encoding encoding11));
        Assert.AreEqual("text/html", media11);
        Assert.IsTrue(encoding11 == Encoding.UTF8);

        //Assert.AreEqual(1, HttpUtils.ParseContentType("text/html;", out string media12, out Encoding encoding12));
        //Assert.AreEqual("text/html", media12);
        //Assert.IsNull(encoding12);

        Assert.AreEqual(1, HttpUtils.ParseContentType("text/html; ", out string media13, out Encoding encoding13));
        Assert.AreEqual("text/html", media13);
        Assert.IsNull(encoding13);


        Assert.AreEqual(1, HttpUtils.ParseContentType("text/html; xx", out string media14, out Encoding encoding14));
        Assert.AreEqual("text/html", media14);
        Assert.IsNull(encoding14);

        Assert.AreEqual(1, HttpUtils.ParseContentType("text/html; Chartset=", out string media15, out Encoding encoding15));
        Assert.AreEqual("text/html", media15);
        Assert.IsNull(encoding15);

        Assert.AreEqual(1, HttpUtils.ParseContentType("text/html; Chartset= ", out string media16, out Encoding encoding16));
        Assert.AreEqual("text/html", media16);
        Assert.IsNull(encoding16);

        Assert.AreEqual(1, HttpUtils.ParseContentType("text/html; Chartset=xxx", out string media17, out Encoding encoding17));
        Assert.AreEqual("text/html", media17);
        Assert.IsNull(encoding17);

    }

}
