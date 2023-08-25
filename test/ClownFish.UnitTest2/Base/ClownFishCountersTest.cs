﻿using System;
using System.Collections.Generic;
using System.Text;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base
{
    [TestClass]
    public class ClownFishCountersTest
    {
        [TestMethod]
        public void Test()
        {
            ClownFishCounters.ResetAll();
            Assert.AreEqual(0L, ClownFishCounters.Console2.Warnning.Get());
            Assert.AreEqual(0L, ClownFishCounters.Logging.WriteCount.Get());


            ClownFishCounters.Console2.Warnning.Increment();
            Assert.AreEqual(1L, ClownFishCounters.Console2.Warnning.Get());

            ClownFishCounters.Logging.WriteCount.Increment();
            Assert.AreEqual(1L, ClownFishCounters.Logging.WriteCount.Get());


            ClownFishCounters.ResetAll();
            Assert.AreEqual(0L, ClownFishCounters.Console2.Warnning.Get());
            Assert.AreEqual(0L, ClownFishCounters.Logging.WriteCount.Get());
        }
    }
}
