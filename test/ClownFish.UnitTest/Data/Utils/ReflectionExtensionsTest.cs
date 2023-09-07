namespace ClownFish.UnitTest.Data.Utils;

[TestClass]
public class ReflectionExtensionsTest : BaseTest
{
    [TestMethod]
    public void Test_ReflectionExtensions_GetTypeString()
    {
        Type t = typeof(Dictionary<string, object>);
        string text = (string)typeof(ReflectionExtensions).InvokeMember("GetTypeString",
                        BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic,
                        null, null, new object[] { t });

        Console.WriteLine(text);
        Assert.AreEqual("System.Collections.Generic.Dictionary<string,System.Object>", text);
    }


    [TestMethod]
    public void Test_ReflectionExtensions_IsAnonymousType()
    {
        object obj = new { a = 1, b = "abc" };
        Assert.IsTrue(ReflectionExtensions.IsAnonymousType(obj.GetType()));

        Assert.IsFalse(ReflectionExtensions.IsAnonymousType(typeof(int)));
    }


    [TestMethod]
    public void Test_ReflectionExtensions_IsIndexerProperty()
    {
        PropertyInfo p = typeof(Dictionary<string, object>).GetProperty("Item");
        Assert.IsTrue(ReflectionExtensions.IsIndexerProperty(p));

        PropertyInfo p2 = typeof(Dictionary<string, object>).GetProperty("Count");
        Assert.IsFalse(ReflectionExtensions.IsIndexerProperty(p2));
    }

    [TestMethod]
    public void Test_ReflectionExtensions_IsVirtualProperty()
    {
        PropertyInfo p = typeof(DbDataReader).GetProperty("VisibleFieldCount");
        Assert.IsTrue(ReflectionExtensions.IsVirtual(p));

        PropertyInfo p2 = typeof(DbCommand).GetProperty("Connection");
        Assert.IsFalse(ReflectionExtensions.IsVirtual(p2));

        PropertyInfo p3 = typeof(DbCommand).GetProperty("CommandText");
        Assert.IsTrue(ReflectionExtensions.IsVirtual(p3));
    }


}
