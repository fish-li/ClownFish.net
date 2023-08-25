using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.UnitTest.Base.WebClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Http
{
    [TestClass]
    public class ResponseUtilsTest
    {
        [TestMethod]
        public void Test_GetAllHeaders()
        {
            MyAssert.IsError<ArgumentNullException>(() => {
                _ = ResponseUtils.GetAllHeaders((HttpWebResponse)null);
            });


            using( HttpWebResponse response = HttpResultTest.CreateHttpWebResponse() ) {

                NameValueCollection headers = response.GetAllHeaders();
                Assert.IsNotNull(headers);
                Assert.AreEqual("OK", headers["x-status"]);

                string[] cookies = headers.GetValues("Set-Cookie");
                Assert.AreEqual(2, cookies.Length);
                Assert.IsTrue(cookies[0].StartsWith("c1=; expires="));
                Assert.IsTrue(cookies[1].StartsWith("c2=xxxxxxx; expires="));
            }
        }


    }
}
