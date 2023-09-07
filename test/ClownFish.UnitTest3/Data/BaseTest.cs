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

    public static readonly string[] ConnNames = new string[] { "sqlserver" };


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
