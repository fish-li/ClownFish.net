using ClownFish.Data.MultiDB.PostgreSQL;

namespace ClownFish.UnitTest.Data.MultiDB;

[TestClass]
public class BaseClientProviderTest
{
    [TestMethod]
    public void Test_1()
    {
        BaseClientProvider client = DbClientFactory.GetProvider(XxxBaseClientProvider.ProviderName);

        Assert.AreEqual((DatabaseType)147852, client.DatabaseType);
        Assert.AreEqual(Npgsql.NpgsqlFactory.Instance, client.ProviderFactory);
        Assert.AreEqual("/abc/", client.GetObjectFullName("abc"));
        Assert.AreEqual("@abc", client.GetParamterName("abc"));
        Assert.AreEqual("@abc", client.GetParamterPlaceholder("abc"));


        using( DbContext db = DbContext.Create("pgxxx") ) {

            Assert.AreEqual(db.DatabaseType, client.DatabaseType);
            Assert.AreEqual(db.ProviderName, XxxBaseClientProvider.ProviderName);
            Assert.AreEqual(db.ConnectionInfo.ProviderName, XxxBaseClientProvider.ProviderName);
            Assert.IsTrue(object.ReferenceEquals(db.Factory, Npgsql.NpgsqlFactory.Instance));
            Assert.IsTrue(object.ReferenceEquals(db.ClientProvider, XxxBaseClientProvider.Instance));
        }
    }

#if NET8_0_OR_GREATER
    [TestMethod]
    public void Test_2() 
    {
        using( DbContext db = DbContext.Create("kingbase2") ) {

            Assert.AreEqual((DatabaseType)7777, db.DatabaseType);
            Assert.AreEqual("Kdbndp", db.ProviderName);        
            
            long id = db.CPQuery.Create("select max(productid) from products").ExecuteScalar<long>();
            Assert.IsTrue(id > 0);
        }


        using( DbContext db = DbContext.Create("kingbase3") ) {

            Assert.AreEqual((DatabaseType)7777, db.DatabaseType);
            Assert.AreEqual("Kdbndp", db.ProviderName);

            long id = db.CPQuery.Create("select max(productid) from products").ExecuteScalar<long>();
            Assert.IsTrue(id > 0);
        }
    }
#endif
}


public class XxxBaseClientProvider : BaseClientProvider
{
    public static readonly BaseClientProvider Instance = new XxxBaseClientProvider();

    public static readonly string ProviderName = "Test.xSqlClient";

    public XxxBaseClientProvider()
    {
    }

    public override DatabaseType DatabaseType => (DatabaseType)147852;

    public override DbProviderFactory ProviderFactory => Npgsql.NpgsqlFactory.Instance;

    public override string GetConnectionString(IDbConfig dbConfig, bool includeDatabase)
    {
        return PostgreSqlClientProvider.GetPostgreSQLConnectionString0(dbConfig, includeDatabase);
    }

    public override string GetObjectFullName(string symbol)
    {
        return "/" + base.GetObjectFullName(symbol) + "/";
    }


    public override CPQuery GetNewIdQuery(CPQuery query, object entity)
    {
        return query + "; SELECT lastval();";
    }

    public override bool IsDuplicateInsertException(Exception ex)
    {
        if( ex is Npgsql.PostgresException ex2 ) {
            return ex2.SqlState == "23505";
        }

        return false;
    }

    public override CPQuery SetPagedQuery(CPQuery query, int skip, int take)
    {
        return StdClientProvider.SetPagedQuery(query, skip, take);
    }

    public override Page2Query GetPagedCommand(BaseCommand query, PagingInfo pagingInfo)
    {
        return StdClientProvider.GetPagedCommand(query, pagingInfo);
    }

}


