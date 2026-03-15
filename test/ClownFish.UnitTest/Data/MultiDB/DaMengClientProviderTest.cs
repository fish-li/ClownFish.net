using System;
using System.Collections.Generic;
using System.Text;
using ClownFish.Data.MultiDB.DaMeng;
using ClownFish.UnitTest.Data.Models;

namespace ClownFish.UnitTest.Data.MultiDB;

#if NETCOREAPP

[TestClass]
public class DaMengClientProviderTest
{
    [TestMethod]
    public void Test_1()
    {
        Assert.IsNotNull(DaMengClientProvider.Instance);
        Assert.IsNotNull(DaMengClientProvider.Instance.ProviderFactory);
        Assert.AreEqual(DatabaseType.DaMeng, DaMengClientProvider.Instance.DatabaseType);
        Assert.AreEqual(@"""database""", DaMengClientProvider.Instance.GetObjectFullName("database"));
        Assert.AreEqual(@":database", DaMengClientProvider.Instance.GetParamterName("database"));
        Assert.AreEqual(@":database", DaMengClientProvider.Instance.GetParamterPlaceholder("database"));
    }

    [TestMethod]
    public void Test_GetNewIdQuery()
    {
        using DbContext dbContext = DbContext.Create("dm1");
        CPQuery query = dbContext.CPQuery.Create("select 1;");

        Category category = new Category();

        CPQuery query2 = DaMengClientProvider.Instance.GetNewIdQuery(query, category);
        string sql = query2.Command.CommandText;
        Assert.AreEqual("select 1;; SELECT IDENT_CURRENT('Categories');", sql);
    }

    [TestMethod]
    public void Test_PrepareCommand()
    {
        using DbContext dbContext = DbContext.Create("dm1");
        var args = new { CategoryID = 3, Quantity = 5 };
        CPQuery query = dbContext.CPQuery.Create("select * from Products where CategoryID = @CategoryID and Quantity < @Quantity", args);
        DbCommand command = query.Command;

        DaMengClientProvider.Instance.PrepareCommand(command, dbContext);

        Assert.AreEqual("select * from Products where CategoryID = :CategoryID and Quantity < :Quantity", command.CommandText);
        Assert.AreEqual(":CategoryID", command.Parameters[0].ParameterName);
        Assert.AreEqual(":Quantity", command.Parameters[1].ParameterName);
    }

    [TestMethod]
    public void Test_GetConnectionString()
    {
        DbConfig dbConfig = DbConnManager.GetAppDbConfig("dm1");
        Assert.IsNotNull (dbConfig);

        string connectionstring = DaMengClientProvider.Instance.GetConnectionString(dbConfig, true);
        Assert.AreEqual("server=PgSqlHost;port=15236;database=MyNorthwind;user=SYSDBA;password=SYSDBA001;app_name=ClownFish.UnitTest", connectionstring);


        DbConfig dbConfig2 = dbConfig.Clone();
        dbConfig2.Args = "aa=123";

        string connectionstring2 = DaMengClientProvider.Instance.GetConnectionString(dbConfig2, true);
        Assert.AreEqual("server=PgSqlHost;port=15236;database=MyNorthwind;user=SYSDBA;password=SYSDBA001;app_name=ClownFish.UnitTest;aa=123", connectionstring2);
    }
}

#endif
