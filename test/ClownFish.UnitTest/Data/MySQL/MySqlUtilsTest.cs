using ClownFish.Data.MultiDB.MySQL;

namespace ClownFish.UnitTest.Data.MySQL;

[TestClass]
public class MySqlUtilsTest
{
    [TestMethod]
    public void Test_RegisterMySqlProvider()
    {
        MySqlProviderUtils.RegisterProvider(1);
        BaseClientProvider client1 = DbClientFactory.GetProvider(DatabaseClients.MySqlClient);
        Assert.AreEqual(MySqlDataClientProvider.Instance, client1);
        Assert.AreEqual("MySql.Data.MySqlClient", MySqlProviderUtils.GetCurrentProviderName());


        MySqlProviderUtils.RegisterProvider(2);
        BaseClientProvider client2 = DbClientFactory.GetProvider(DatabaseClients.MySqlClient);
        Assert.AreEqual(MySqlConnectorClientProvider.Instance, client2);
        Assert.AreEqual("MySqlConnector", MySqlProviderUtils.GetCurrentProviderName());


        MyAssert.IsError<ArgumentOutOfRangeException>(() => {
            MySqlProviderUtils.RegisterProvider(22);
        });
    }


}
