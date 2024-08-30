using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.MQ;
[TestClass]
public class Class1
{
    private static string FixWinName(string windowsName)
    {
        windowsName = windowsName.Replace("®", "");                       // Microsoft® Windows Server® 2008 Enterprise
        //windowsName = windowsName.Replace("(R)", "").Replace(",", "");    // Microsoft(R) Windows(R) Server 2003, Enterprise Edition

        // 由于历史原因 2003 的名称太乱了，已遇到3个
        // Microsoft(R) Windows(R) Server 2003 Enterprise x64 Edition
        // Microsoft(R) Windows(R) Server 2003, Enterprise Edition
        // Microsoft(R) Windows(R) Server 2003, Standard Edition
        if( windowsName.IndexOfIgnoreCase("Server 2003") >= 0 )
            return "Windows Server 2003";

        if( windowsName.IndexOfIgnoreCase("Windows NT 6.1") >= 0 )       // Windows NT 6.1.7601 Service Pack 1
            windowsName = "Windows 7";

        if( windowsName.IndexOfIgnoreCase("Windows NT 6.2") >= 0 )
            windowsName = "Windows 8";

        if( windowsName.IndexOfIgnoreCase("Windows NT 6.3") >= 0 )
            windowsName = "Windows 8.1";

        if( windowsName.IndexOfIgnoreCase("Windows NT 10.0") >= 0 )
            windowsName = "Windows 10";

        if( windowsName.StartsWithIgnoreCase("Microsoft ") )
            windowsName = windowsName.Substring(10); // "Microsoft Windows 7" =>  "Windows 7"

        if( windowsName.EndsWithIgnoreCase(" Evaluation") )    // Windows Server 2016 Datacenter Evaluation
            windowsName = windowsName.Substring(0, windowsName.Length - 11);

        if( windowsName.EndsWithIgnoreCase(" Edition") )
            windowsName = windowsName.Substring(0, windowsName.Length - 8);

        if( windowsName.EndsWithIgnoreCase(" 评估版") )    // Windows Server 2012 R2 Standard 评估版
            windowsName = windowsName.Substring(0, windowsName.Length - 4);

        return windowsName;
    }

    [TestMethod]
    public void Test1()
    {
        Assert.AreEqual("Windows Server 2003", FixWinName("Microsoft(R) Windows(R) Server 2003 Enterprise x64 Edition"));
        Assert.AreEqual("Windows Server 2003", FixWinName("Microsoft(R) Windows(R) Server 2003, Enterprise Edition"));
        Assert.AreEqual("Windows Server 2003", FixWinName("Microsoft(R) Windows(R) Server 2003, Standard Edition"));

        Assert.AreEqual("Windows Server 2008 Enterprise", FixWinName("Microsoft® Windows Server® 2008 Enterprise"));
        Assert.AreEqual("Windows Server 2008 HPC", FixWinName("Microsoft® Windows Server® 2008 HPC Edition"));
        Assert.AreEqual("Windows Server 2008 R2 Enterprise", FixWinName("Microsoft® Windows Server® 2008 R2 Enterprise"));
        Assert.AreEqual("Windows Server 2008 R2 Datacenter", FixWinName("Microsoft® Windows Server® 2008 R2 Datacenter"));
        Assert.AreEqual("Windows Web Server 2008 R2", FixWinName("Microsoft Windows Web Server 2008 R2"));

        Assert.AreEqual("Windows Server 2012 Standard", FixWinName("Microsoft Windows Server 2012 Standard"));
        Assert.AreEqual("Windows Server 2012 R2 Enterprise", FixWinName("Microsoft Windows Server 2012 R2 Enterprise"));
        Assert.AreEqual("Windows Server 2012 R2 Datacenter", FixWinName("Microsoft Windows Server 2012 R2 Datacenter"));

        Assert.AreEqual("Windows Server 2016 Datacenter", FixWinName("Microsoft Windows Server 2016 Datacenter"));
        Assert.AreEqual("Windows Server 2019 Datacenter", FixWinName("Microsoft Windows Server 2019 Datacenter"));

        Assert.AreEqual("Windows 7", FixWinName("Microsoft Windows NT 6.1.7601 Service Pack 1"));
        Assert.AreEqual("Windows 8", FixWinName("Microsoft Windows NT 6.2.7601 Service Pack 1"));
        Assert.AreEqual("Windows 8.1", FixWinName("Microsoft Windows NT 6.3.7601 Service Pack 1"));
        Assert.AreEqual("Windows 10", FixWinName("Microsoft Windows NT 10.0.14393.0"));

        Assert.AreEqual("Windows Server 2016 Datacenter", FixWinName("Microsoft Windows Server 2016 Datacenter Evaluation"));
        Assert.AreEqual("Windows Server 2012 R2 Standard", FixWinName("Microsoft Windows Server 2012 R2 Standard 评估版"));

        Assert.AreEqual("Windows 10 教育版", FixWinName("Microsoft Windows 10 教育版"));
        Assert.AreEqual("Windows 8.1 企业版", FixWinName("Microsoft Windows 8.1 企业版"));

    }
}
