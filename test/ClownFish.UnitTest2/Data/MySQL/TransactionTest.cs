namespace ClownFish.UnitTest.Data.MySQL;

[TestClass]
public class TransactionTest
{
    [TestMethod]
    public void Test_Cross_database_transaction()
    {
        using( DbContext dbContext = DbContext.Create("mysql") ) {

            dbContext.BeginTransaction();

            var args1 = new { p1 = "abc" };
            dbContext.CPQuery.Create("insert into Trans1 (StrValue) values ( @p1 )", args1).ExecuteNonQuery();


            dbContext.ChangeDatabase("TestTrans");

            var args2 = new { p1 = 123 };
            dbContext.CPQuery.Create("insert into Trans2 (IntValue) values ( @p1 )", args2).ExecuteNonQuery();

            dbContext.Commit();
        }
    }

}
