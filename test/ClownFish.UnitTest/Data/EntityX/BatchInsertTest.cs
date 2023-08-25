using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Data;
using ClownFish.Log;
using ClownFish.Log.Logging;
using ClownFish.UnitTest.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Data.EntityX
{
#if NET6_0_OR_GREATER

    [TestClass]
    public class BatchInsertTest
    {
        [TestMethod]
        public void Test_Sync()
        {
            Test_Sync(-1);   // 不分批
            Test_Sync(2);    // 2条记录一个批
        }


        private void Test_Sync(int batchSize)
        {
            List<Product> list = CreateList(7);

            foreach( var conn in BaseTest.ConnNames ) {
                using( DbContext dbContext = DbContext.Create(conn) ) {
                    var count1 = dbContext.Batch.Insert(Empty.List<Product>(), batchSize);
                    Assert.AreEqual(0, count1);

                    var count2 = dbContext.Batch.Insert(list, batchSize);
                    Assert.AreEqual(7, count2);
                }

                using( DbContext dbContext = DbContext.Create(conn) ) {
                    dbContext.CPQuery.Create("delete from products where CategoryID = 547823").ExecuteNonQuery();
                }
            }
        }


        [TestMethod]
        public async Task Test_Sync_WithTransaction()
        {
            await Test_Sync_WithTransaction(-1);
            await Test_Sync_WithTransaction(2);
        }


        private async Task Test_Sync_WithTransaction(int batchSize)
        {
            List<Product> list = CreateList(7);

            foreach( var conn in BaseTest.ConnNames ) {
                using( DbContext dbContext = DbContext.Create(conn) ) {

                    var count1 = await dbContext.Batch.InsertAsync(Empty.List<Product>(), batchSize);
                    Assert.AreEqual(0, count1);


                    dbContext.BeginTransaction();

                    var count2 = dbContext.Batch.Insert(list, batchSize);
                    Assert.AreEqual(7, count2);

                    dbContext.Commit();
                }

                using( DbContext dbContext = DbContext.Create(conn) ) {
                    dbContext.CPQuery.Create("delete from products where CategoryID = 547823").ExecuteNonQuery();
                }
            }
        }


        [TestMethod]
        public async Task Test_Async()
        {
            await Test_Async(-1);
            await Test_Async(2);
        }

        private async Task Test_Async(int batchSize)
        {
            List<Product> list = CreateList(7);

            foreach( var conn in BaseTest.ConnNames ) {
                using( DbContext dbContext = DbContext.Create(conn) ) {
                    var count = await dbContext.Batch.InsertAsync(list, batchSize);
                    Assert.AreEqual(7, count);
                }


                using( DbContext dbContext = DbContext.Create(conn) ) {
                    await dbContext.CPQuery.Create("delete from products where CategoryID = 547823").ExecuteNonQueryAsync();
                }
            }
        }


        [TestMethod]
        public async Task Test_Async_WithTransaction()
        {
            await Test_Async_WithTransaction(-1);
            await Test_Async_WithTransaction(2);
        }
        private async Task Test_Async_WithTransaction(int batchSize)
        {
            using OprLogScope scope = OprLogScope.Start();

            List<Product> list = CreateList(7);

            using( DbContext dbContext = DbContext.Create("mysql") ) {
                dbContext.BeginTransaction();

                var count = await dbContext.Batch.InsertAsync(list, batchSize);
                Assert.AreEqual(7, count);

                dbContext.Commit();
            }

            using( DbContext dbContext = DbContext.Create("mysql") ) {
                await dbContext.CPQuery.Create("delete from products where CategoryID = 547823").ExecuteNonQueryAsync();
            }

            List<StepItem> stepItems = scope.GetStepItems();

            StepItem step = scope.GetStepItems().First(x => x.StepName == "SQL_BatchInsert_Async");
            Assert.AreEqual("sqlbatch", step.StepKind);
            Assert.AreEqual(1, step.IsAsync);
            Assert.IsTrue(step.Cmdx is DbBatch);
        }


        internal static List<Product> CreateList(int count)
        {
            List<Product> list = new List<Product>(count);

            for( int i = 0; i < count; i++ ) {
                Product product = new Product {
                    ProductName = $"BatchInsertTest_{i}_{Guid.NewGuid().ToString("N")}",
                    CategoryID = 547823,
                    Quantity = 999888
                };
                product.LoadDefaultValues();
                list.Add(product);
            }

            return list;
        }



        [TestMethod]
        public void Test_DbBatch_ToLoggingText()
        {
            List<Product> list = CreateList(5);

            using( DbContext dbContext = DbContext.Create("mysql") ) {

                DbBatch batch = dbContext.Batch.CreateDbBatch(list, CurdKind.Insert);

                string text = DbCommandSerializer.ToLoggingText(batch);

                Console.WriteLine(text);

                Assert.IsTrue(text.StartsWith("INSERT INTO Products("));
                Assert.IsTrue(text.Contains("RecordsAffected: 5"));

            }
        }

    }

#endif

}
