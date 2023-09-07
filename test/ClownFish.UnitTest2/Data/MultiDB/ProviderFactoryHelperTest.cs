namespace ClownFish.UnitTest.Data.MultiDB;

[TestClass]
public class ProviderFactoryHelperTest : BaseTest
{
    [TestMethod]
    public void Test_GetDbProviderFactory()
    {
        DbProviderFactory factory3 = DbClientFactory.GetDbProviderFactory(DatabaseClients.MySqlClient);

        Assert.IsTrue(object.ReferenceEquals(MySql.Data.MySqlClient.MySqlClientFactory.Instance, factory3));
    }

}
