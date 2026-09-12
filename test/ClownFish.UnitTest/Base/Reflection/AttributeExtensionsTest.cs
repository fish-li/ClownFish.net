using System;
using System.Collections.Generic;
using System.Text;

namespace ClownFish.UnitTest.Base.Reflection;

[TestClass]
public class AttributeExtensionsTest
{
    [TestMethod]
    public void Test_GetMyAttribute()
    {
        MethodInfo m1 = typeof(TestAttrClass).GetMethod("TestMethod1");
        My1Attribute a1 = m1.GetMyAttribute<My1Attribute>();
        Assert.AreEqual(3, a1.Add());

        MethodInfo m2 = typeof(TestAttrClass).GetMethod("TestMethod2");
        My1Attribute a2 = m2.GetMyAttribute<My1Attribute>();
        Assert.AreEqual(17, a2.Add());

        My1Attribute a3 = typeof(TestAttrClass).GetMyAttribute<My1Attribute>();
        Assert.AreEqual(21, a3.Add());
    }
}



[My2(5, 6)]
public class TestAttrClass
{
    [My1(1, 2)]
    public void TestMethod1()
    {
    }
    [My2(3, 4)]
    public void TestMethod2()
    {
    }
}

[AttributeUsage(AttributeTargets.All)]
public class My1Attribute : Attribute
{
    public int A { get; set; }
    public int B { get; set; }
    public My1Attribute(int a, int b)
    {
        A = a;
        B = b;
    }

    public virtual int Add()
    {
        return A + B; 
    }
}


[AttributeUsage(AttributeTargets.All)]
public class My2Attribute : My1Attribute
{
    public My2Attribute(int a, int b) : base(a, b)
    {
    }
    public override int Add()
    {
        return A + B + 10;
    }
}