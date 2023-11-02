using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Log.Logging;
[TestClass]
public class CodeSnippetContextTest
{
    [TestMethod]
    public void Test1()
    {
        long count1 = ClownFishCounters.Logging.WriteCount.Get();

        using( CodeSnippetContext ctx = new CodeSnippetContext(typeof(CodeSnippetContextTest), "Test1") ) {
            ctx.OprLogScope.EnableFxEvent();
            ctx.OprLogScope.AddFxEvent(new NameTime("name1", DateTime.Now));

            Thread.Sleep(100);

            // Dispose 时会生成2条日志：OprLog, InvokeLog
        }

        long count2 = ClownFishCounters.Logging.WriteCount.Get();
        Assert.AreEqual(count1+2, count2);
    }

    [TestMethod]
    public void Test2()
    {
        OprLogScope s1 = OprLogScope.Get();
        Assert.IsNotNull(s1);
        Assert.IsTrue(s1.IsNull);


    }
}
