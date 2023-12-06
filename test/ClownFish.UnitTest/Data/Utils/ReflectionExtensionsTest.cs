using ClownFish.Data;

namespace ClownFish.UnitTest.Data.Utils;

[TestClass]
public class ReflectionExtensionsTest : BaseTest
{
    [TestMethod]
    public void Test_ReflectionExtensions_GetTypeString()
    {
        Type t = typeof(Dictionary<string, object>);
        Assert.AreEqual("Dictionary<string, System.Object>", t.GetTypeString0());
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


    [TestMethod]
    public void Test_ToTypeString()
    {
        Assert.AreEqual("string", typeof(string).ToTypeString());
        Assert.AreEqual("byte", typeof(byte).ToTypeString());
        Assert.AreEqual("byte?", typeof(byte?).ToTypeString());
        Assert.AreEqual("sbyte", typeof(sbyte).ToTypeString());
        Assert.AreEqual("sbyte?", typeof(sbyte?).ToTypeString());
        Assert.AreEqual("short", typeof(short).ToTypeString());
        Assert.AreEqual("short?", typeof(short?).ToTypeString());
        Assert.AreEqual("int", typeof(int).ToTypeString());
        Assert.AreEqual("int?", typeof(int?).ToTypeString());
        Assert.AreEqual("long", typeof(long).ToTypeString());
        Assert.AreEqual("long?", typeof(long?).ToTypeString());
        Assert.AreEqual("bool", typeof(bool).ToTypeString());
        Assert.AreEqual("bool?", typeof(bool?).ToTypeString());
        Assert.AreEqual("DateTime", typeof(DateTime).ToTypeString());
        Assert.AreEqual("DateTime?", typeof(DateTime?).ToTypeString());
        Assert.AreEqual("TimeSpan", typeof(TimeSpan).ToTypeString());
        Assert.AreEqual("TimeSpan?", typeof(TimeSpan?).ToTypeString());
        Assert.AreEqual("Guid", typeof(Guid).ToTypeString());
        Assert.AreEqual("Guid?", typeof(Guid?).ToTypeString());
        Assert.AreEqual("decimal", typeof(decimal).ToTypeString());
        Assert.AreEqual("decimal?", typeof(decimal?).ToTypeString());
        Assert.AreEqual("double", typeof(double).ToTypeString());
        Assert.AreEqual("double?", typeof(double?).ToTypeString());
        Assert.AreEqual("float", typeof(float).ToTypeString());
        Assert.AreEqual("float?", typeof(float?).ToTypeString());


        Assert.AreEqual("List<int>", typeof(List<int>).ToTypeString());
        Assert.AreEqual("List<int[]>", typeof(List<int[]>).ToTypeString());
        Assert.AreEqual("List<List<int>>", typeof(List<List<int>>).ToTypeString());

        Assert.AreEqual("int[]", typeof(int[]).ToTypeString());
        Assert.AreEqual("int[][]", typeof(int[][]).ToTypeString());
        Assert.AreEqual("List<int>[]", typeof(List<int>[]).ToTypeString());

        Assert.AreEqual("Dictionary<string, int>", typeof(Dictionary<string, int>).ToTypeString());
        Assert.AreEqual("Dictionary<string, int[]>", typeof(Dictionary<string, int[]>).ToTypeString());
        Assert.AreEqual("Dictionary<string, List<int>>", typeof(Dictionary<string, List<int>>).ToTypeString());
        Assert.AreEqual("Dictionary<string, Dictionary<string, int>>", typeof(Dictionary<string, Dictionary<string, int>>).ToTypeString());

        Assert.AreEqual("List<string>", typeof(List<string>).ToTypeString());
        Assert.AreEqual("string[]", typeof(string[]).ToTypeString());

        Assert.AreEqual("System.DayOfWeek", typeof(System.DayOfWeek).ToTypeString());
        Assert.AreEqual("System.Drawing.Point", typeof(System.Drawing.Point).ToTypeString());
        Assert.AreEqual("ClownFish.UnitTest.Data.Models.EncSaveString", typeof(ClownFish.UnitTest.Data.Models.EncSaveString).ToTypeString());

        Assert.AreEqual("ClownFish.UnitTest.Data.Utils.Test27a2ab0b72f5479ead7189178682af14.A.B", typeof(Test27a2ab0b72f5479ead7189178682af14.A.B).ToTypeString());
        Assert.AreEqual("ClownFish.UnitTest.Data.Utils.Test27a2ab0b72f5479ead7189178682af14.A.B[]", typeof(Test27a2ab0b72f5479ead7189178682af14.A.B[]).ToTypeString());
        Assert.AreEqual("List<ClownFish.UnitTest.Data.Utils.Test27a2ab0b72f5479ead7189178682af14.A.B>", typeof(List<Test27a2ab0b72f5479ead7189178682af14.A.B>).ToTypeString());
    }

}


public class Test27a2ab0b72f5479ead7189178682af14
{
    public class A
    {
        public class B
        {

        }
    }
}
