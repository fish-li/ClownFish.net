using System;
using System.Collections.Generic;
using System.Text;

namespace ClownFish.UnitTest.Base.Common;

[TestClass]
public class RegexUtilsTest
{
    [TestMethod]
    public void Test_CreateRouteRegex()
    {
        string pattern = "/page/{id}/{year}-{month}-{day}.aspx";
        Regex r = RegexUtils.CreateRouteRegex(pattern);
        string url = "/page/123/2024-06-30.aspx";
        Match m = r.Match(url);
        Assert.IsTrue(m.Success);
        Assert.AreEqual("123", m.Groups["id"].Value);
        Assert.AreEqual("2024", m.Groups["year"].Value);
        Assert.AreEqual("06", m.Groups["month"].Value);
        Assert.AreEqual("30", m.Groups["day"].Value);
    }

    [TestMethod]
    public void Test_HasRouteName()
    {
        Assert.IsTrue(RegexUtils.HasRouteName("/page/{id}/{year}-{month}-{day}.aspx"));
        Assert.IsFalse(RegexUtils.HasRouteName("/page/123/2024-06-30.aspx"));
    }



}
