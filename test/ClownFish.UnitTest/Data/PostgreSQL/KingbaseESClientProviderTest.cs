using ClownFish.UnitTest.Data.Models;

namespace ClownFish.UnitTest.Data.PostgreSQL;

using ClownFish.Data.MultiDB.PostgreSQL;

#if TEST_KINGBASE2

using Kdbndp;


[TestClass]
public class KingbaseESClientProviderTest
{
    [TestMethod]
    public void Test1()
    {
        KingbaseESClientProvider provider = new KingbaseESClientProvider();

        Assert.AreEqual((DatabaseType)7777, provider.DatabaseType);

        Assert.AreSame(Kdbndp.KdbndpFactory.Instance, provider.ProviderFactory);

        using DbContext dbContext = DbContext.Create("kingbase2");

        Assert.AreEqual("\"Table\"", provider.GetObjectFullName("Table"));

        Assert.AreEqual("@Table", provider.GetParamterName("Table", dbContext));

        Assert.AreEqual("@Table", provider.GetParamterPlaceholder("Table", dbContext));
    }


    [TestMethod]
    public void Test_GetNewIdQuery()
    {
        using DbContext dbContext = DbContext.Create("kingbase2");
        Category c1 = new Category { CategoryName = "手机" };

        string sql = EntityCudUtils.GetInsertSQL(c1, dbContext);
        CPQuery query = dbContext.CPQuery.Create(EntityCudUtils.GetInsertSQL(c1, dbContext), c1);
        query = query.Context.ClientProvider.GetNewIdQuery(query, null);

        string sql2 = query.Command.CommandText;
        Assert.IsTrue(sql2.EndsWith0("; SELECT lastval();"));
    }


    [TestMethod]
    public void Test_SetPagedQuery()
    {
        KingbaseESClientProvider provider = new KingbaseESClientProvider();
        using DbContext dbContext = DbContext.Create("kingbase2");

        var args = new { id = 2 };
        CPQuery query = dbContext.CPQuery.Create("select * from table1 where id = @id", args);

        var query2 = provider.SetPagedQuery(query, 5, 10);
        Assert.IsTrue(query2.Command.CommandText.EndsWith("LIMIT 10 OFFSET 5"));
    }


    [TestMethod]
    public void Test_GetPagedCommand()
    {
        KingbaseESClientProvider provider = new KingbaseESClientProvider();
        using DbContext dbContext = DbContext.Create("kingbase2");

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


    [TestMethod]
    public void Test_IsDuplicateInsertException()
    {
        KingbaseESClientProvider provider = new KingbaseESClientProvider();

        KingbaseException ex = new KingbaseException("xx-message", "xx-severity", "xxxx", "23505");
        Assert.IsTrue(provider.IsDuplicateInsertException(ex));

        KingbaseException ex2 = new KingbaseException("xx-message", "xx-severity", "xxxx", "11111");
        Assert.IsFalse(provider.IsDuplicateInsertException(ex2));


        Assert.IsFalse(provider.IsDuplicateInsertException(ExceptionHelper.CreateException()));
    }
}

#endif
