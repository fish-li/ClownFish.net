using System;
using System.Collections.Generic;
using System.Text;
using ClownFish.Base.WebClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.WebClient
{
    [TestClass]
    public class ArgsTest
    {
        public ArgsTest()
        {

        }

        [TestMethod]
        public void Test1()
        {
            HttpOption option = new HttpOption {
                Url = "http://www.fish-test.com/test1.aspx"
            };

            BeforeSendEventArgs arg1 = new BeforeSendEventArgs();
            arg1.OperationId = Guid.NewGuid().ToString();
            arg1.HttpOption = option;

            Assert.AreEqual(36, arg1.OperationId.Length);
            Assert.AreEqual("http://www.fish-test.com/test1.aspx", arg1.HttpOption.Url);
        }

        [TestMethod]
        public void Test2()
        {
            HttpOption option = new HttpOption {
                Url = "http://www.fish-test.com/test1.aspx"
            };
            DateTime time = DateTime.Now;

            RequestFinishedEventArgs arg2 = new RequestFinishedEventArgs();
            arg2.OperationId = Guid.NewGuid().ToString();
            arg2.HttpOption = option;
            arg2.Request = null;
            arg2.Response = null;
            arg2.Exception = ExceptionHelper.CreateException();
            arg2.StartTime = time;
            arg2.EndTime = time.AddSeconds(3);


            Assert.AreEqual(36, arg2.OperationId.Length);
            Assert.AreEqual("http://www.fish-test.com/test1.aspx", arg2.HttpOption.Url);
            Assert.IsNull(arg2.Request);
            Assert.IsNull(arg2.Response);
            Assert.IsNotNull(arg2.Exception);
            Assert.AreEqual(time, arg2.StartTime);
            Assert.AreEqual(time.AddSeconds(3), arg2.EndTime);

            string text = arg2.ToLoggingText();
            Assert.IsTrue(text.StartsWith("GET http://www.fish-test.com/test1.aspx"));
        }
    }
}
