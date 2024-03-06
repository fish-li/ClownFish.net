using System.Net.Http;
using System.Net.Sockets;

namespace ClownFish.UnitTest.Base.Init;

#if NETCOREAPP

[TestClass]
public class AppPartUtilsTest
{
    static AppPartUtilsTest()
    {
        // 确保静态变量已赋值
        _ = AppPartUtils.GetApplicationPartAsmList();
    }

    [TestMethod]
    public void Test_1()
    {
        List<Assembly> list = new List<Assembly>();
        list.Add(typeof(string).Assembly);
        list.Add(typeof(DbConnection).Assembly);
        list.Add(typeof(SqlClientFactory).Assembly);
        list.Add(typeof(HttpWebRequest).Assembly);
        list.Add(typeof(HttpRequestMessage).Assembly);
        list.Add(typeof(TcpListener).Assembly);

        List<Assembly> list2 = AppPartUtils.GetApplicationPartAsmList0(list);
        string names2 = string.Join(";", list2.Select(x => x.GetName().Name).ToArray());
        //Console.WriteLine(names2);
        Assert.AreEqual("System.Private.CoreLib;System.Data.Common;System.Data.SqlClient;System.Net.Requests;System.Net.Http;System.Net.Sockets;ClownFish.UnitTest", names2);



        MemoryConfig.AddSetting("ClownFish_EnableAppParts", "System.Private.CoreLib;System.Data.Common;System.Data.SqlClient");
        List<Assembly> list3 = AppPartUtils.GetApplicationPartAsmList0(list);
        string names3 = string.Join(";", list3.Select(x => x.GetName().Name).ToArray());
        Assert.AreEqual("System.Private.CoreLib;System.Data.Common;System.Data.SqlClient;ClownFish.UnitTest", names3);



        MemoryConfig.RemoveSetting("ClownFish_EnableAppParts");
        MemoryConfig.AddSetting("ClownFish_IgnoreAppParts", "System.Private.CoreLib;System.Data.Common;System.Data.SqlClient");
        List<Assembly> list4 = AppPartUtils.GetApplicationPartAsmList0(list);
        string names4 = string.Join(";", list4.Select(x => x.GetName().Name).ToArray());
        Assert.AreEqual("System.Net.Requests;System.Net.Http;System.Net.Sockets;ClownFish.UnitTest", names4);


        MemoryConfig.RemoveSetting("ClownFish_IgnoreAppParts");


        List<Assembly> list5 = AppPartUtils.GetApplicationPartAsmList0(new List<Assembly>());
        string names5 = string.Join(";", list5.Select(x => x.GetName().Name).ToArray());
        Assert.AreEqual("ClownFish.UnitTest", names5);

    }
}
#endif
