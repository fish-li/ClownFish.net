using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.WebClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Base.UnitTest.WebClient
{
    [TestClass]
    public class HttpOptionTest
    {
        [TestMethod]
        public void Test_HttpOption_FromRawText()
        {
            string text = @"
POST http://www.fish-web-demo.com/api/ns/TestAutoAction/submit.aspx HTTP/1.1
Host: www.fish-web-demo.com
User-Agent: Mozilla/5.0 (Windows NT 6.3; WOW64; rv:36.0) Gecko/20100101 Firefox/36.0
Accept: */*
Accept-Language: zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3
Accept-Encoding: gzip, deflate
Content-Type: application/x-www-form-urlencoded; charset=UTF-8
X-Requested-With: XMLHttpRequest
Referer: http://www.fish-web-demo.com/Pages/Demo/TestAutoFindAction.htm
Content-Length: 72
Cookie: hasplmlang=_int_; LoginBy=productKey; PageStyle=Style2;
Connection: keep-alive
Pragma: no-cache
Cache-Control: no-cache

input=Fish+Li&Base64=%E8%BD%AC%E6%8D%A2%E6%88%90Base64%E7%BC%96%E7%A0%81
";

            HttpOption option = HttpOption.FromRawText(text);

            Assert.AreEqual("http://www.fish-web-demo.com/api/ns/TestAutoAction/submit.aspx", option.GetRequestUrl());
            Assert.AreEqual("input=Fish+Li&Base64=%E8%BD%AC%E6%8D%A2%E6%88%90Base64%E7%BC%96%E7%A0%81", option.GetPostData().ToString());
            Assert.AreEqual("application/x-www-form-urlencoded", option.ContentType);
        }


        [TestMethod]
        public void Test_HttpOption_GET()
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
        public void Test_HttpOption_POST()
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
    }
}
