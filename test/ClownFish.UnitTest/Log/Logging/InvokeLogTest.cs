using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Log.Logging;
using ClownFish.Log.Models;
using ClownFish.UnitTest.Http.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Log.Logging
{
    [TestClass]
    public class InvokeLogTest
    {
        [TestMethod]
        public void Test()
        {
            InvokeLog log = new InvokeLog();

            log.AppName = "e122610af9ba49f486eb14d8e155b675";
            log.ProcessId = "d8cec8b7e81b44cc82275ef64041707b";
            log.ActionType = 100;
            log.StartTime = new DateTime(2023, 1, 1, 2, 2, 2);
            log.ExecuteTime = TimeSpan.FromSeconds(5);
            log.IsLongTask = 1;
            log.IsSlow = 1;
            log.Status = 200;
            log.Title = "39eee30f9fe84372910cebcbbed30515";

            Assert.AreEqual("e122610af9ba49f486eb14d8e155b675", log.AppName);
            Assert.AreEqual("d8cec8b7e81b44cc82275ef64041707b", log.ProcessId);
            Assert.AreEqual(100, log.ActionType);
            Assert.AreEqual(new DateTime(2023, 1, 1, 2, 2, 2), log.StartTime);
            Assert.AreEqual(TimeSpan.FromSeconds(5), log.ExecuteTime);
            Assert.IsTrue(log.IsLongTask == 1);
            Assert.IsTrue(log.IsSlow ==1);
            Assert.AreEqual(200, log.Status);
            Assert.AreEqual("39eee30f9fe84372910cebcbbed30515", log.Title);
            Assert.AreEqual("39eee30f9fe84372910cebcbbed30515", log.ToString());
        }
    }
}
