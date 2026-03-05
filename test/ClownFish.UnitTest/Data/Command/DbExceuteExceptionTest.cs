namespace ClownFish.UnitTest.Data.Command;
[TestClass]
public class DbExceuteExceptionTest
{
    [TestMethod]
    public void Test1()
    {
        Exception ex1 = ExceptionHelper.CreateException("message1xxxxxxx");
        using DbContext db1 = DbContext.Create("s1");
        CPQuery query = db1.CPQuery.Create("select * from table1");

        DbExceuteException ex2 = new DbExceuteException(ex1, query.Command);

        Assert.AreEqual("message1xxxxxxx", ex2.Message);
        Assert.AreEqual("select * from table1", ex2.Command.CommandText);
        
        Assert.AreEqual("Data Source=MsSqlHost;Initial Catalog=MyNorthwind;User ID=user1;Password=qaz1@wsx;Application Name=ClownFish.UnitTest", ex2.ConnectionString);



        MyAssert.IsError<NullReferenceException>(() => {
            _ = new DbExceuteException(null, query.Command);
        });


        MyAssert.IsError<ArgumentNullException>(() => {
            _ = new DbExceuteException(ex1, null);
        });
    }


}
