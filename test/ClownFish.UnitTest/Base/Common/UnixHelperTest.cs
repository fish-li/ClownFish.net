using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Common;
[TestClass]
public class UnixHelperTest
{
    [TestMethod]
    public void Test_TrimTerminalCtrolChar1()
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files/log-base64-soh.txt");
        string base64 = File.ReadAllText(filePath);
        byte[] buffer = Convert.FromBase64String(base64);

        string lines = UnixHelper.TrimTerminalCtrolChar(buffer);
        Assert.IsTrue(lines.Contains("ApplicationName:            Nebula.Ceres"));

        string lines2 = UnixHelper.TrimTerminalCtrolChar2(buffer);
        Assert.IsTrue(lines2.Contains("ApplicationName:            Nebula.Ceres"));
    }


    [TestMethod]
    public void Test_TrimTerminalCtrolChar2()
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files/log-base64-stx.txt");
        string base64 = File.ReadAllText(filePath);
        byte[] buffer = Convert.FromBase64String(base64);

        string lines = UnixHelper.TrimTerminalCtrolChar(buffer);
        Assert.IsTrue(lines.Contains("INFO  actix_web::middleware::logger]"));

        string lines2 = UnixHelper.TrimTerminalCtrolChar2(buffer);
        Assert.IsTrue(lines2.Contains("INFO  actix_web::middleware::logger]"));
    }
}
