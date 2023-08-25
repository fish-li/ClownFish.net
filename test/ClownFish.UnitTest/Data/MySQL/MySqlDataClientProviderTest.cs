using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data.MultiDB.MySQL;
using ClownFish.UnitTest.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Data.MySQL;
[TestClass]
public class MySqlDataClientProviderTest
{
    [TestMethod]
    public void Test1()
    {
        MySqlDataClientProvider provider = new MySqlDataClientProvider();

        Assert.AreEqual(DatabaseType.MySQL, provider.DatabaseType);

        Assert.AreSame(MySql.Data.MySqlClient.MySqlClientFactory.Instance, provider.ProviderFactory);

        using DbContext dbContext = DbContext.Create("mysql");

        Assert.AreEqual("`Table`", provider.GetObjectFullName("Table"));

        Assert.AreEqual("@Table", provider.GetParamterName("Table", dbContext));

        Assert.AreEqual("@Table", provider.GetParamterPlaceholder("Table", dbContext));
    }


    [TestMethod]
    public void Test_GetNewIdQuery()
    {
        using DbContext dbContext = DbContext.Create("mysql");
        Category c1 = new Category { CategoryName = "手机" };

        string sql = EntityCudUtils.GetInsertSQL(c1, dbContext);
        CPQuery query = dbContext.CPQuery.Create(EntityCudUtils.GetInsertSQL(c1, dbContext), c1);
        query = query.Context.ClientProvider.GetNewIdQuery(query, null);

        string sql2 = query.Command.CommandText;
        Assert.IsTrue(sql2.EndsWith0("; SELECT LAST_INSERT_ID();"));
    }


    [TestMethod]
    public void Test_SetPagedQuery()
    {
        MySqlDataClientProvider provider = new MySqlDataClientProvider();
        using DbContext dbContext = DbContext.Create("mysql");

        var args = new { id = 2 };
        CPQuery query = dbContext.CPQuery.Create("select * from table1 where id = @id", args);

        var query2 = provider.SetPagedQuery(query, 5, 10);
        Assert.IsTrue(query2.Command.CommandText.EndsWith("LIMIT 10 OFFSET 5"));
    }


    [TestMethod]
    public void Test_GetPagedCommand()
    {
        MySqlDataClientProvider provider = new MySqlDataClientProvider();
        using DbContext dbContext = DbContext.Create("mysql");

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
