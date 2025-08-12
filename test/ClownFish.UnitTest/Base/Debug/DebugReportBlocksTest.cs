using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Base.Debug;


[TestClass]
public class DebugReportBlocksTest
{
    [TestMethod]
    public void Test_GetLoggingCounters()
    {
        string text = DebugReportBlocks.GetLoggingCounters().ToString2();
        Console.WriteLine(text);
    }


    [TestMethod]
    public void Test_GetStaticVariablesReportBlock()
    {
        string text = DebugReport.GetStaticVariables().First().ToString2();
        Console.WriteLine(text);
    }

    [TestMethod]
    public void Test_GetCacheStatus()
    {
        string text = DebugReportBlocks.GetCacheStatus().ToString2();
        Console.WriteLine(text);
    }


    [TestMethod]
    public void Test_GetSystemInfo()
    {
        string text = DebugReportBlocks.GetSystemInfo().ToString2();
        Console.WriteLine(text);
    }

#if NETCOREAPP
    [TestMethod]
    public void Test_GetThreadPoolInfo()
    {
        string text = DebugReportBlocks.GetThreadPoolInfo().ToString2();
        Console.WriteLine(text);
    }

    [TestMethod]
    public void Test_GetGCInfo()
    {
        string text = DebugReportBlocks.GetGCInfo().ToString2();
        Console.WriteLine(text);
    }

#endif


    [TestMethod]
    public void Test_GetEnvironmentVariables()
    {
        string text = DebugReportBlocks.GetEnvironmentVariables().ToString2();
        Console.WriteLine(text);
    }


    [TestMethod]
    public void Test_GetEntityProxyLoaderList()
    {
        string text = DebugReportBlocks.GetEntityProxyLoaderList().ToString2();
        Console.WriteLine(text);
    }

    [TestMethod]
    public void Test_GetAssemblyListInfo()
    {
        string text = DebugReportBlocks.GetAssemblyListInfo().ToString2();
        Console.WriteLine(text);
    }


    [TestMethod]
    public void Test_GetStatusInfo()
    {
        string text = DebugReport.GetStatusInfo().ToText();
        Console.WriteLine(text);
    }

    [TestMethod]
    public void Test_GetSysInfo()
    {
        string text = DebugReport.GetSysInfo().ToText();
        Console.WriteLine(text);
    }

    [TestMethod]
    public void Test_GetAsmInfo()
    {
        string text = DebugReport.GetAsmInfo().ToText();
        Console.WriteLine(text);
    }

    [TestMethod]
    public void Test_GetAllData()
    {
        string text = DebugReport.GetAllData().ToText();
        Console.WriteLine(text);

        string outfile = Path.Combine(AppContext.BaseDirectory, "temp/DebugReport.txt");
        File.WriteAllText(outfile, text, Encoding.UTF8);
    }
}
