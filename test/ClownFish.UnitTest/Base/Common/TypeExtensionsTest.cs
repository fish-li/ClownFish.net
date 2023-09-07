namespace ClownFish.UnitTest.Base.Common;

[TestClass]
public class TypeExtensionsTest
{
    [TestMethod]
    public void Test_IsSimpleValueType()
    {
        Assert.IsTrue(typeof(DateTime).IsSimpleValueType());
        Assert.IsTrue(typeof(Guid).IsSimpleValueType());
        Assert.IsTrue(typeof(decimal).IsSimpleValueType());
        Assert.IsTrue(typeof(DayOfWeek).IsSimpleValueType());

    }

    [TestMethod]
    public void Test_GetRealType()
    {
        Assert.AreEqual(typeof(int), typeof(int).GetRealType());
        Assert.AreEqual(typeof(int), typeof(int?).GetRealType());

        Assert.AreNotEqual(typeof(int), typeof(long?).GetRealType());
    }

    [TestMethod]
    public void Test_IsNullableType()
    {
        Assert.IsFalse(typeof(DateTime).IsNullableType());
        Assert.IsTrue(typeof(DateTime?).IsNullableType());

        Assert.IsFalse(typeof(Guid).IsNullableType());
        Assert.IsTrue(typeof(Guid?).IsNullableType());
    }


    [TestMethod]
    public void Test_IsCompatible()
    {
        Assert.IsTrue(typeof(MemoryStream).IsCompatible(typeof(Stream)));
        Assert.IsTrue(typeof(MemoryStream).IsCompatible(typeof(IDisposable)));

        Assert.IsFalse(typeof(FileStream).IsCompatible(typeof(MemoryStream)));
    }




}


