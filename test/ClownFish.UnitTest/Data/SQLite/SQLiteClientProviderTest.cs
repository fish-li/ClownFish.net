using ClownFish.Data.MultiDB.SQLite;
using ClownFish.UnitTest.Data.Models;

namespace ClownFish.UnitTest.Data.SQLite;

[TestClass]
public class SQLiteClientProviderTest
{
    [TestMethod]
    public void Test1()
    {
        SQLiteClientProvider provider = new SQLiteClientProvider();

        Assert.AreEqual(DatabaseType.SQLite, provider.DatabaseType);

        Assert.AreSame(System.Data.SQLite.SQLiteFactory.Instance, provider.ProviderFactory);

        using DbContext dbContext = DbContext.Create("xxxxxxxxxxx", "System.Data.SQLite");

        Assert.AreEqual("[Table]", provider.GetObjectFullName("Table"));

        Assert.AreEqual("@Table", provider.GetParamterName("Table", dbContext));

        Assert.AreEqual("@Table", provider.GetParamterPlaceholder("Table", dbContext));
    }


    [TestMethod]
    public void Test_GetNewIdQuery()
    {
        using DbContext dbContext = DbContext.Create("xxxxxxxxxxx", "System.Data.SQLite");
        Category c1 = new Category { CategoryName = "手机" };

        string sql = EntityCudUtils.GetInsertSQL(c1, dbContext);
        CPQuery query = dbContext.CPQuery.Create(EntityCudUtils.GetInsertSQL(c1, dbContext), c1);
        query = query.Context.ClientProvider.GetNewIdQuery(query, null);

        string sql2 = query.Command.CommandText;
        Assert.IsTrue(sql2.EndsWith0("; SELECT last_insert_rowid();"));
    }


    [TestMethod]
    public void Test_SetPagedQuery()
    {
        SQLiteClientProvider provider = new SQLiteClientProvider();
        using DbContext dbContext = DbContext.Create("xxxxxxxxxxx", "System.Data.SQLite");

        var args = new { id = 2 };
        CPQuery query = dbContext.CPQuery.Create("select * from table1 where id = @id", args);

        var query2 = provider.SetPagedQuery(query, 5, 10);
        Assert.IsTrue(query2.Command.CommandText.EndsWith("LIMIT 10 OFFSET 5"));
    }


    [TestMethod]
    public void Test_GetPagedCommand()
    {
        SQLiteClientProvider provider = new SQLiteClientProvider();
        using DbContext dbContext = DbContext.Create("xxxxxxxxxxx", "System.Data.SQLite");

        var args = new { id = 2 };
        CPQuery query = dbContext.CPQuery.Create("select * from table1 where id = @id order by id", args);

        PagingInfo pagingInfo = new PagingInfo {
            PageIndex = 0,
            PageSize = 10
        };

        var query2 = provider.GetPagedCommand(query, pagingInfo);
        Console.WriteLine(query2.ListQuery.Command.CommandText);
        Console.WriteLine(query2.CountQuery.Command.CommandText);

        Assert.IsTrue(query2.ListQuery.Command.CommandText.EndsWith("LIMIT 10 OFFSET 0"));
        Assert.IsTrue(query2.CountQuery.Command.CommandText.StartsWith("SELECT COUNT(*) AS totalrows FROM ("));
    }

}


