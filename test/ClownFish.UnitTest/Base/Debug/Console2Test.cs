using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Debug
{
    [TestClass]
    public class Console2Test
    {
        [TestMethod]
        public void Test_Error()
        {
            var ex0 = ExceptionHelper.CreateException();

            long count = ClownFishCounters.Console2.Error;
            Console2.Error("0123456789", ex0);
            Assert.AreEqual(count + 1, ClownFishCounters.Console2.Error);



            count = ClownFishCounters.Console2.Error;
            Console2.Error(ex0);
            Assert.AreEqual(count + 1, ClownFishCounters.Console2.Error);


            count = ClownFishCounters.Console2.Error;
            Console2.Error(null);
            Assert.AreEqual(count, ClownFishCounters.Console2.Error);
        }


        [TestMethod]
        public void Test_Warnning()
        {
            var ex0 = ExceptionHelper.CreateException();

            long count = ClownFishCounters.Console2.Warnning;
            Console2.Warnning(ex0);
            Assert.AreEqual(count + 1, ClownFishCounters.Console2.Warnning);


            count = ClownFishCounters.Console2.Warnning;
            Console2.Warnning((Exception)null);
            Assert.AreEqual(count, ClownFishCounters.Console2.Warnning);


            count = ClownFishCounters.Console2.Warnning;
            Console2.Warnning("0123456789");
            Assert.AreEqual(count + 1, ClownFishCounters.Console2.Warnning);

            Console2.Warnning((Exception)null);
            Console2.Warnning((string)null);
            Console2.Warnning(string.Empty);
        }

        [TestMethod]
        public void Test_Info()
        {
            Assert.IsTrue(Console2.InfoEnabled);

            Console2.Info(null);
            Console2.Info(string.Empty);
            Console2.Info("0123456789");

            Console2.WriteSeparatedLine();
        }

        [TestMethod]
        public void Test_Debug()
        {
            Console2.Debug(null);
            Console2.Debug(string.Empty);
            Console2.Debug("0123456789");
        }



        [TestMethod]
        public void Test_ShowHTTP()
        {
            HttpOption request = new HttpOption {
                Method = "POST2",
                Url = "http://www.abc.com/aa/bb/cc.aspx",
                Header = new {
                    x_header1 = "123",
                    x_header2 = "abc"
                },
                Format = SerializeFormat.Form,
                Data = new {
                    id = 2,
                    name = "1qaz"
                }
            };

            string text = @"x-RequestId: 65a09b23a7644aa7a27a641a43a5c657
x-HostName: 1b5b8b3075a2
x-AppName: XDemo.WebSiteApp
x-dotnet: .NET 6.0.5
x-Nebula: 6.22.615.100
x-req-url: /v20/api/WebSiteApp/perftest/database2?provider=MySqlConnector&tenantId=596c871f21722
x-ReuestCount: 2439508
x-PreRequestExecute-ThreadId: 41
x-PostRequestExecute-ThreadId: 40
x-result-datatype: String
Content-Length: 103
Content-Type: text/plain; charset=utf-8
Date: Wed, 15 Jun 2022 11:03:38 GMT
Server: Kestrel";

            NameValueCollection headerCollection = text.ToHeaderCollection();
            HttpResult<string> response = new (200, headerCollection, "d495fe273bf1492c953900fb9738ac30");

            Console2.ShowHTTP(request, response, true);
            Console2.ShowHTTP(request, response, false);
        }
    }
}
