using ClownFish.Data.CodeDom;
using ClownFish.UnitTest.Data.Models;

namespace ClownFish.UnitTest.Data.CodeDom;

[TestClass]
public class EntityGeneratorTest
{
    [TestMethod]
    public void Test_生成实体代理类型代码()
    {
        EntityGenerator g = new EntityGenerator();
        string code = g.GetCode<Product>();

        code = EntityGenerator.UsingCodeBlock + code;

        string tempPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp");
        string outFile = Path.Combine(tempPath, "EntityGeneratorTest_code.cs");

        RetryFile.WriteAllText(outFile, code, Encoding.UTF8);
    }


    [TestMethod]
    public void Test_生成实体代理类程序集()
    {
        Type[] entityTypes = new Type[] { typeof(Product), typeof(Customer) };

        string tempPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp");
        string dllFilePath = Path.Combine(tempPath, "Test.EntityProxy.dll");

        RetryFile.Delete(dllFilePath);

        var result = ProxyBuilder.Compile(entityTypes, dllFilePath);

        Assert.IsTrue(RetryFile.Exists(dllFilePath));

        // 加载程序集并确认结果
        Assembly asm = Assembly.LoadFrom(dllFilePath);

        Type[] types = asm.GetExportedTypes();

        var t1 = (from x in types
                  where x.Name.StartsWith("Customer_") && x.Name.EndsWith("_Loader")
                  select x).First();

        var t2 = (from x in types
                  where x.Name.StartsWith("Customer_") && x.Name.EndsWith("_Proxy")
                  select x).First();

        var t3 = (from x in types
                  where x.Name.StartsWith("Product_") && x.Name.EndsWith("_Loader")
                  select x).First();

        var t4 = (from x in types
                  where x.Name.StartsWith("Product_") && x.Name.EndsWith("_Proxy")
                  select x).First();



    }
}
