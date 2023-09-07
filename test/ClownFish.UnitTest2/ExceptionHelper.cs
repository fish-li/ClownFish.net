namespace ClownFish.UnitTest;

internal static class ExceptionHelper
{
    internal static Exception CreateException()
    {
        try {
            throw new MessageException("一个用于测试的异常");
        }
        catch( Exception ex ) {
            return ex;
        }
    }

    internal static Exception CreateException(string message)
    {
        try {
            try {
                throw new MessageException("这是个内部异常");
            }
            catch( Exception ex ) {
                throw new InvalidOperationException(message, ex);
            }
        }
        catch( Exception ex2 ) {
            return ex2;
        }
    }


    public static string ExecuteActionReturnErrorMessage(Action action)
    {
        try {
            action();
        }
        catch( Exception ex ) {
            return ex.Message;
        }


        throw new InternalTestFailureException("异常没有出现！");
    }

    public async static Task<string> ExecuteActionReturnErrorMessage(Func<Task> action)
    {
        try {
            await action();
        }
        catch( Exception ex ) {
            return ex.Message;
        }


        throw new InternalTestFailureException("异常没有出现！");
    }





    internal static MySql.Data.MySqlClient.MySqlException CreateMySqlException1(int errno, string msg = "xxxx")
    {
        ConstructorInfo ctor = typeof(MySql.Data.MySqlClient.MySqlException).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
            new Type[] { typeof(MySql.Data.MySqlClient.MySqlErrorCode), typeof(string) }, null);

        return (MySql.Data.MySqlClient.MySqlException)ctor.Invoke(new object[] { (MySql.Data.MySqlClient.MySqlErrorCode)errno, msg });
    }


    internal static MyDbException CreateMyDbException(string message = "xxxx")
    {
        return new MyDbException(message);
    }
}


public class MyDbException : DbException
{
    internal MyDbException(string message) : base(message)
    {
    }
}
