namespace ClownFish.UnitTest.Base.Reflection;

[TestClass]
public class CtorTest
{
    [TestMethod]
    public void Test_FastNew()
    {
        DataObject data = (DataObject)typeof(DataObject).FastNew();
        Assert.IsNotNull(data);
    }


    [ExpectedException(typeof(NotSupportedException))]
    [TestMethod]
    public void Test_FastNew_NotSupportedException()
    {
        _ = (InstanceObject)typeof(InstanceObject).FastNew();
    }

    [TestMethod]
    public void Test_DynamicMethodFactory_Error()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = DynamicMethodFactory.CreateConstructor(null);
        });

        MyAssert.IsError<NotSupportedException>(() => {
            ConstructorInfo ctor = typeof(XxxObject).GetConstructor(new Type[] { typeof(int) });
            _ = DynamicMethodFactory.CreateConstructor(ctor);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            Type instanceType = null;
            _ = instanceType.FastNew();
        });

    }

}
