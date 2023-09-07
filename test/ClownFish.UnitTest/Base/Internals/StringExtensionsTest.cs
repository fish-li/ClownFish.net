using ClownFish.Base.Internals;

namespace ClownFish.UnitTest.Base.Internals;

[TestClass]
public class StringExtensionsTest
{
    [TestMethod]
    public void Test_ToLower()
    {
        string s1 = "Abc";

        string s2 = s1.ToLower();
        string s3 = s1.ToLower();
        Assert.IsFalse(object.ReferenceEquals(s2, s3));

        string s4 = s1.NameToLower();
        string s5 = s1.NameToLower();
        Assert.IsTrue(object.ReferenceEquals(s4, s5));

        string s11 = "abc";
        string s12 = s11.ToLower();
#if NETFRAMEWORK
        Assert.IsFalse(object.ReferenceEquals(s12, s11));
#else
        Assert.IsTrue(object.ReferenceEquals(s12, s11));
#endif
        Assert.IsFalse(object.ReferenceEquals(s12, s2));

        string s13 = s11.NameToLower();
        Assert.IsFalse(object.ReferenceEquals(s13, s4));
        Assert.IsFalse(object.ReferenceEquals(s13, s5));

        Assert.AreEqual("", ClownFish.Base.Internals.StringExtensions.NameToLower(""));
    }

    [TestMethod]
    public void Test_ToUpper()
    {
        string s1 = "Abc";

        string s2 = s1.ToUpper();
        string s3 = s1.ToUpper();
        Assert.IsFalse(object.ReferenceEquals(s2, s3));

        string s4 = s1.NameToUpper();
        string s5 = s1.NameToUpper();
        Assert.IsTrue(object.ReferenceEquals(s4, s5));

        string s11 = "ABC";
        string s12 = s11.ToUpper();
#if NETFRAMEWORK
        Assert.IsFalse(object.ReferenceEquals(s12, s11));
#else
        Assert.IsTrue(object.ReferenceEquals(s12, s11));
#endif
        Assert.IsFalse(object.ReferenceEquals(s12, s2));

        string s13 = s11.NameToUpper();
        Assert.IsFalse(object.ReferenceEquals(s13, s4));
        Assert.IsFalse(object.ReferenceEquals(s13, s5));

        Assert.AreEqual("", ClownFish.Base.Internals.StringExtensions.NameToUpper(""));
    }
}
