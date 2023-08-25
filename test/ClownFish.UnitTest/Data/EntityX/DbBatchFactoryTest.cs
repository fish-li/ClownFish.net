#if NET6_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Data;
using ClownFish.UnitTest.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Data.EntityX;

[TestClass]
public class DbBatchFactoryTest
{
    [TestMethod]
    public void Test_Error()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            var factory = new DbBatchFactory(null);
        });


        MyAssert.IsError<NotSupportedException>(() => {
            List<Product> list = BatchInsertTest.CreateList(5);

            using( DbContext dbContext = DbContext.Create("mysql") ) {
                var factory = new DbBatchFactory(dbContext);
                var result = factory.CreateDbBatch(list, CurdKind.Delete);
            }
        });
    }


    internal static List<Product> GetList(DbContext dbContext)
    {
        string sql = dbContext.DatabaseType == DatabaseType.SQLSERVER
                        ? "select top 7 * from Products order by ProductID"
                        : "select * from Products order by ProductID limit 7";

        return dbContext.CPQuery.Create(sql).ToList<Product>();
    }



    [TestMethod]
    public async Task Test_Update()
    {
        await Test_Update(-1);
        await Test_Update(2);
    }

    private async Task Test_Update(int batchSize)
    {
        foreach( var conn in BaseTest.ConnNames ) {
            using( DbContext dbContext = DbContext.Create(conn) ) {
                dbContext.OpenConnection();

                List<Product> list = GetList(dbContext);

                foreach( var x in list ) {
                    x.Quantity++;
                }

                dbContext.BeginTransaction();
                int count1 = dbContext.Batch.Update(list, batchSize);
                dbContext.Commit();
                Assert.AreEqual(list.Count, count1);

                //==============================================

                foreach( var x in list ) {
                    x.Quantity++;
                }
                dbContext.BeginTransaction();
                int count2 = await dbContext.Batch.UpdateAsync(list, batchSize);
                dbContext.Commit();
                Assert.AreEqual(list.Count, count2);
            }
        }
    }


    private static List<BaseCommand> CreateCommandList(DbContext dbContext)
    {
        List<BaseCommand> list = new List<BaseCommand>();

        // 新增一条记录，肯定会执行成功
        string insertSql = "insert into Products(ProductName, CategoryID, Quantity, Unit, UnitPrice, Remark) values (@ProductName, @CategoryID, @Quantity, 'x', 21.34, 'aaaaaaa')";
        var args1 = new { ProductName = "DbBatchFactoryTest_Test_Execute", CategoryID = 547824, Quantity = 22 };
        CPQuery query1 = dbContext.CPQuery.Create(insertSql, args1);

        // 修改一条现有的记录(ProductID = 50)，肯定会执行成功
        string updateSql = "update Products set Quantity = @Quantity where ProductID = @ProductID ";
        var args2 = new { ProductID = 50, Quantity = 33 };
        CPQuery query2 = dbContext.CPQuery.Create(updateSql, args2);

        // 删除一条不存在的记录，执行失败！
        string deleteSql = "delete from Products where ProductID = @ProductID ";
        var args3 = new { ProductID = -1 };
        CPQuery query3 = dbContext.CPQuery.Create(deleteSql, args3);

        list.Add(query1);
        list.Add(query2);
        list.Add(query3);
        list.Add(null);
        return list;
    }


    [TestMethod]
    public void Test_Execute()
    {
        Test_Execute(-1);
        Test_Execute(2);
    }


    private void Test_Execute(int batchSize)
    {
        foreach( var conn in BaseTest.ConnNames ) {
            using( DbContext dbContext = DbContext.Create(conn) ) {

                int count1 = dbContext.Batch.Execute(Empty.List<BaseCommand>(), batchSize);
                Assert.AreEqual(0, count1);

                dbContext.OpenConnection();

                List<BaseCommand> list = CreateCommandList(dbContext);

                dbContext.BeginTransaction();
                int count2 = dbContext.Batch.Execute(list, batchSize);
                dbContext.Commit();

                Assert.AreEqual(2, count2);   // insert, update 执行成功，delete 失败
            }
        }
    }


    [TestMethod]
    public async Task Test_ExecuteAsync()
    {
        await Test_ExecuteAsync(-1);
        await Test_ExecuteAsync(2);
    }


    private async Task Test_ExecuteAsync(int batchSize)
    {
        foreach( var conn in BaseTest.ConnNames ) {
            using( DbContext dbContext = DbContext.Create(conn) ) {

                int count1 = await dbContext.Batch.ExecuteAsync(Empty.List<BaseCommand>(), batchSize);
                Assert.AreEqual(0, count1);


                dbContext.OpenConnection();

                List<BaseCommand> list = CreateCommandList(dbContext);

                dbContext.BeginTransaction();
                await dbContext.Batch.ExecuteAsync(list, batchSize);
                dbContext.Commit();
            }
        }
    }



    [TestMethod]
    public void Test_ExecuteError()
    {
        foreach( var conn in BaseTest.ConnNames ) {
            using( DbContext dbContext = DbContext.Create(conn) ) {

                dbContext.OpenConnection();

                List<BaseCommand> list = new List<BaseCommand>();
                CPQuery query1 = dbContext.CPQuery.Create("delete from tablexxx where id =2");
                list.Add(query1);

                DbContextEvent.OnAfterExecuteBatch += DbContextEvent_OnAfterExecuteBatch;
                s_errorCounter.Reset();
                Exception lastException = null;

                try {
                    dbContext.BeginTransaction();
                    dbContext.Batch.Execute(list);
                    dbContext.Commit();
                }
                catch( Exception ex ) {
                    lastException = ex;
                }
                finally {
                    DbContextEvent.OnAfterExecuteBatch -= DbContextEvent_OnAfterExecuteBatch;
                }

                Assert.IsNotNull(lastException);

                if( dbContext.Factory.CanCreateBatch ) {
                    Assert.AreEqual(1, s_errorCounter.Get());
                }
            }
        }
    }

    [TestMethod]
    public async Task Test_ExecuteErrorAsync()
    {
        foreach( var conn in BaseTest.ConnNames ) {
            using( DbContext dbContext = DbContext.Create(conn) ) {

                dbContext.OpenConnection();

                List<BaseCommand> list = new List<BaseCommand>();
                CPQuery query1 = dbContext.CPQuery.Create("delete from tablexxx where id =2");
                list.Add(query1);

                DbContextEvent.OnAfterExecuteBatch += DbContextEvent_OnAfterExecuteBatch;
                s_errorCounter.Reset();
                Exception lastException = null;

                try {
                    dbContext.BeginTransaction();
                    await dbContext.Batch.ExecuteAsync(list);
                    dbContext.Commit();
                }
                catch( Exception ex ) {
                    lastException = ex;
                }
                finally {
                    DbContextEvent.OnAfterExecuteBatch -= DbContextEvent_OnAfterExecuteBatch;
                }

                Assert.IsNotNull(lastException);

                if( dbContext.Factory.CanCreateBatch ) {
                    Assert.AreEqual(1, s_errorCounter.Get());
                }
            }
        }
    }

    private void DbContextEvent_OnAfterExecuteBatch(object sender, ExecuteBatchEventArgs e)
    {
        s_errorCounter.Increment();
    }

    private static readonly ValueCounter s_errorCounter = new ValueCounter();

}

#endif
