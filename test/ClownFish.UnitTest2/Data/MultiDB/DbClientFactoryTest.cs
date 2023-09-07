using ClownFish.Data.MultiDB.MySQL;

namespace ClownFish.UnitTest.Data.MultiDB;

[TestClass]
public class DbClientFactoryTest
{
    [TestMethod]
    public void Test()
    {

        DbProviderFactory factory4 = DbClientFactory.GetDbProviderFactory("MySql.Data.MySqlClient");
        Assert.AreEqual(MySql.Data.MySqlClient.MySqlClientFactory.Instance, factory4);


    }


    [TestMethod]
    public void Test2()
    {
        using( DbContext dbContext = DbContext.Create("mysql") ) {

            string value1 = dbContext.CPQuery.Create("select now();").ExecuteScalar<string>();

            Assert.IsTrue(dbContext.Connection.GetType().FullName.StartsWith("MySql.Data.MySqlClient.MySqlConnection"));
        }

    }



    [TestMethod]
    public void Test_Error()
    {
        MyAssert.IsError<NotSupportedException>(() => {
            _ = DbClientFactory.GetDbProviderFactory("xxx");
        });


        MyAssert.IsError<ArgumentNullException>(() => {
            DbClientFactory.RegisterProvider(null, MySqlConnectorClientProvider.Instance);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            DbClientFactory.RegisterProvider("xxx", null);
        });
    }

}
