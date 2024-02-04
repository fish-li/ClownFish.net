namespace ClownFish.UnitTest.Base;

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
        Assert.IsNotNull(ctx.OprLogScope);
        Assert.IsTrue(ctx.OprLogScope.IsNull);

        ctx.PerformanceThresholdMs = 50;

        Assert.AreEqual(200, ctx.GetStatus());
        Assert.AreEqual(24, ctx.ProcessId.Length);
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
        Assert.IsFalse(ctx.OprLogScope.IsNull);

        ctx.DisposeOprLogScope();
        Assert.IsNotNull(ctx.OprLogScope);
        Assert.IsTrue(ctx.OprLogScope.IsNull);

        Assert.IsFalse(ctx.IsLongTask);
        ctx.SetAsLongTask();
        Assert.IsTrue(ctx.IsLongTask);
        Assert.AreEqual(0, ctx.PerformanceThresholdMs);
    }
}
