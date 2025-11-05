using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Base.Common;


[TestClass]
public class VersionParserTest
{
    [TestMethod]
    public void Test()
    {
        Assert.AreEqual("1.22", VersionParser.Parse("1.22").ToString());
        Assert.AreEqual("1.0", VersionParser.Parse("1.x").ToString());
        Assert.AreEqual("9.0.10", VersionParser.Parse("9.0.10").ToString());
        Assert.AreEqual("9.25.1015.1", VersionParser.Parse("9.25.1015.1").ToString());
        Assert.AreEqual("5.1.14393.4530", VersionParser.Parse("5.1.14393.4530").ToString());
        Assert.AreEqual("1.2", VersionParser.Parse("1.2.-1.-1").ToString());
        Assert.AreEqual("3.0", VersionParser.Parse("3.0.-1.-1").ToString());   // powershell
        Assert.AreEqual("0.3", VersionParser.Parse("0.3").ToString());
        Assert.AreEqual("0.3", VersionParser.Parse("0.3.2a").ToString());
        Assert.AreEqual("2.0", VersionParser.Parse("2").ToString());
        Assert.AreEqual("2.0", VersionParser.Parse("2.a").ToString());
        Assert.AreEqual("0.0", VersionParser.Parse("a.2").ToString());
        Assert.AreEqual("0.0", VersionParser.Parse("0.a").ToString());
        Assert.AreEqual("0.0", VersionParser.Parse("x.a").ToString());        
        Assert.AreEqual("0.0", VersionParser.Parse("xx").ToString());
        Assert.AreEqual("0.0", VersionParser.Parse("").ToString());

        Assert.AreEqual("10.0", VersionParser.Parse("10.0.0-rc.2.25502.107").ToString());

    }
}

