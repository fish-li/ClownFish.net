namespace ClownFish.UnitTest.Base.Reflection;

[TestClass]
public class TypeHelperTest
{
    [TestMethod]
    public void Test_GetType()
    {
        MyAssert.IsError<ArgumentNullException>(()=> {
            _ = TypeHelper.GetType(null, false);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = TypeHelper.GetType(string.Empty, false);
        });

        MyAssert.IsError<ArgumentOutOfRangeException>(() => {
            _ = TypeHelper.GetType("xx", true);
        });

        Assert.IsNull(TypeHelper.GetType("xx", false));

        Type t = TypeHelper.GetType("ClownFish.UnitTest.Base.Reflection.TypeHelperTest, ClownFish.UnitTest", true);
        Assert.IsNotNull(t);
        Assert.AreEqual("ClownFish.UnitTest.Base.Reflection.TypeHelperTest", t.FullName);
    }

    [TestMethod]
    public void Test_GetShortName()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = TypeHelper.GetShortName(null);
        });

        Assert.AreEqual("XXX", TypeHelper.GetShortName("ClownFish.UnitTest.XXX, ClownFish.UnitTest"));
        Assert.AreEqual("XXX", TypeHelper.GetShortName("ClownFish.UnitTest.XXX,ClownFish.UnitTest"));
        Assert.AreEqual("XXX", TypeHelper.GetShortName("ClownFish.UnitTest.XXX"));
        Assert.AreEqual("XXX", TypeHelper.GetShortName("XXX"));
        Assert.AreEqual("XXX", TypeHelper.GetShortName("XXX, ClownFish.UnitTest"));

        Assert.AreEqual("XXX", TypeHelper.GetShortName("ClownFish.UnitTest.XXX, ClownFish.UnitTest, rrrrrr"));
    }
}
