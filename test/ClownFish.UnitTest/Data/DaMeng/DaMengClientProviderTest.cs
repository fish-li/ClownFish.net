#if NETCOREAPP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data;
using ClownFish.Data.MultiDB.DaMeng;
using ClownFish.UnitTest.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Data.DaMeng;


[TestClass]
public class DaMengClientProviderTest
{
    [TestMethod]
    public void Test1()
    {
        DaMengClientProvider provider = new DaMengClientProvider();

        Assert.AreEqual(DatabaseType.DaMeng, provider.DatabaseType);

        Assert.AreSame(Dm.DmClientFactory.Instance, provider.ProviderFactory);

        using DbContext dbContext = DbContext.Create("dm");

        Assert.AreEqual("\"Table\"", provider.GetObjectFullName("Table"));

        Assert.AreEqual(":Table", provider.GetParamterName("Table", dbContext));

        Assert.AreEqual(":Table", provider.GetParamterPlaceholder("Table", dbContext));

        dbContext.EnableDelimiter = true;
        Assert.AreEqual(":\"Table\"", provider.GetParamterPlaceholder("Table", dbContext));        
    }


    [TestMethod]
    public void Test_PrepareCommand()
    {
        DaMengClientProvider provider = new DaMengClientProvider();
        using DbContext dbContext = DbContext.Create("dm");

        CPQuery query = dbContext.CPQuery.Create("select * from table1 where id = @id");

        DbParameter parameter = query.Command.CreateParameter();
        parameter.ParameterName = "@id";
        parameter.Value = 2;
        query.Command.Parameters.Add(parameter);

        provider.PrepareCommand(query.Command, dbContext);
        Assert.AreEqual("select * from table1 where id = :id", query.Command.CommandText);

        foreach( DbParameter p in query.Command.Parameters ) {
            Assert.IsTrue(p.ParameterName[0] == ':');
        }
    }


    [TestMethod]
    public void Test_GetNewIdQuery()
    {
        using DbContext dbContext = DbContext.Create("dm");
        Category c1 = new Category { CategoryName = "手机" };

        string sql = EntityCudUtils.GetInsertSQL(c1, dbContext);
        CPQuery query = dbContext.CPQuery.Create(EntityCudUtils.GetInsertSQL(c1, dbContext), c1);
        query = query.Context.ClientProvider.GetNewIdQuery(query, c1);

        string sql2 = query.Command.CommandText;
        Assert.IsTrue(sql2.EndsWith0("; SELECT IDENT_CURRENT('Categories');"));
    }

    [TestMethod]
    public void Test_GetNewIdQuery_2()
    {
        using DbContext dbContext = DbContext.Create("dm");
        Category c1 = new Category { CategoryName = "手机" };

        string sql = EntityCudUtils.GetInsertSQL(c1, dbContext);
        CPQuery query = dbContext.CPQuery.Create(EntityCudUtils.GetInsertSQL(c1, dbContext), c1);

        MyAssert.IsError<ArgumentOutOfRangeException>(() => {
            query = query.Context.ClientProvider.GetNewIdQuery(query, new object());
        });

        query = query.Context.ClientProvider.GetNewIdQuery(query, null);

        string sql2 = query.Command.CommandText;
        Assert.AreEqual(sql2, sql);
    }

    [TestMethod]
    public void Test_SetPagedQuery()
    {
        DaMengClientProvider provider = new DaMengClientProvider();
        using DbContext dbContext = DbContext.Create("dm");

        var args = new { id = 2 };
        CPQuery query = dbContext.CPQuery.Create("select * from table1 where id = @id", args);

        var query2 = provider.SetPagedQuery(query, 5, 10);
        Assert.IsTrue(query2.Command.CommandText.EndsWith("LIMIT 10 OFFSET 5"));
    }


    [TestMethod]
    public void Test_GetPagedCommand()
    {
        DaMengClientProvider provider = new DaMengClientProvider();
        using DbContext dbContext = DbContext.Create("dm");

        var args = new { id = 2 };
        CPQuery query = dbContext.CPQuery.Create("select * from table1 where id = @id", args);

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
#endif

