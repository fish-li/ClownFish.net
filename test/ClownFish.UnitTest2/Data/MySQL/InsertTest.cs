using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Data;
using ClownFish.UnitTest.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace ClownFish.UnitTest.Data.MySQL
{
    [TestClass]
    public class InsertTest
    {
        [TestMethod]
        public void Test_Duplicate_insert()
        {
            //ClownFish.Data.DbExceuteException: Duplicate entry '手机' for key 'IX_CategoryName'
            //--->MySql.Data.MySqlClient.MySqlException(0x80004005): Duplicate entry '手机' for key 'IX_CategoryName'

            var args = new {
                name = "手机xx1"
            };

            Exception exception = null;
            using( DbContext dbContext = DbContext.Create("mysql") ) {

                // 先执行删除，清空环境
                dbContext.CPQuery.Create("delete from categories where CategoryName = @name", args).ExecuteNonQuery();

                try {                   
                    dbContext.CPQuery.Create("insert into categories(CategoryName) values( @name )", args).ExecuteNonQuery();
                    dbContext.CPQuery.Create("insert into categories(CategoryName) values( @name )", args).ExecuteNonQuery();
                }
                catch( Exception ex ) {
                    exception = ex;
                }

                Assert.IsTrue(dbContext.Connection.GetType().FullName.StartsWith("MySql.Data.MySqlClient.MySqlConnection"));

                Assert.IsNotNull(exception);
                Console.WriteLine(exception.ToString());

                Assert.IsTrue(exception.GetBaseException() is MySql.Data.MySqlClient.MySqlException);

                // 不同的MYSQL版本的异常消息不一样，目前发现有以下2种：
                // Duplicate entry '手机xx1' for key 'IX_CategoryName'
                // Duplicate entry '手机xx1' for key 'categories.IX_CategoryName'
                Assert.IsTrue(exception.Message.StartsWith0("Duplicate entry '手机xx1' for key "));

                Assert.IsTrue(dbContext.IsDuplicateInsert(exception));
            }
        }
         

        [TestMethod]
        public void Test_Duplicate_insert2()
        {
            using( DbContext dbContext = DbContext.Create("mysql") ) {
                Category category1 = new Category {
                    CategoryName = "手机xx2"
                };

                // 先执行删除，清空环境
                dbContext.CPQuery.Create("delete from categories where CategoryName = @CategoryName", category1).ExecuteNonQuery();

                var successed = category1.Insert2(dbContext);      // 第一次成功插入
                Assert.IsTrue(successed);


                successed = category1.Insert2(dbContext);          // 唯一索引导致插入时发生异常 
                Assert.IsFalse(successed);

                Assert.IsTrue(dbContext.Connection.GetType().FullName.StartsWith("MySql.Data.MySqlClient.MySqlConnection"));
            }
        }



        [TestMethod]
        public async Task Test_Duplicate_insert2Async()
        {
            using( DbContext dbContext = DbContext.Create("mysql") ) {
                Category category1 = new Category {
                    CategoryName = "手机xx2a"
                };

                // 先执行删除，清空环境
                await dbContext.CPQuery.Create("delete from categories where CategoryName = @CategoryName", category1).ExecuteNonQueryAsync();

                var result1 = await category1.Insert2Async(dbContext);       // 第一次成功插入
                Assert.IsTrue(result1);

                result1 = await category1.Insert2Async(dbContext);          // 唯一索引导致插入时发生异常 
                Assert.IsFalse(result1);


                Assert.IsTrue(dbContext.Connection.GetType().FullName.StartsWith("MySql.Data.MySqlClient.MySqlConnection"));
            }
        }

    }
}

