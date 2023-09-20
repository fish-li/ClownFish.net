using ClownFish.Data.MultiDB.MySQL;

namespace ClownFish.UnitTest.Data.MySQL;

[TestClass]
public class MySqlUtilsTest
{
    [TestMethod]
    public void Test_RegisterMySqlProvider()
    {
        MySqlProviderUtils.RegisterProvider(2);
        BaseClientProvider client1 = DbClientFactory.GetProvider(DatabaseClients.MySqlClient);
        Assert.AreEqual(MySqlConnectorClientProvider.Instance, client1);
        Assert.AreEqual("MySql.Data.MySqlClient", MySqlProviderUtils.GetCurrentProviderName());
    }


}
