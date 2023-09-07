using ClownFish.Data.CodeDom;

namespace ClownFish.UnitTest.Data.CodeDom;

[TestClass]
public class CodeGenUtilsTest
{
    [TestMethod]
    public void Test1()
    {
        string binPath = "App_Data";

        int result = CodeGenUtils.CompileEntityProxyAsm(binPath, "xxx.dll", true);
        Assert.AreEqual(0, result);
    }


    [TestMethod]
    public void Test2()
    {
        string binPath = "App_Data";

        List<Type> types = CodeGenUtils.SearchEntityTypes(binPath);
        Assert.AreEqual(0, types.Count);
    }


    [TestMethod]
    public void Test3()
    {
        string binPath = ".";
        string dllSaveFilePath = "./temp/_xxx.EntityProxy.dll";

        File.Delete(dllSaveFilePath);

        int result = CodeGenUtils.CompileEntityProxyAsm(binPath, dllSaveFilePath, true);

        Assert.IsTrue(result > 0);
        Assert.IsTrue(File.Exists(dllSaveFilePath));
    }


    [TestMethod]
    public void Test4()
    {
        string binPath = ".";

        List<Type> types = CodeGenUtils.SearchEntityTypes(binPath);
        Assert.IsTrue(types.Count > 0);

        foreach( var t in types )
            Console.WriteLine(t.FullName);
    }
}
