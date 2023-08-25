//using System;
//using System.Collections.Generic;
//using System.Data.SQLite;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using ClownFish.Base.Exceptions;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Npgsql;
//using Npgsql.Internal.TypeHandling;
//using Pgvector;
//using Pgvector.Npgsql;

////<PackageReference Include="Pgvector" Version="0.1.3" />

//namespace ClownFish.UnitTest.Data.PostgreSQL;

//[TestClass]
//public class VectorTest
//{
//    private static readonly string s_dbconfigPG = "DbType=PostgreSQL;Server=LinuxTest;Port=15432;UserName=postgres;Password=1qaz7410;Database=ykf_aidb";

//    [TestMethod]
//    public async Task Test1()
//    {
//        // 参考：https://github.com/pgvector/pgvector-dotnet

//        string connString = DbConfig.Parse(s_dbconfigPG).GetConnectionString(true);

//        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connString);
//        dataSourceBuilder.UseVector();
//        using var dataSource = dataSourceBuilder.Build();

//        var conn = dataSource.OpenConnection();

//        using( var cmd = new NpgsqlCommand("SELECT id, embedding  FROM items ORDER BY embedding <=> $1 LIMIT 5", conn) ) {
//            var embedding = new Vector(new float[] { 1, 1, 1 });
//            cmd.Parameters.AddWithValue(embedding);

//            await using( var reader = await cmd.ExecuteReaderAsync() ) {
//                while( await reader.ReadAsync() ) {
//                    Console.WriteLine((Vector)reader.GetValue(1));
//                }
//            }
//        }
//    }


//    [TestMethod]
//    public async Task Test2()
//    {
//        DbConfig dbConfig = DbConfig.Parse(s_dbconfigPG);

//        var dataSourceBuilder = new NpgsqlDataSourceBuilder(dbConfig.GetConnectionString(true));
//        dataSourceBuilder.UseVector();
//        using var dataSource = dataSourceBuilder.Build();

//        using var conn = dataSource.OpenConnection();

//        using DbContext dbContext = DbContext.Create(conn, dbConfig.GetProviderName());

//        string sql = "SELECT id, embedding  FROM items ORDER BY embedding <=> @embedding LIMIT 5";
//        var args = new { embedding = new Vector(new float[] { 1, 1, 1 }) };

//        using( var reader = await dbContext.CPQuery.Create(sql, args).ExecuteReaderAsync() ) {
//            while( await reader.ReadAsync() ) {
//                Console.WriteLine((Vector)reader.GetValue(1));
//            }
//        }
//    }


//    [TestMethod]
//    public void Test3()
//    {
//        Test3b();
//        Test3b();
//        Test3b();
//    }

//    private void Test3b()
//    {
//        using DbContext db = DbConfig.Parse(s_dbconfigPG).CreateDbContext(true);
//        db.EnablePgvector();

//        DataTable table = db.CPQuery.Create("select * from items").ToDataTable();

//        foreach( DataRow row in table.Rows ) {
//            Console.WriteLine($"id={row[0]}; embedding={row[1]}");
//        }
//    }

//}


//internal static class PgvectorExtenions
//{
//    public static void EnablePgvector(this DbContext db)
//    {
//        // Npgsql 的新版本太SB了，注册 TypeHandlerResolverFactory 必须经过 NpgsqlDataSourceBuilder
//        // 太太太麻烦了，所以这里采用暴力方式来解决 ~~~

//        Npgsql.NpgsqlConnection conn = (Npgsql.NpgsqlConnection)db.Connection;

//        NpgsqlDataSource dataSource = (NpgsqlDataSource)conn.GetType().InvokeMember("NpgsqlDataSource",
//                                        BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, conn, null);

//        // NpgsqlDataSourceConfiguration 
//        object configuration = dataSource.GetType().InvokeMember("Configuration",
//                                        BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, dataSource, null);

//        List<TypeHandlerResolverFactory> list = (List<TypeHandlerResolverFactory>)configuration.GetType().InvokeMember("ResolverFactories",
//                                        BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, configuration, null);

//        lock( list ) {
//            if( list.Any(x => x is VectorTypeHandlerResolverFactory) == false )
//                list.Add(new VectorTypeHandlerResolverFactory());
//        }


//        // 下面这个方法不靠谱，目前在 7.0 版本中标记了 [Obsolete] ，未来可能会删除，所以放弃使用！

//        // TODO: 程序启动时执行一次
//        //#pragma warning disable CS0618
//        // NpgsqlConnection.GlobalTypeMapper.UseVector();
//        //#pragma warning restore CS0618
//    }
//}

