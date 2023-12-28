using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.MQ.Pipeline;

#if NETCOREAPP
[TestClass]
public class PipelineUtilsTest
{
    [TestMethod]
    public void Test_SafeCall()
    {
        // 不抛异常就算测试通过
        PipelineUtils.SafeCall<string>(M1, null);
    }

    [TestMethod]
    public async Task Test_SafeCallAsync()
    {
        // 不抛异常就算测试通过
        await PipelineUtils.SafeCallAsync<string>(M2, null);
    }


    private static void M1(PipelineContext<string> ctx)
    {
        throw new NotImplementedException();
    }

    private static Task M2(PipelineContext<string> ctx)
    {
        throw new NotImplementedException();
    }

    [TestMethod]
    public void Test_ExceptionIsNeedRetry()
    {
        Assert.IsFalse(PipelineUtils.ExceptionIsNeedRetry(null));

        Assert.IsFalse(PipelineUtils.ExceptionIsNeedRetry(new System.ComponentModel.DataAnnotations.ValidationException()));
        Assert.IsFalse(PipelineUtils.ExceptionIsNeedRetry(new ValidationException2("xxxx")));
        Assert.IsFalse(PipelineUtils.ExceptionIsNeedRetry(new ClientDataException("xxxx")));
        Assert.IsFalse(PipelineUtils.ExceptionIsNeedRetry(new ArgumentException("xxxx")));
        Assert.IsFalse(PipelineUtils.ExceptionIsNeedRetry(new InvalidOperationException("xxxx")));
        Assert.IsFalse(PipelineUtils.ExceptionIsNeedRetry(new InvalidDataException("xxxx")));
        Assert.IsFalse(PipelineUtils.ExceptionIsNeedRetry(new InvalidCodeException("xxxx")));
        Assert.IsFalse(PipelineUtils.ExceptionIsNeedRetry(new MessageException("xxxx")));
        Assert.IsFalse(PipelineUtils.ExceptionIsNeedRetry(new TaskCanceledException("xxxx")));
        Assert.IsFalse(PipelineUtils.ExceptionIsNeedRetry(new DuplicateInsertException("xxxx", null)));
        Assert.IsFalse(PipelineUtils.ExceptionIsNeedRetry(new ForbiddenException("xxxx")));
        Assert.IsFalse(PipelineUtils.ExceptionIsNeedRetry(new DatabaseNotFoundException("xxxx")));
        Assert.IsFalse(PipelineUtils.ExceptionIsNeedRetry(new TenantNotFoundException("xxxx")));
        Assert.IsFalse(PipelineUtils.ExceptionIsNeedRetry(new ConfigurationErrorsException("xxxx")));
        Assert.IsFalse(PipelineUtils.ExceptionIsNeedRetry(new NotImplementedException("xxxx")));
        Assert.IsFalse(PipelineUtils.ExceptionIsNeedRetry(new NotSupportedException("xxxx")));


        Assert.IsTrue(PipelineUtils.ExceptionIsNeedRetry(new ApplicationException("xxxx")));
        Assert.IsFalse(PipelineUtils.ExceptionIsNeedRetry(new ApplicationException("xxxx timeout xxx")));


        HttpResult<string> httpResult1 = new HttpResult<string>(500, null, "xxxxxxx");
        RemoteWebException ex1 = new RemoteWebException(new MessageException("xxxx"), "http://xxxxxxxx");
        PropertyInfo p = ex1.GetType().GetProperty("Result", BindingFlags.Instance| BindingFlags.Public| BindingFlags.NonPublic);
        p.SetValue(ex1, httpResult1);
        Assert.IsTrue(PipelineUtils.ExceptionIsNeedRetry(ex1));


        HttpResult<string> httpResult2 = new HttpResult<string>(400, null, "xxxxxxx");
        RemoteWebException ex2 = new RemoteWebException(new MessageException("xxxx"), "http://xxxxxxxx");
        p.SetValue(ex2, httpResult2);
        Assert.IsFalse(PipelineUtils.ExceptionIsNeedRetry(ex2));
    }
}
#endif
