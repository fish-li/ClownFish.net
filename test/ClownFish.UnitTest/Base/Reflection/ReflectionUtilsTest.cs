using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Base.Reflection;
[TestClass]
public class ReflectionUtilsTest
{
    [TestMethod]
    public void Test_Get()
    {
        NameValue v1 = new NameValue("key1", "abc");
        Assert.AreEqual("key1", v1.Get<string>("Name"));
        Assert.AreEqual("abc", v1.Get<string>("Value"));

        MyAssert.IsError<ArgumentNullException>(() => {
            ReflectionUtils.Get<string>(null, "Name");
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            ReflectionUtils.Get<string>(v1, "");
        });

        MyAssert.IsError<ArgumentOutOfRangeException>(() => {
            ReflectionUtils.Get<string>(v1, "xxx");
        });
    }


    [TestMethod]
    public void Test_CallStaticMethod()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            ReflectionUtils.CallStaticMethod(null, "Name");
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            ReflectionUtils.CallStaticMethod("xxxxxx", "");
        });

        Assert.AreEqual(-1, ReflectionUtils.CallStaticMethod("xxxxxxxxxxxxxxxxxxxxxx", "Name"));
        Assert.AreEqual(-2, ReflectionUtils.CallStaticMethod("ClownFish.UnitTest.Base.Reflection.ReflectionUtilsTest, ClownFish.UnitTest", "Name"));

        Assert.AreEqual(1, ReflectionUtils.CallStaticMethod("ClownFish.UnitTest.Base.Reflection.ReflectionUtilsTest, ClownFish.UnitTest", "M1"));

    }


    private static void M1()
    {
        // 什么也不做
    }
}
