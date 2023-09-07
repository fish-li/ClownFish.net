namespace ClownFish.UnitTest.Data.Events;

[TestClass]
public class BaseEventArgsTest
{
    [TestMethod]
    public void Test()
    {
        using( DbContext db = DbContext.Create() ) {

            var query = db.CPQuery.Create("select getdate()");
            var xx = query.ExecuteScalar<string>();

            BaseEventArgs e = new BaseEventArgs {
                DbContext = db,
                Exception = ExceptionHelper.CreateException(),
                StartTime = DateTime.Now.AddSeconds(-3),
                EndTime = DateTime.Now
            };


            Assert.IsNotNull(e.DbContext);
            Assert.IsNotNull(e.Exception);
            Assert.IsTrue(e.StartTime.Year > 2000);
            Assert.IsTrue(e.EndTime.Year > 2000);



            OpenConnEventArgs e2 = new OpenConnEventArgs {
                DbContext = db,
                Exception = ExceptionHelper.CreateException(),
                StartTime = DateTime.Now.AddSeconds(-3),
                EndTime = DateTime.Now,
                Connection = db.Connection,
                IsAsync = true
            };

            Assert.IsNotNull(e2.DbContext);
            Assert.IsNotNull(e2.Exception);
            Assert.IsTrue(e2.StartTime.Year > 2000);
            Assert.IsTrue(e2.EndTime.Year > 2000);
            Assert.IsNotNull(e2.Connection);
            Assert.AreEqual(true, e2.IsAsync);



            ExecuteCommandEventArgs e3 = new ExecuteCommandEventArgs {
                DbContext = db,
                Exception = ExceptionHelper.CreateException(),
                StartTime = DateTime.Now.AddSeconds(-3),
                EndTime = DateTime.Now,
                OperationId = "xxxxxxxxxxxxxxxxx",
                OperationName = "getdate",
                Command = query,
                IsAsync = true
            };

            Assert.IsNotNull(e3.DbContext);
            Assert.IsNotNull(e3.Exception);
            Assert.IsTrue(e3.StartTime.Year > 2000);
            Assert.IsTrue(e3.EndTime.Year > 2000);
            Assert.IsTrue(e3.OperationId.HasValue());
            Assert.AreEqual("getdate", e3.OperationName);
            Assert.IsNotNull(e3.Command);
            Assert.IsNotNull(e3.DbCommand);
            Assert.AreEqual(true, e3.IsAsync);
        }
    }
}
