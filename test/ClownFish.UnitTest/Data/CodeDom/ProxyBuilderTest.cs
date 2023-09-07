using ClownFish.Data.CodeDom;
using ClownFish.UnitTest.Data.Models;

namespace ClownFish.UnitTest.Data.CodeDom;

[TestClass]
public class ProxyBuilderTest
{
    [TestMethod]
    public void Test_CompileAllEntityProxy_Error()
    {
        MyAssert.IsError<ArgumentNullException>(()=> {
            ProxyBuilder.CompileAllEntityProxy(null, true);
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            ProxyBuilder.CompileAllEntityProxy("xx", true);
        });
    }

    [TestMethod]
    public void Test_SearchAllEntityTypes()
    {
        List<EntityCompileResult> existCompileResult = ProxyLoader.SearchExistEntityCompileResult();

        List<Type> list1 = ProxyBuilder.SearchAllEntityTypes(existCompileResult, true);
        List<Type> list2 = ProxyBuilder.SearchAllEntityTypes(existCompileResult, false);

        Assert.AreEqual(list1.Count, list2.Count);
        MyAssert.AreEqual(list1.Select(x => x.FullName).ToList(), list2.Select(x => x.FullName).ToList());


        string block = ProxyBuilder.CompileEntityListReportBlock.ToString2();
        Console.WriteLine(block);

        Assert.IsTrue(block.Contains("##### Compile Entity List #####"));
        Assert.IsTrue(block.Contains(nameof(Customer)));
        Assert.IsTrue(block.Contains(nameof(Product)));

        Assert.IsTrue(block.Contains(nameof(OrderDetailX2)));
        Assert.IsTrue(block.Contains(nameof(OrderDetailX3)));
        Assert.IsTrue(block.Contains(nameof(OrderDetailX4)));

        Assert.IsFalse(block.Contains(nameof(OrderDetailX1)));
        Assert.IsFalse(block.Contains(nameof(OrderDetailX5)));
        Assert.IsFalse(block.Contains(nameof(OrderDetailX6)));
    }


    [TestMethod]
    public void Test_GetAssemblyEntityTypes()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            ProxyBuilder.GetAssemblyEntityTypes(null);
        });

        Type[] types = ProxyBuilder.GetAssemblyEntityTypes(typeof(ProxyBuilderTest).Assembly);

        Assert.IsTrue(types.Contains(typeof(Customer)));
        Assert.IsTrue(types.Contains(typeof(Product)));
        
        Assert.IsTrue(types.Contains(typeof(OrderDetailX2)));
        Assert.IsTrue(types.Contains(typeof(OrderDetailX3)));
        Assert.IsTrue(types.Contains(typeof(OrderDetailX4)));

        Assert.IsFalse(types.Contains(typeof(OrderDetailX1)));
        Assert.IsFalse(types.Contains(typeof(OrderDetailX5)));
        Assert.IsFalse(types.Contains(typeof(OrderDetailX6)));

    }


    [TestMethod]
    public void Test_Compile_Error()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            ProxyBuilder.Compile(null, "xx.dll");
        });

        var list = ProxyBuilder.Compile(Empty.Array<Type>(), "xx.dll");
        Assert.AreEqual(0, list.Count);
    }


}
