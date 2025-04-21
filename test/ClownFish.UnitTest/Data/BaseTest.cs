using ClownFish.Data.Xml;
using ClownFish.UnitTest.Data.Events;

namespace ClownFish.UnitTest.Data;

[TestClass]
public abstract class BaseTest
{
    /// <summary>
    /// 将CPQuery的内部参数序号计数器重置为零，便于做SQL语句的断言
    /// </summary>
    [TestInitialize]
    public void ResetCPQueryParamIndex()
    {
        typeof(CPQuery).InvokeMember("s_index",
                            BindingFlags.SetField | BindingFlags.NonPublic | BindingFlags.Static,
                            null, null, new object[] { 0 });
    }


#if NET9_0
    // 【国产数据库】不是想运行就能运行的，它们有license限制，没办法一直对它测试！
    public static readonly string[] ConnNames = new string[] { "sqlserver", "mysql", "postgresql",
#if TEST_KINGBASE2
        "kingbase2",    // 人大金仓，使用 Kdbndp 驱动
#endif
#if TEST_KINGBASE
        "kingbase",     // 人大金仓，使用 Npgsql 驱动
#endif
#if TEST_DM
        "dm",           // 达梦
#endif
#if TEST_VASTBASE
        "vastbase"      // 海量
#endif
    };
#elif NET8_0
    public static readonly string[] ConnNames = new string[] { "sqlserver", "mysql" };
#else
    public static readonly string[] ConnNames = new string[] { "sqlserver" };
#endif



    public void AssertDbCommand(DbCommand command, string text)
    {
        string text2 = ClownFishDataEventSubscriber.CommandToAllText(command);
        MyAssert.SqlAreEqual(text, text2);
    }

    public void AssertLastExecuteSQL(string text)
    {
        MyAssert.SqlAreEqual(text, ClownFishDataEventSubscriber.LastExecuteSQL);
    }


    public void AssertLastQuery(string text)
    {
        MyAssert.SqlAreEqual(text, ClownFishDataEventSubscriber.LastQuery);
    }


    public string GetSql(string xmlcommandName)
    {
        // 这个测试类为了简单，就直接借用XmlCommand中定义的SQL语句

        XmlCommandItem x1 = XmlCommandManager.Instance.GetCommand(xmlcommandName);
        return x1.CommandText;
    }

    public void ShowCurrentThread()
    {
        //System.Console.WriteLine("ThreadId: " + System.Threading.Thread.CurrentThread.ManagedThreadId);
    }


}
