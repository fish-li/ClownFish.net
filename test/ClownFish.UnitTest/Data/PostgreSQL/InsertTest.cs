using ClownFish.UnitTest.Data.Models;
using Npgsql;

namespace ClownFish.UnitTest.Data.PostgreSQL;

[TestClass]
public class InsertTest
{
    [TestMethod]
    public void Test_Duplicate_insert()
    {
        //ClownFish.Data.DbExceuteException: 23505: duplicate key value violates unique constraint "ix_categoryname"
        // --->Npgsql.PostgresException(0x80004005): 23505: duplicate key value violates unique constraint "ix_categoryname"

        var args = new {
            name = "手机xx1"
        };

        Exception exception = null;
        using( DbContext dbContext = DbContext.Create("postgresql") ) {

            // 先执行删除，清空环境
            dbContext.CPQuery.Create("delete from categories where CategoryName = @name", args).ExecuteNonQuery();

            try {
                dbContext.CPQuery.Create("insert into Categories (CategoryName) values( @name )", args).ExecuteNonQuery();
                dbContext.CPQuery.Create("insert into Categories (CategoryName) values( @name )", args).ExecuteNonQuery();
            }
            catch( Exception ex ) {
                exception = ex;
            }

            Assert.IsNotNull(exception);
            Console.WriteLine(exception.ToString());

            Assert.IsTrue(exception.GetBaseException() is PostgresException);
            Assert.IsTrue(exception.Message.Contains("23505: duplicate key value violates unique constraint \"ix_categoryname\""));

            Assert.IsTrue(dbContext.IsDuplicateInsert(exception));
        }
    }



    [TestMethod]
    public void Test_Duplicate_insert2()
    {
        using( DbContext dbContext = DbContext.Create("postgresql") ) {
            Category category1 = new Category {
                CategoryName = "手机xx2"
            };

            // 先执行删除，清空环境
            dbContext.CPQuery.Create("delete from categories where CategoryName = @CategoryName", category1).ExecuteNonQuery();

            var successed = category1.Insert2(dbContext);
            Assert.IsTrue(successed);


            successed = category1.Insert2(dbContext);
            Assert.IsFalse(successed);

        }
    }
}

