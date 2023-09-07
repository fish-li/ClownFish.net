using ClownFish.Data.CodeDom;

namespace ClownFish.UnitTest.Data.CodeDom;

[TestClass]
public class CodeCompilerHelperTest
{
    [TestMethod]
    public void Test()
    {
        MyAssert.IsError<ArgumentNullException>(()=> {
            CodeCompilerHelper.CompileCode(string.Empty, "test_xxx.dll");
        });


        MyAssert.IsError<ArgumentNullException>(() => {
            CodeCompilerHelper.CompileCode("xxx", string.Empty);
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            CodeCompilerHelper.CompileCode("xxx", "test_xxx.dll");
        });

        RetryFile.Delete("test_xxx.dll");
        RetryFile.Delete("test_xxx.dll.cs");
        RetryFile.Delete("test_xxx.dll.error");
    }
}
