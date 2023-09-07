namespace ClownFish.UnitTest.Data.Context;

[TestClass]
public  class ConnectionInfoTest
{
    [TestMethod]
    public void Test_DatabaseType()
    {
        Assert.AreEqual(DatabaseType.SQLSERVER, DbClientFactory.GetProvider(null).DatabaseType);
        Assert.AreEqual(DatabaseType.SQLSERVER, DbClientFactory.GetProvider(DatabaseClients.SqlClient).DatabaseType);
        Assert.AreEqual(DatabaseType.MySQL, DbClientFactory.GetProvider(DatabaseClients.MySqlClient).DatabaseType);
        Assert.AreEqual(DatabaseType.PostgreSQL, DbClientFactory.GetProvider(DatabaseClients.PostgreSQL).DatabaseType);
    }

    [TestMethod]
    public void Test_Error()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _= new ConnectionInfo(null, "providerName");
        });
    }










}
