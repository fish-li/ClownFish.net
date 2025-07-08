namespace ClownFish.UnitTest.Base.Reflection;
[TestClass]
public class MethodInfoExtensionsTest
{
    [TestMethod]
    public void Test_GetMethodFullName()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = ClownFish.Base.Reflection.MethodInfoExtensions.GetMethodFullName((MethodInfo)null);
        });

        MethodInfo method = typeof(MethodInfoExtensionsTest).GetMethod("Add"); ;
        Assert.AreEqual("ClownFish.UnitTest.Base.Reflection.MethodInfoExtensionsTest/Add", method.GetMethodFullName());
    }

    public static int Add(int a, int b)
    {
        if( a > 1000 )
            throw new ArgumentOutOfRangeException();

        return a + b;
    }

    [TestMethod]
    public void Test_InvokeAndLog()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = ClownFish.Base.Reflection.MethodInfoExtensions.InvokeAndLog((MethodInfo)null, null);
        });

        MethodInfo method = typeof(MethodInfoExtensionsTest).GetMethod("Add");
        int a1 = (int)method.InvokeAndLog(null, new object[] { 2, 3 });
        Assert.AreEqual(5, a1);

        using( CodeSnippetContext ctx = new CodeSnippetContext(typeof(MethodInfoExtensionsTest), nameof(Test_InvokeAndLog), 1) ) {
            int a2 = (int)method.InvokeAndLog(null, new object[] { 3, 4 });
            Assert.AreEqual(7, a2);

            Assert.AreEqual(1, ctx.OprLogScope.GetStepItems().Count);
            StepItem step = ctx.OprLogScope.GetStepItems().First();
            Assert.AreEqual(200, step.Status);
        }

        using( CodeSnippetContext ctx2 = new CodeSnippetContext(typeof(MethodInfoExtensionsTest), nameof(Test_InvokeAndLog), 1) ) {
            MyAssert.IsError<ArgumentOutOfRangeException>(() => {
                int a3 = (int)method.InvokeAndLog(null, new object[] { 300000, 4 });
            });

            Assert.AreEqual(1, ctx2.OprLogScope.GetStepItems().Count);
            StepItem step = ctx2.OprLogScope.GetStepItems().First();
            Assert.AreEqual(500, step.Status);
            Assert.AreEqual(typeof(ArgumentOutOfRangeException).FullName, step.ExType);
        }

    }

}
