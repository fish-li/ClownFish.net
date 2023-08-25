using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.WebClient
{
    [TestClass]
    public class HttpUtilsTest
    {
        [TestMethod]
        public void Test_RequestHasBody()
        {
            MyAssert.IsError<ArgumentNullException>(()=> {
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
        public void Test_ContentIsText()
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
    }
}
