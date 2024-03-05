namespace ClownFish.UnitTest.Base.Reflection;

[TestClass]
public class TypeHelperTest
{
    static TypeHelperTest()
    {
        string text = @"
# 格式：alias = dest-type-name
Nebula.Common.Security.WebUserInfo111, Nebula.net = ClownFish.UnitTest.Base.Reflection.WebUserInfo2, ClownFish.UnitTest
";
        TypeHelper.InitFormText(text);

        TypeHelper.RegisterAlias("Nebula.Common.Security.AppClientInfo111, Nebula.net", typeof(AppClientInfo2));

        TypeHelper.Init();
    }

    [TestMethod]
    public void Test_CheckInit()
    {
        Type t1 = TypeHelper.GetType("Nebula.Common.Security.WebUserInfo111, Nebula.net", false);
        Assert.IsNotNull(t1);
        Assert.AreEqual(typeof(WebUserInfo2), t1);


        Type t2 = TypeHelper.GetType("Nebula.Common.Security.AppClientInfo111, Nebula.net", false);
        Assert.IsNotNull(t2);
        Assert.AreEqual(typeof(AppClientInfo2), t2);


        Type t3 = TypeHelper.GetType("Nebula.Common.Security.xxxxxxxxxxx, Nebula.net", false);
        Assert.IsNull(t3);
    }

    [TestMethod]
    public void Test_RegisterAlias()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            TypeHelper.RegisterAlias(null, typeof(TypeHelperTest));
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            TypeHelper.RegisterAlias("xxxxxxxxx", (Type)null);
        });
    }

    [TestMethod]
    public void Test_GetType()
    {
        MyAssert.IsError<ArgumentNullException>(()=> {
            _ = TypeHelper.GetType(null, false);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = TypeHelper.GetType(string.Empty, false);
        });

        MyAssert.IsError<TypeLoadException>(() => {
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



public class WebUserInfo2 { }
public class AppClientInfo2 { }
