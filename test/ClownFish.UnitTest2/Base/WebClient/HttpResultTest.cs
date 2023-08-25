using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Base.WebClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.WebClient
{
    [TestClass]
    public class HttpResultTest
    {
        [TestMethod]
        public void Test()
        {
            HttpResult<string> result2 = new (600, new NameValueCollection(), "abc");
            Assert.AreEqual(600, result2.StatusCode);
            Assert.AreEqual("abc", result2.Result);
            Assert.IsNotNull(result2.Headers);
            Assert.AreEqual(0, result2.Headers.Count);
        }


        [TestMethod]
        public void Test2()
        {
            using( HttpWebResponse response = CreateHttpWebResponse() ) {

                HttpResult<string> result = response.GetResult();

                string text = result.ToAllText();

                Console.WriteLine(text);

                Assert.IsTrue(text.Contains("x-status: OK"));
                Assert.IsTrue(text.Contains("Set-Cookie: c1=; expires="));
                Assert.IsTrue(text.Contains("Set-Cookie: c2=xxxxxxx; expires="));
                Assert.IsTrue(text.Contains("Content-Type: text/html; charset=utf-8"));
            }
        }


        internal static HttpWebResponse CreateHttpWebResponse(string url = null)
        {
            HttpOption option = new HttpOption {
                Url = url ?? HttpOptionTest.TestUrl
            };

            return option.GetResult<HttpWebResponse>();
        }
    }
}
