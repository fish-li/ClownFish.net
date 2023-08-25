﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Log.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base
{
    [TestClass]
    public class BasePipelineContextTest
    {
        private class XxxPipelineContext : BasePipelineContext
        {
        }


        [TestMethod]
        public void Test()
        {
            XxxPipelineContext ctx = new XxxPipelineContext();
            ctx.PerformanceThresholdMs = 50;

            Assert.AreEqual(200, ctx.GetStatus());
            Assert.AreEqual(32, ctx.ProcessId.Length);
            Assert.AreEqual(50, ctx.PerformanceThresholdMs);
            Assert.AreEqual(string.Empty, ctx.GetTitle());
            Assert.IsNull(ctx.GetRequest());

            Exception ex = ExceptionHelper.CreateException();
            ctx.SetException(ex);
            Assert.AreEqual(ex, ctx.LastException);
            Assert.AreEqual(500, ctx.GetStatus());

            ctx.ClearErrors();
            Assert.IsNull(ctx.LastException);
            Assert.AreEqual(200, ctx.GetStatus());

            ctx.End();
            Assert.IsTrue((ctx.EndTime - ctx.StartTime).TotalSeconds < 2);

            long count1 = ClownFishCounters.Logging.WriteCount.Get();

            long count2 = ClownFishCounters.Logging.WriteCount.Get();

            Assert.IsTrue(count2 - count1 == 0);

            MyAssert.IsError<ArgumentNullException>(() => {
                ctx.SetOprLogScope(null);
            });

            OprLogScope scope = OprLogScope.Start(ctx);
            ctx.SetOprLogScope(scope);

            Assert.IsNotNull(ctx.OprLogScope);
            ctx.DisposeOprLogScope();
            Assert.IsNull(ctx.OprLogScope);

            Assert.IsFalse(ctx.IsLongTask);
            ctx.SetAsLongTask();
            Assert.IsTrue(ctx.IsLongTask);
            Assert.AreEqual(0, ctx.PerformanceThresholdMs);
        }
    }
}
