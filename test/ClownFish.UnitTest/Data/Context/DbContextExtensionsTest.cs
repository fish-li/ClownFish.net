namespace ClownFish.UnitTest.Data.Context;

[TestClass]
public class DbContextExtensionsTest
{
    [TestMethod]
    public void Test_GetFullName()
    {
        using( DbContext dbContext = DbContext.Create("sqlserver") ) {

            Assert.AreEqual("server", dbContext.GetObjectFullName("server"));

            dbContext.EnableDelimiter = true;
            Assert.AreEqual("[server]", dbContext.GetObjectFullName("server"));
        }

        using( DbContext dbContext = DbContext.Create("mysql") ) {

            Assert.AreEqual("server", dbContext.GetObjectFullName("server"));

            dbContext.EnableDelimiter = true;
            Assert.AreEqual("`server`", dbContext.GetObjectFullName("server"));
        }

        using( DbContext dbContext = DbContext.Create("postgresql") ) {

            Assert.AreEqual("server", dbContext.GetObjectFullName("server"));

            dbContext.EnableDelimiter = true;
            Assert.AreEqual("\"server\"", dbContext.GetObjectFullName("server"));
        }
    }


    [TestMethod]
    public void Test_ExecuteAction()
    {
        using( DbContext dbContext = DbContext.Create() ) {

            MyAssert.IsError<ArgumentNullException>(() => {
                DbContextExtensions.ExecuteAndIgnoreDuplicateInsertException(null, Action1);
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                DbContextExtensions.ExecuteAndIgnoreDuplicateInsertException(dbContext, null);
            });

            Assert.IsTrue(dbContext.ExecuteAndIgnoreDuplicateInsertException(Action1));

            Assert.IsFalse(dbContext.ExecuteAndIgnoreDuplicateInsertException(ThrowEx1));

            MyAssert.IsError<InvalidCastException>(() => {
                dbContext.ExecuteAndIgnoreDuplicateInsertException(ThrowEx2);
            });
        }
    }

    [TestMethod]
    public async Task Test_ExecuteActionAsync()
    {
        using( DbContext dbContext = DbContext.Create() ) {

            await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
               await  DbContextExtensions.ExecuteAndIgnoreDuplicateInsertExceptionAsync(null, Action2);
            });

            await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
                await DbContextExtensions.ExecuteAndIgnoreDuplicateInsertExceptionAsync(dbContext, null);
            });

            Assert.IsTrue(dbContext.ExecuteAndIgnoreDuplicateInsertExceptionAsync(Action2).GetAwaiter().GetResult());

            Assert.IsFalse(dbContext.ExecuteAndIgnoreDuplicateInsertExceptionAsync(ThrowEx1Async).GetAwaiter().GetResult());

            await MyAssert.IsErrorAsync<InvalidCastException>(async () => {
                await dbContext.ExecuteAndIgnoreDuplicateInsertExceptionAsync(ThrowEx2Async);
            });
        }
    }

    private void ThrowEx1()
    {
        throw ExceptionHelper.CreateSqlException(2601);
    }

    private void ThrowEx2()
    {
        throw new InvalidCastException();
    }

    private void Action1()
    {
        // ...........
    }

    private Task ThrowEx1Async()
    {
        throw ExceptionHelper.CreateSqlException(2601);
    }

    private Task ThrowEx2Async()
    {
        throw new InvalidCastException();
    }
    private Task Action2()
    {
        return Task.CompletedTask;
    }


    [TestMethod]
    public void Test_IsDuplicateInsert()
    {
        using( DbContext dbContext = DbContext.Create() ) {

            MyAssert.IsError<ArgumentNullException>(() => {
                DbContextExtensions.IsDuplicateInsert(null, new Exception());
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                DbContextExtensions.IsDuplicateInsert(dbContext, null);
            });

            Assert.IsTrue(dbContext.IsDuplicateInsert(ExceptionHelper.CreateSqlException(2601)));

            Assert.IsFalse(dbContext.IsDuplicateInsert(ExceptionHelper.CreateSqlException(260)));

            Assert.IsFalse(dbContext.IsDuplicateInsert(new Exception()));
        }
    }
}
